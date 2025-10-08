using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Server.Handlers
{

    public class GameLoadedPacketHandler : IServerPacketHandler
    {
        public byte PacketId => (byte)ClientSentPacketId.GAME_LOADED_PACKET;

        public GameLoadedPacketHandler()
        {
        }
        public void Handle(IncomingMessage msg, Connection conn)
        {
        }
    }
}
