using Multibonk.Networking.Lobby;
using UnityEngine;

namespace Multibonk.UserInterface.Window
{
    public class HostLobbyWindow : WindowBase
    {
        private LobbyContext LobbyContext { get; }
        public event Action OnCloseLobby;

        public HostLobbyWindow(LobbyContext context) : base(new Rect(50, 50, 400, 300))
        {
            LobbyContext = context;
        }

        protected override void RenderWindow(Rect rect)
        {
            GUILayout.BeginArea(rect, GUI.skin.window);
            GUI.Box(new Rect(0, 0, rect.width, rect.height), GUIContent.none, GUI.skin.window);

            GUILayout.Label("Host Lobby (Hide with F5)", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } });
            GUILayout.Label("Connected Players:", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } });

            foreach (var player in LobbyContext.GetPlayers())
                GUILayout.Label($"{player.Name} - {player.Ping}ms - {player.SelectedCharacter}", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } });

            if (GUILayout.Button("Leave Lobby")) CloseLobby();

            GUILayout.EndArea();
        }


        private void CloseLobby()
        {
            OnCloseLobby?.Invoke();
        }
    }
}
