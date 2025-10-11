using Multibonk.Game;
using Multibonk.Game.Handlers;
using Multibonk.Game.World;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Client.Handlers.Player
{
    public class PlayerAnimatorChangedPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.PLAYER_ANIMATOR_CHANGED_PACKET;

        private readonly GameWorld _world;

        public PlayerAnimatorChangedPacketHandler(GameWorld world)
        {
            _world = world;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new PlayerAnimatorChangedPacket(msg);

            GameDispatcher.Enqueue(() =>
            {
                var player = _world.CurrentSession?.PlayerManager.GetPlayer(packet.PlayerId);
                if (player != null)
                {
                    player.SetAnimation(packet.AnimationId, packet.Active);
                }
            });
        }
    }
}