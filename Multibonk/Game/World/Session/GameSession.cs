using Il2CppAssets.Scripts.Actors.Enemies;
using Multibonk.Game.Utils;
using Multibonk.Game.World.Session.Managers;
using Multibonk.Networking.Lobby;
using UnityEngine;

namespace Multibonk.Game.World.Session
{
    // <summary>
    /// Manages the dynamic state of a single game run/session.
    /// It should be created when a run starts and discarded/reset when the run ends.
    /// </summary>
    public class GameSession
    {
        public NetworkPlayerManager PlayerManager { get; private set; }
        public WorldEnemyManager EnemyManager { get; private set; }
        public MapManager MapManager { get; private set; }
        public PauseManager PauseManager { get; private set; }

        public GameSession(byte myClientId)
        {
            PlayerManager = new NetworkPlayerManager();
            EnemyManager = new WorldEnemyManager(myClientId);
            MapManager = new MapManager();
            PauseManager = new PauseManager();
        }

    }
}
