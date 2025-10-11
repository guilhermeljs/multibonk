using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Game.Handlers;
using Multibonk.Game.World;

namespace Multibonk.Networking.Comms.Client.Handlers.Player
{
    public class SpawnPlayerPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.SPAWN_PLAYER_PACKET;

        private readonly GameWorld _world;

        public SpawnPlayerPacketHandler(GameWorld world)
        {
            _world = world;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new SpawnPlayerPacket(msg);

            GameDispatcher.Enqueue(() =>
            {
                _world.CurrentSession?.PlayerManager.SpawnPlayer(
                    packet.PlayerId,
                    packet.Character,
                    packet.Position,
                    packet.Rotation
                );
            });
        }
    }
}
