using System.Threading.Tasks;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    internal class GameLoadedPacket
    {
        public GameLoadedPacket(IncomingMessage msg) { }
    }

    public class SendGameLoadedPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ClientSentPacketId.GAME_LOADED_PACKET;

        public SendGameLoadedPacket()
        {
            Message.WriteByte(Id);
        }
    }
}
