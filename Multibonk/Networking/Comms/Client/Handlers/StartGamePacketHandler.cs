using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Game.Handlers;
using Multibonk.Game.World;

namespace Multibonk.Networking.Comms.Client.Handlers
{
    public class StartGamePacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.START_GAME;

        private readonly GameWorld _world;

        public StartGamePacketHandler(GameWorld world)
        {
            _world = world;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new StartGamePacket(msg);
            var seed = packet.Seed;

            GameDispatcher.Enqueue(() =>
            {
                _world.StartGame(seed);
            });
        }
    }
}
