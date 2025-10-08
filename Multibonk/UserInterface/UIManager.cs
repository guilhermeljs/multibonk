using System;
using MelonLoader;
using Multibonk.Networking.Lobby;
using Multibonk.Networking.Steam;
using Multibonk.UserInterface.Window;
using UnityEngine;

namespace Multibonk
{
    public class UIManager
    {
        public enum UIState
        {
            Connection,
            ClientLobby,
            HostLobby
        }

        private UIState currentState = UIState.Connection;

        public bool IsShowingMenu { get; set; } = true;
        private bool _showingMenuBuffer = true;

        public ConnectionWindow connectionWindow;
        public ClientLobbyWindow clientLobbyWindow;
        public HostLobbyWindow hostLobbyWindow;
        private readonly OptionsWindow optionsWindow;
        private readonly LobbyService lobbyService;
        private readonly SteamTunnelService steamTunnelService;

        private SteamTunnelEndpoint? displayedSteamEndpoint;
        private string steamTunnelStatusMessage = string.Empty;
        private bool lastOverlayAvailability;

        public UIManager(
            ConnectionWindow connectionWindow,
            ClientLobbyWindow clientLobbyWindow,
            HostLobbyWindow hostLobbyWindow,
            OptionsWindow optionsWindow,
            LobbyContext lobby,
            LobbyService lobbyService,
            SteamTunnelService steamTunnelService
        )
        {
            this.connectionWindow = connectionWindow;
            this.clientLobbyWindow = clientLobbyWindow;
            this.hostLobbyWindow = hostLobbyWindow;
            this.optionsWindow = optionsWindow;
            this.lobbyService = lobbyService;
            this.steamTunnelService = steamTunnelService;

            connectionWindow.OnConnectClicked += args =>
            {
                MelonLogger.Msg($"Connecting to ip {args.Endpoint}");
                lobbyService.JoinLobby(args.Address, args.Port, args.PlayerName);
            };

            connectionWindow.OnStartServerClicked += args =>
            {
                lobbyService.CreateLobby(args.PlayerName, args.Port);
            };

            connectionWindow.OnOptionsClicked += ToggleOptions;
            connectionWindow.OnSteamOverlayClicked += HandleSteamOverlayRequest;

            hostLobbyWindow.OnCloseLobby += () => lobbyService.CloseLobby();
            hostLobbyWindow.OnOptionsClicked += ToggleOptions;
            hostLobbyWindow.OnSteamOverlayClicked += HandleSteamOverlayRequest;

            clientLobbyWindow.OnLeaveLobby += () => lobbyService.CloseLobby();
            clientLobbyWindow.OnOptionsClicked += ToggleOptions;
            clientLobbyWindow.OnSteamOverlayClicked += HandleSteamOverlayRequest;

            optionsWindow.OpenSteamOverlayRequested += HandleSteamOverlayRequest;
            optionsWindow.PreferencesChanged += () => RefreshSteamTunnelStatus(forceUpdate: true);

            lobby.OnLobbyJoin += _ =>
            {
                connectionWindow.SetConnectionError(string.Empty);
                SetState(UIState.ClientLobby);
            };
            lobby.OnLobbyCreated += _ =>
            {
                connectionWindow.SetConnectionError(string.Empty);
                SetState(UIState.HostLobby);
            };
            lobby.OnLobbyClosed += _ =>
            {
                connectionWindow.SetConnectionError(string.Empty);
                SetState(UIState.Connection);
            };
            lobby.OnLobbyJoinFailed += HandleLobbyJoinFailed;

            RefreshSteamTunnelStatus(forceUpdate: true);
        }

        public void OnGUI()
        {
            Event e = Event.current;
            if (e.rawType == EventType.KeyDown && e.keyCode == KeyCode.F5)
            {
                _showingMenuBuffer = !_showingMenuBuffer;
            }

            if (e.rawType == EventType.Layout)
            {
                if (_showingMenuBuffer != IsShowingMenu)
                {
                    IsShowingMenu = _showingMenuBuffer;
                }

                RefreshSteamTunnelStatus();
            }

            if (!IsShowingMenu)
            {
                if (optionsWindow.IsOpen)
                {
                    optionsWindow.Hide();
                }

                return;
            }

            switch (currentState)
            {
                case UIState.Connection:
                    connectionWindow.Handle();
                    break;

                case UIState.ClientLobby:
                    clientLobbyWindow.Handle();
                    break;

                case UIState.HostLobby:
                    hostLobbyWindow.Handle();
                    break;
            }

            if (optionsWindow.IsOpen)
            {
                optionsWindow.Handle();
            }
        }

