using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendJoinLobbyPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ClientSentPacketId.JOIN_LOBBY_PACKET;
        public SendJoinLobbyPacket(int version, string playerName)
        {
            Message.WriteByte(Id);

            Message.WriteInt(version);

            Message.WriteString(playerName);
        }
    }

    internal class JoinLobbyPacket
    {
        public int ModVersion { get; private set; }
        public string PlayerName { get; private set; }

        public JoinLobbyPacket(IncomingMessage msg)
        {
            ModVersion = msg.ReadInt();
            PlayerName = msg.ReadString();
        }
    }
}
