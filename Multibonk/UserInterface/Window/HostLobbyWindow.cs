using Multibonk.Networking.Lobby;
using UnityEngine;

namespace Multibonk.UserInterface.Window
{
    public class HostLobbyWindow : WindowBase
    {
        private LobbyContext LobbyContext { get; }
        public event Action OnCloseLobby;

        private bool levelSyncToggle;

        public HostLobbyWindow(LobbyContext context) : base(new Rect(50, 50, 400, 300))
        {
            LobbyContext = context;
        }

        protected override void RenderWindow(Rect rect)
        {
            float lineHeight = 25f;
            int playerCount = LobbyContext.GetPlayers().Count;
            int extraLines = 3;
            float buttonHeight = 25f;
            float padding = 10f;

            float totalHeight = (playerCount+extraLines) * lineHeight + buttonHeight + padding * 2;

            rect.height = totalHeight;

            GUILayout.BeginArea(rect, GUI.skin.window);
            GUI.Box(new Rect(0, 0, rect.width, rect.height), GUIContent.none, GUI.skin.window);

            GUILayout.Label("Host Lobby (Hide with F5)", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } });

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Preferences.LevelSynchronization.Value ? "Level Sync: ON" : "Level Sync: OFF"))
            {
                Preferences.LevelSynchronization.Value = !Preferences.LevelSynchronization.Value;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("Connected Players:", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } });

            foreach (var player in LobbyContext.GetPlayers())
                GUILayout.Label($"{player.Name} - {player.Ping}ms - {player.SelectedCharacter}", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } });


            if (GUILayout.Button("Leave Lobby", GUILayout.Height(buttonHeight))) CloseLobby();

            GUILayout.EndArea();
        }


        private void CloseLobby()
        {
            OnCloseLobby?.Invoke();
        }
    }
}
