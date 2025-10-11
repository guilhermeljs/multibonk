using MelonLoader;
using Multibonk.Game.Handlers;
using Multibonk.Game.World;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Client.Handlers
{
    public class EnemyDeathPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.ENEMY_DEATH_PACKET;
        private readonly GameWorld _gameWorld;

        public EnemyDeathPacketHandler(GameWorld world)
        {
            _gameWorld = world;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new EnemyDeathPacket(msg);

            GameDispatcher.Enqueue(() =>
            {
                _gameWorld.CurrentSession?.EnemyManager?.KillEnemy(packet.EnemyId);
            });
        }
    }
}
