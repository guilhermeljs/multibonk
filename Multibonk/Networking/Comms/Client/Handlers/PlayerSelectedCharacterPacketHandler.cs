using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Networking.Comms.Client.Handlers
{
    public class PlayerSelectedCharacterPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.PLAYER_SELECTED_CHARACTER;

        private LobbyContext LobbyContext { get; }

        public PlayerSelectedCharacterPacketHandler(LobbyContext lobby)
        {
            LobbyContext = lobby;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new PlayerSelectedCharacterPacket(msg);

            var targetPlayer = LobbyContext.GetPlayer(packet.PlayerId);
            if (targetPlayer != null)
            {
                targetPlayer.SelectedCharacter = packet.CharacterName;
            }
        }
    }
}
