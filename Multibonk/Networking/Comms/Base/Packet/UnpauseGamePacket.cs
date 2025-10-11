using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    internal class UnpauseGamePacket
    {
        public UnpauseGamePacket(IncomingMessage msg) { }
    }

    public class SendUnpauseGamePacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ClientSentPacketId.UNPAUSE_GAME;

        public SendUnpauseGamePacket()
        {
            Message.WriteByte(Id);
        }
    }
}
