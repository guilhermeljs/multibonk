using MelonLoader;
using Multibonk.Networking.Lobby;
using UnityEngine;

namespace Multibonk.UserInterface.Window
{
    public class ClientLobbyWindow : WindowBase
    {
        private LobbyContext lobby;
        public event Action OnLeaveLobby;

        public ClientLobbyWindow(LobbyContext lobby) : base(new Rect(50, 50, 300, 200)) 
        {
            this.lobby = lobby;
        }

        protected override void RenderWindow(Rect rect)
        {
            GUILayout.BeginArea(rect, GUI.skin.window);
            GUI.Box(new Rect(0, 0, rect.width, rect.height), GUIContent.none, GUI.skin.window);

            GUILayout.Label("Client Lobby (Hide with F5)", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } });
            GUILayout.Label("Connected Players:", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } });

            foreach (var player in lobby.GetPlayers())
                GUILayout.Label($"{player.Name} - {player.Ping}ms - {player.SelectedCharacter}", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } });

            if (GUILayout.Button("Leave Lobby")) LeaveLobby();

            GUILayout.EndArea();
        }

        private void LeaveLobby()
        {
            OnLeaveLobby?.Invoke();
        }
    }

}
