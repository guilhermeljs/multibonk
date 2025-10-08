using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    internal class StartGamePacket
    {
        public int Seed { get; private set; }

        public StartGamePacket(IncomingMessage msg)
        {
            Seed = msg.ReadInt();
        }
    }

    public class SendStartGamePacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.START_GAME;

        public SendStartGamePacket(int seed)
        {
            Message.WriteByte(Id);
            Message.WriteInt(seed);
        }
    }
}
