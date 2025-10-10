using System;
using Multibonk.Networking.Lobby;
using UnityEngine;

namespace Multibonk.UserInterface.Window
{
    public class HostLobbyWindow : WindowBase
    {
        private readonly LobbyContext LobbyContext;
        private bool steamOverlayAvailable;
        private string steamTunnelStatus = string.Empty;

        public event Action OnCloseLobby;
        public event Action OnOptionsClicked;
        public event Action OnSteamOverlayClicked;

        public HostLobbyWindow(LobbyContext context) : base(new Rect(50, 50, 420, 280))
        {
            LobbyContext = context;
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

            GUILayout.Label("Host Lobby (Hide with F5)", labelStyle);
            GUILayout.Label("Connected Players:", labelStyle);

            foreach (var player in LobbyContext.GetPlayers())
            {
                GUILayout.Label($"{player.Name} - {player.Ping}ms - {player.SelectedCharacter}", labelStyle);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Leave Lobby"))
            {
                CloseLobby();
            }

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

        private void CloseLobby()
        {
            OnCloseLobby?.Invoke();
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
