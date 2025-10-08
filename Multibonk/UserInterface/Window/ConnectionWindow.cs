using UnityEngine;

namespace Multibonk.UserInterface.Window
{
    public class ConnectionWindowEventArgs
    {
        public string IP { get; }
        public string PlayerName { get; }

        public ConnectionWindowEventArgs(string playerName, string ip)
        {
            IP = ip;
            PlayerName = playerName;
        }
    }

    public class ConnectionWindow : WindowBase
    {
        public event Action<ConnectionWindowEventArgs> OnStartServerClicked;
        public event Action<ConnectionWindowEventArgs> OnConnectClicked;

        private string ipAddress = "127.0.0.1";
        private string playerName = "PlayerName";
        private bool nameIsFocused = false;
        private bool ipIsFocused = false;

        public ConnectionWindow() : base(new Rect(10, 10, 300, 200))
        {
            ipAddress = Preferences.IpAddress.Value;
            playerName = Preferences.PlayerName.Value;
        }

        protected override void RenderWindow(Rect rect)
        {
            GUILayout.BeginArea(rect, GUI.skin.window);
            GUI.Box(new Rect(0, 0, rect.width, rect.height), GUIContent.none, GUI.skin.window);

            GUILayout.Label("Multibonk Connection Menu (Hide with F5)", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } });


            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } });
            playerName = Utils.CustomTextField(playerName, ref nameIsFocused, new Rect(0, 0, 150, 20));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("IP:", new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } });
            ipAddress = Utils.CustomTextField(ipAddress, ref ipIsFocused, new Rect(0, 0, 150, 20));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Start Server"))
            {
                Preferences.IpAddress.Value = ipAddress;
                Preferences.PlayerName.Value = playerName;
                OnStartServer();
            }

            if (GUILayout.Button("Connect"))
            {
                Preferences.IpAddress.Value = ipAddress;
                Preferences.PlayerName.Value = playerName;
                OnConnect();
            }

            GUILayout.EndArea();
        }

        private void OnStartServer() => OnStartServerClicked?.Invoke(new ConnectionWindowEventArgs(playerName, ipAddress));
        private void OnConnect() => OnConnectClicked?.Invoke(new ConnectionWindowEventArgs(playerName, ipAddress));
    }
}