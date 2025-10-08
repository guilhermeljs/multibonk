using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendSelectCharacterPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ClientSentPacketId.CHARACTER_SELECTION;

        public SendSelectCharacterPacket(string characterName)
        {
            Message.WriteByte(Id);
            Message.WriteString(characterName);
        }
    }

    public class SelectCharacterPacket
    {
        public string CharacterName { get; private set; }

        public SelectCharacterPacket(IncomingMessage msg)
        {
            CharacterName = msg.ReadString();
        }
    }
}
