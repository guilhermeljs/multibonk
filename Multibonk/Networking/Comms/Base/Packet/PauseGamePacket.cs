using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    internal class PauseGamePacket
    {
        public PauseGamePacket(IncomingMessage msg) { }
    }

    public class SendPauseGamePacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ClientSentPacketId.PAUSE_GAME;

        public SendPauseGamePacket()
        {
            Message.WriteByte(Id);
        }
    }
}
