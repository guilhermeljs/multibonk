
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    internal class GamePausedPacket
    {
        public GamePausedPacket(IncomingMessage msg)
        {
        }
    }

    public class SendGamePausedPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.GAME_PAUSED;

        public SendGamePausedPacket()
        {
            Message.WriteByte(Id);
        }
    }
}
