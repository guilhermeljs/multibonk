using System;
using UnityEngine;
using Multibonk.Networking;

namespace Multibonk.UserInterface.Window
{
    public class ConnectionWindowEventArgs
    {
        public string PlayerName { get; }
        public string Address { get; }
        public int Port { get; }
        public string Endpoint => $"{Address}:{Port}";

        public ConnectionWindowEventArgs(string playerName, string address, int port)
        {
            PlayerName = playerName;
            Address = address;
            Port = port;
        }
    }

    public class ConnectionWindow : WindowBase
    {
        public event Action<ConnectionWindowEventArgs> OnStartServerClicked;
        public event Action<ConnectionWindowEventArgs> OnConnectClicked;
        public event Action OnOptionsClicked;
        public event Action OnSteamOverlayClicked;

        private string ipAddress = $"{NetworkDefaults.DefaultAddress}:{NetworkDefaults.DefaultPort}";
        private string listenPort = NetworkDefaults.DefaultPort.ToString();
        private string playerName = "PlayerName";
        private bool nameIsFocused;
        private bool ipIsFocused;
        private bool listenPortIsFocused;
        private bool steamOverlayAvailable;
        private string steamTunnelStatus = string.Empty;
        private string connectionErrorMessage = string.Empty;

        public ConnectionWindow() : base(new Rect(10, 10, 340, 260))
        {
            playerName = Preferences.PlayerName.Value;
        }

        protected override void RenderWindow(Rect rect)
        {
            GUILayout.BeginArea(rect, GUI.skin.window);
            GUI.Box(new Rect(0, 0, rect.width, rect.height), GUIContent.none, GUI.skin.window);

            var labelStyle = new GUIStyle(GUI.skin.label)
            {
                wordWrap = true
            };
            labelStyle.normal.textColor = Color.white;

            GUILayout.Label("Multibonk Connection Menu (Hide with F5)", labelStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", labelStyle);
            playerName = Utils.CustomTextField(playerName, ref nameIsFocused, new Rect(0, 0, 160, 22));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("IP:", labelStyle);
            string newEndpoint = Utils.CustomTextField(ipAddress, ref ipIsFocused, new Rect(0, 0, 160, 22));
            if (!string.Equals(newEndpoint, ipAddress, StringComparison.Ordinal))
            {
                ipAddress = newEndpoint;
                connectionErrorMessage = string.Empty;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Listen Port:", labelStyle);
            string newListenPort = Utils.CustomTextField(listenPort, ref listenPortIsFocused, new Rect(0, 0, 80, 22));
            if (!string.Equals(newListenPort, listenPort, StringComparison.Ordinal))
            {
                listenPort = newListenPort;
                connectionErrorMessage = string.Empty;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Options"))
            {
                OnOptionsClicked?.Invoke();
            }

            bool shouldEnableOverlayButton = steamOverlayAvailable;
            bool previousGuiState = GUI.enabled;
            GUI.enabled = shouldEnableOverlayButton;
            if (GUILayout.Button("Steam Friends Overlay"))
            {
                OnSteamOverlayClicked?.Invoke();
            }
            GUI.enabled = previousGuiState;
            GUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(steamTunnelStatus))
            {
                GUILayout.Label(steamTunnelStatus, labelStyle);
            }

            if (!string.IsNullOrEmpty(connectionErrorMessage))
            {
                var errorStyle = new GUIStyle(labelStyle)
                {
                    normal = { textColor = Color.red }
                };
                GUILayout.Label(connectionErrorMessage, errorStyle);
            }

            if (GUILayout.Button("Start Server"))
            {
                Preferences.PlayerName.Value = playerName;
                OnStartServer();
            }

            if (GUILayout.Button("Connect"))
            {
                Preferences.PlayerName.Value = playerName;
                OnConnect();
            }

            GUILayout.EndArea();
        }

        private void OnStartServer()
        {
            if (!TryParseListenPort(listenPort, out var port, out var error))
            {
                connectionErrorMessage = error;
                return;
            }

            listenPort = port.ToString();
            connectionErrorMessage = string.Empty;
            OnStartServerClicked?.Invoke(new ConnectionWindowEventArgs(playerName, NetworkDefaults.DefaultAddress, port));
        }

        private void OnConnect()
        {
            if (!TryParseEndpoint(ipAddress, out var address, out var port, out var error))
            {
                connectionErrorMessage = error;
                return;
            }

            connectionErrorMessage = string.Empty;
            OnConnectClicked?.Invoke(new ConnectionWindowEventArgs(playerName, address, port));
        }

        public void SetSteamOverlayAvailability(bool isAvailable)
        {
            steamOverlayAvailable = isAvailable;
        }

        public void SetSteamTunnelStatus(string status)
        {
            steamTunnelStatus = status;
        }

        public void SetIpAddress(string address)
        {
            if (!string.IsNullOrWhiteSpace(address))
            {
                ipAddress = address;
                connectionErrorMessage = string.Empty;
            }
        }

        public string GetPlayerName() => playerName;

        public void SetConnectionError(string message)
        {
            connectionErrorMessage = message ?? string.Empty;
        }

        private bool TryParseEndpoint(string input, out string address, out int port, out string error)
        {
            address = string.Empty;
            port = NetworkDefaults.DefaultPort;
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Enter a server address to connect.";
                return false;
            }

            string trimmed = input.Trim();

            if (Uri.TryCreate($"tcp://{trimmed}", UriKind.Absolute, out var uri) && !string.IsNullOrEmpty(uri.Host))
            {
                address = uri.Host;
                port = uri.IsDefaultPort ? NetworkDefaults.DefaultPort : uri.Port;
            }
            else
            {
                var parts = trimmed.Split(':');
                if (parts.Length == 1)
                {
                    address = parts[0];
                    port = NetworkDefaults.DefaultPort;
                }
                else if (parts.Length == 2 && int.TryParse(parts[1], out var parsedPort))
                {
                    address = parts[0];
                    port = parsedPort;
                }
                else
                {
                    error = "Use the format host:port (example: 127.0.0.1:25565).";
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                error = "Server address cannot be empty.";
                return false;
            }

            if (port <= 0 || port > 65535)
            {
                error = "Port must be between 1 and 65535.";
                return false;
            }

            return true;
        }

        private bool TryParseListenPort(string input, out int port, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                port = NetworkDefaults.DefaultPort;
                return true;
            }

            if (!int.TryParse(input.Trim(), out port) || port <= 0 || port > 65535)
            {
                port = NetworkDefaults.DefaultPort;
                error = "Listen port must be between 1 and 65535.";
                return false;
            }

            return true;
        }
    }
}
