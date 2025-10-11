
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    internal class GameUnpausedPacket
    {
        public GameUnpausedPacket(IncomingMessage msg)
        {
        }
    }


    public class SendGameUnpausedPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.GAME_UNPAUSED;

        public SendGameUnpausedPacket()
        {
            Message.WriteByte(Id);
        }
    }
}
