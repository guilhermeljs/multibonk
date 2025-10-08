using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendLobbyPlayerListPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.LOBBY_PLAYER_LIST_PACKET;

        public SendLobbyPlayerListPacket(List<LobbyPlayer> players)
        {
            Message.WriteByte(Id);
            Message.WriteByte((byte)players.Count);

            foreach (var player in players)
            {
                Message.WriteUShort(player.UUID);
                Message.WriteString(player.Name);
                Message.WriteString(player.SelectedCharacter);
            }
        }
    }

    internal class LobbyPlayerListPacket
    {
        public IReadOnlyList<LobbyPlayer> Players { get; }

        public LobbyPlayerListPacket(IncomingMessage msg)
        {
            int count = msg.ReadByte();
            var players = new List<LobbyPlayer>(count);

            for (int i = 0; i < count; i++)
            {
                ushort uuid = msg.ReadUShort();
                string name = msg.ReadString();
                string selectedCharacter = msg.ReadString();
                players.Add(new LobbyPlayer(name, uuid, selectedCharacter));
            }

            Players = players;
        }
    }
}
