using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendPlayerPickupXpPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ClientSentPacketId.PICKUP_XP_PACKET;

        public SendPlayerPickupXpPacket(int xp)
        {
            Message.WriteByte(Id);
            Message.WriteInt(xp);
        }
    }

    internal class PlayerPickupXpPacket
    {
        public int Xp { get; private set; }

        public PlayerPickupXpPacket(IncomingMessage msg)
        {
            Xp = msg.ReadInt();
        }
    }
}