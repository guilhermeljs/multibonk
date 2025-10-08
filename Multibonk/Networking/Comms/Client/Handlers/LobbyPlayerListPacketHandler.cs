using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base;

namespace Multibonk.Networking.Comms.Client.Handlers
{
    public class LobbyPlayerListPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.LOBBY_PLAYER_LIST_PACKET;

        private LobbyContext LobbyContext { get; }


        public LobbyPlayerListPacketHandler(LobbyContext lobby)
        {
            LobbyContext = lobby;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            LobbyPlayerListPacket packet = new(msg);

            LobbyContext.SetMyself(packet.Players[0]);

            foreach (var player in packet.Players.Skip(1))
            {
                LobbyContext.AddPlayer(player);
            }
        }
    }
}
