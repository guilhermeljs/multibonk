using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using MelonLoader;

namespace Multibonk.Networking.Comms.Client.Handlers
{
    public class MapFinishedLoadingPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.MAP_FINISHED_LOADING;

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new MapFinishedLoadingPacket(msg);

            GameDispatcher.Enqueue(() =>
            {
                MelonLogger.Msg("Server finished loading packet");
            });
        }
    }
}
