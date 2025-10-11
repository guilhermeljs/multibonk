using Multibonk.Game.Handlers;
using Multibonk.Game.World;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;
using MelonLoader;

namespace Multibonk.Networking.Comms.Server.Handlers
{
    public class KillEnemyPacketHandler : IServerPacketHandler
    {
        private readonly LobbyContext _lobbyContext;
        private readonly GameWorld _gameWorld;

        public KillEnemyPacketHandler(LobbyContext lobbyContext, GameWorld world)
        {
            _lobbyContext = lobbyContext;
            _gameWorld = world;
        }

        public byte PacketId => (byte)ClientSentPacketId.KILL_ENEMY_PACKET;

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new KillEnemyPacket(msg);
            var sender = _lobbyContext.GetPlayer(conn);

            GameDispatcher.Enqueue(() =>
            {
                _gameWorld.CurrentSession?.EnemyManager?.KillEnemy(packet.EnemyId);
            });


            var deathPacket = new SendEnemyDeathPacket(packet.EnemyId);
            foreach (var player in _lobbyContext.GetPlayers())
            {
                if (player.Connection == null || player == sender)
                    continue;

                player.Connection.EnqueuePacket(deathPacket);
            }
        }
    }
}
