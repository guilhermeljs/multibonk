using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendPlayerSelectedCharacterPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.PLAYER_SELECTED_CHARACTER;

        public SendPlayerSelectedCharacterPacket(byte playerId, string characterName)
        {
            Message.WriteByte(Id);
            Message.WritePlayerId(playerId);
            Message.WriteString(characterName);
        }
    }

    public class PlayerSelectedCharacterPacket
    {
        public ushort PlayerId { get; private set; }
        public string CharacterName { get; private set; }

        public PlayerSelectedCharacterPacket(IncomingMessage msg)
        {
            PlayerId = msg.ReadPlayerId();
            CharacterName = msg.ReadString();
        }
    }
}