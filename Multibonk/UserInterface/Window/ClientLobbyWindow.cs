using System;
using Multibonk.Networking.Lobby;
using UnityEngine;

namespace Multibonk.UserInterface.Window
{
    public class ClientLobbyWindow : WindowBase
    {
        private readonly LobbyContext lobby;
        private bool steamOverlayAvailable;
        private string steamTunnelStatus = string.Empty;

        public event Action OnLeaveLobby;
        public event Action OnOptionsClicked;
        public event Action OnSteamOverlayClicked;

        public ClientLobbyWindow(LobbyContext lobby) : base(new Rect(50, 50, 360, 240))
        {
            this.lobby = lobby;
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

            GUILayout.Label("Client Lobby (Hide with F5)", labelStyle);
            GUILayout.Label("Connected Players:", labelStyle);

            foreach (var player in lobby.GetPlayers())
            {
                GUILayout.Label($"{player.Name} - {player.Ping}ms - {player.SelectedCharacter}", labelStyle);
            }

            if (GUILayout.Button("Leave Lobby"))
            {
                LeaveLobby();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Options"))
            {
                OnOptionsClicked?.Invoke();
            }

            bool originalState = GUI.enabled;
            GUI.enabled = steamOverlayAvailable;
            if (GUILayout.Button("Steam Friends Overlay"))
            {
                OnSteamOverlayClicked?.Invoke();
            }
            GUI.enabled = originalState;
            GUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(steamTunnelStatus))
            {
                GUILayout.Label(steamTunnelStatus, labelStyle);
            }

            GUILayout.EndArea();
        }

        private void LeaveLobby()
        {
            OnLeaveLobby?.Invoke();
        }

        public void SetSteamOverlayAvailability(bool available)
        {
            steamOverlayAvailable = available;
        }

        public void SetSteamTunnelStatus(string status)
        {
            steamTunnelStatus = status;
        }
    }
}
