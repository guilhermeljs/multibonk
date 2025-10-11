using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Game.Handlers;
using Multibonk.Game.World;

namespace Multibonk.Networking.Comms.Client.Handlers.Player
{
    public class PlayerRotatedPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.PLAYER_ROTATED_PACKET;

        private readonly GameWorld _world;

        public PlayerRotatedPacketHandler(GameWorld world)
        {
            _world = world;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new PlayerRotatedPacket(msg);

            GameDispatcher.Enqueue(() =>
            {
                var player = _world.CurrentSession?.PlayerManager.GetPlayer(packet.PlayerId);
                if (player != null)
                {
                    player.Rotate(packet.EulerAngles);
                }
            });
        }
    }
}
