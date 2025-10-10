using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendPlayerSelectedCharacterPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.PLAYER_SELECTED_CHARACTER;

        public SendPlayerSelectedCharacterPacket(ushort playerId, string characterName)
        {
            Message.WriteByte(Id);
            Message.WriteUShort(playerId);   // 2 bytes
            Message.WriteString(characterName);
        }
    }

    public class PlayerSelectedCharacterPacket
    {
        public ushort PlayerId { get; private set; }
        public string CharacterName { get; private set; }

        public PlayerSelectedCharacterPacket(IncomingMessage msg)
        {
            PlayerId = msg.ReadUShort();   // 2 bytes
            CharacterName = msg.ReadString();
        }
    }
}