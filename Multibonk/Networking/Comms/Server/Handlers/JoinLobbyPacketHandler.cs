using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Networking.Comms.Server.Handlers
{
    public class JoinLobbyPacketHandler : IServerPacketHandler
    {
        public byte PacketId => (byte)ClientSentPacketId.JOIN_LOBBY_PACKET;

        private LobbyContext LobbyContext { get; }

        public JoinLobbyPacketHandler(
            LobbyContext lobby
        )
        {
            LobbyContext = lobby;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            JoinLobbyPacket packet = new(msg);

            var newPlayer = new LobbyPlayer(name: packet.PlayerName, connection: conn);

            var currentPlayers = LobbyContext.GetPlayers().Prepend(newPlayer).ToList();

            var playerList = new SendLobbyPlayerListPacket(currentPlayers);
            conn.EnqueuePacket(playerList);

            LobbyContext.AddPlayer(
                newPlayer
            );
        }
    }
}
