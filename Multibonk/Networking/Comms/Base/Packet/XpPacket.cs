using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendXpPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.XP_PACKET;

        public SendXpPacket(int xp)
        {
            Message.WriteByte(Id);
            Message.WriteInt(xp);
        }
    }

    internal class XpPacket
    {
        public int Xp { get; private set; }

        public XpPacket(IncomingMessage msg)
        {
            Xp = msg.ReadInt();
        }
    }
}