
using MelonLoader;
using Multibonk.Game.World;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    public class EnemyDeathEventHandler: IGameEventHandler
    {
        public EnemyDeathEventHandler(LobbyContext lobby, NetworkService network, GameWorld world)
        {
            GamePatchEvents.EnemyDiedEvent += (enemy) =>
            {
                if (enemy == null)
                    return;

                uint enemyId = world.CurrentSession.EnemyManager.GetIdFromEnemy(enemy);

                if (LobbyPatchFlags.IsHosting)
                {
                    var deathPacket = new SendEnemyDeathPacket(enemyId);
                    foreach (var targetPlayer in lobby.GetPlayers())
                    {
                        if (targetPlayer.Connection == null)
                            continue;

                        targetPlayer.Connection.EnqueuePacket(deathPacket);
                    }
                }
                else
                {
                    var killPacket = new SendKillEnemyPacket(enemyId);
                    network.GetClientService().Enqueue(killPacket);
                }
            };
        }
    }
}