        public void SetState(UIState newState)
        {
            currentState = newState;
        }

        public UIState GetState() => currentState;

        private void ToggleOptions()
        {
            if (optionsWindow.IsOpen)
            {
                optionsWindow.Hide();
            }
            else
            {
                optionsWindow.Show();
            }
        }

        private void HandleSteamOverlayRequest()
        {
            if (!steamTunnelService.TryOpenFriendsOverlay())
            {
                RefreshSteamTunnelStatus(forceUpdate: true);
            }
        }

        private void RefreshSteamTunnelStatus(bool forceUpdate = false)
        {
            bool overlayAvailable = steamTunnelService.IsOverlayAvailable;
            SteamTunnelEndpoint? endpoint = null;
            string status;

            if (!overlayAvailable)
            {
                status = "Steam overlay is unavailable. Make sure Steam is running and overlay access is enabled.";
            }
            else if (steamTunnelService.TryPeekEndpoint(out var pending))
            {
                endpoint = pending;
                status = $"Steam invite ready: {pending.Address}:{pending.Port}.";
            }
            else
            {
                status = "Open the Steam friends overlay to invite or join friends.";
            }

            bool overlayChanged = forceUpdate || overlayAvailable != lastOverlayAvailability;
            if (overlayChanged)
            {
                connectionWindow.SetSteamOverlayAvailability(overlayAvailable);
                clientLobbyWindow.SetSteamOverlayAvailability(overlayAvailable);
                hostLobbyWindow.SetSteamOverlayAvailability(overlayAvailable);
                optionsWindow.SetSteamOverlayAvailability(overlayAvailable);
                lastOverlayAvailability = overlayAvailable;
            }

            if (forceUpdate || !string.Equals(status, steamTunnelStatusMessage, StringComparison.Ordinal))
            {
                steamTunnelStatusMessage = status;
                connectionWindow.SetSteamTunnelStatus(status);
                clientLobbyWindow.SetSteamTunnelStatus(status);
                hostLobbyWindow.SetSteamTunnelStatus(status);
                optionsWindow.SetSteamTunnelStatus(status);
            }

            if (endpoint.HasValue)
            {
                bool shouldUpdateEndpoint = forceUpdate || !displayedSteamEndpoint.HasValue || !displayedSteamEndpoint.Value.Equals(endpoint.Value);
                if (shouldUpdateEndpoint)
                {
                    displayedSteamEndpoint = endpoint;
                    connectionWindow.SetIpAddress(endpoint.Value.ToString());
                    AttemptAutoJoinFromSteam(endpoint.Value);
                }
            }
            else if (displayedSteamEndpoint.HasValue)
            {
                displayedSteamEndpoint = null;
            }
        }

        private void AttemptAutoJoinFromSteam(SteamTunnelEndpoint endpoint)
        {
            if (currentState != UIState.Connection)
            {
                return;
            }

            if (!steamTunnelService.TryConsumeEndpoint(out var consumed))
            {
                return;
            }

            var playerName = connectionWindow.GetPlayerName();
            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = Preferences.PlayerName.Value;
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "Player";
            }

            Preferences.PlayerName.Value = playerName;

            var message = $"Connecting to Steam invite {consumed.Address}:{consumed.Port}...";
            steamTunnelStatusMessage = message;
            connectionWindow.SetConnectionError(string.Empty);
            connectionWindow.SetSteamTunnelStatus(message);
            clientLobbyWindow.SetSteamTunnelStatus(message);
            hostLobbyWindow.SetSteamTunnelStatus(message);
            optionsWindow.SetSteamTunnelStatus(message);

            MelonLogger.Msg($"Automatically joining Steam tunnel endpoint {consumed}.");
            lobbyService.JoinLobby(consumed.Address, consumed.Port, playerName);
        }

        private void HandleLobbyJoinFailed(string reason)
        {
            var message = string.IsNullOrWhiteSpace(reason)
                ? "Failed to join lobby."
                : reason;

            MelonLogger.Warning($"Lobby join failed: {message}");

            connectionWindow.SetConnectionError(message);

            steamTunnelStatusMessage = message;
            connectionWindow.SetSteamTunnelStatus(message);
            clientLobbyWindow.SetSteamTunnelStatus(message);
            hostLobbyWindow.SetSteamTunnelStatus(message);
            optionsWindow.SetSteamTunnelStatus(message);

            SetState(UIState.Connection);
        }
    }
}
