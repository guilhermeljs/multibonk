using UnityEngine;
using Multibonk.UserInterface.Window;
using MelonLoader;
using Multibonk.Networking.Lobby;

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

        public UIManager(
            ConnectionWindow connectionWindow,
            ClientLobbyWindow clientLobbyWindow,
            HostLobbyWindow hostLobbyWindow,

            LobbyContext lobby,
            LobbyService lobbyService
        )
        {
            this.connectionWindow = connectionWindow;
            this.clientLobbyWindow = clientLobbyWindow;
            this.hostLobbyWindow = hostLobbyWindow;

            connectionWindow.OnConnectClicked += (args) =>
            {
                var parts = args.IP.Split(':');
                if (parts.Length != 2) return;

                var ip = parts[0];
                if (!int.TryParse(parts[1], out var port)) return;

                MelonLogger.Msg("Connecting to ip " + args.IP);

                lobbyService.JoinLobby(ip, port, args.PlayerName);

            };

            connectionWindow.OnStartServerClicked += (args) =>
            {
                lobbyService.CreateLobby(args.PlayerName);
            };

            hostLobbyWindow.OnCloseLobby += () => lobbyService.CloseLobby();

            clientLobbyWindow.OnLeaveLobby += () => lobbyService.CloseLobby();

            lobby.OnLobbyJoin += (_) => SetState(UIState.ClientLobby);
            lobby.OnLobbyCreated += (_) => SetState(UIState.HostLobby);
            lobby.OnLobbyClosed += (_) => SetState(UIState.Connection);
            lobby.OnLobbyJoinFailed += (_) => SetState(UIState.Connection);

        }

        public void OnGUI()
        {
            Event e = Event.current;
            if(e.rawType == EventType.KeyDown && e.keyCode == KeyCode.F5)
            {
                _showingMenuBuffer = !_showingMenuBuffer;
            }

            if(e.rawType == EventType.Layout && _showingMenuBuffer != IsShowingMenu)
            {
                IsShowingMenu = _showingMenuBuffer;
            }

            if (IsShowingMenu) {
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
            }
        }
        public void SetState(UIState newState)
        {
            currentState = newState;
        }

        public UIState GetState() => currentState;

    }
}