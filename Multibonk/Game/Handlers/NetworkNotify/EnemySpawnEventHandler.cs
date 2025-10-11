using Il2Cpp;
using Il2CppAssets.Scripts.Actors.Enemies;
using Il2CppAssets.Scripts.Managers;
using Il2CppRewired;
using Multibonk.Game.World;
using Multibonk.Game.World.Session.Managers;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    internal class EnemySpawnEventHandler: IGameEventHandler
    {
        public EnemySpawnEventHandler(LobbyContext lobby, NetworkService network, GameWorld world)
        {
            GameEvents.EnemySpawnedEvent += (e) =>
            {
                uint? enemyId = world.CurrentSession?.EnemyManager.RegisterEnemySpawned(e);
                if (enemyId == null) return;

                var prefabId = WorldEnemyManager.GetEnemyPrefabIndex(e);

                if (!LobbyPatchFlags.IsHosting)
                {
                    var enemySpawned = new SendSpawnEnemyPacket(enemyId.Value, prefabId, e.transform.position);
                    network.GetClientService().Enqueue(enemySpawned);
                }else
                {
                    var enemySpawned = new SendEnemySpawnedPacket(enemyId.Value, prefabId, e.transform.position, lobby.GetMyself().UUID);
                    foreach (var targetPlayer in lobby.GetPlayers())
                    {
                        if (targetPlayer.Connection == null)
                            continue;

                        targetPlayer.Connection.EnqueuePacket(enemySpawned);
                    }
                }
            };
        }
    }
}