using Il2Cpp;
using MelonLoader;
using Multibonk.Game.World.Session;
using Multibonk.Game.World.Session.Managers;
using Multibonk.Networking.Lobby;

namespace Multibonk.Game.World
{
    public class GameWorld
    {
        public GameSession CurrentSession { get; private set; }

        public event Action<GameSession> SessionStarted;
        public event Action<GameSession> GameLoaded;

        private LobbyContext _lobby;

        public GameWorld(LobbyContext context) 
        {
            _lobby = context;

            GameEvents.GameLoadedEvent += ()=>
            {
                GameLoaded?.Invoke(CurrentSession);
            };
        }

        public void StartGame(int seed = -1)
        {
            CurrentSession = new GameSession(_lobby.GetMyself().UUID);

            if (seed != -1)
                MapManager.Seed = seed;

            var ui = UnityEngine.Object.FindObjectOfType<MapSelectionUi>();
            if (ui != null)
            {
                GamePatchFlags.AllowStartMapCall = true;
                MelonLogger.Msg("Starting map..");
                ui.StartMap();

                MelonLogger.Msg("Map started succesfully..");
                GamePatchFlags.AllowStartMapCall = false;
            }


            SessionStarted?.Invoke(CurrentSession);
        }

    }
}
