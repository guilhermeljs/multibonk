using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Networking.Comms.Server.Handlers
{
    public class SelectCharacterPacketHandler : IServerPacketHandler
    {
        public byte PacketId => (byte)ClientSentPacketId.CHARACTER_SELECTION;
        private LobbyContext LobbyContext { get; }

        public SelectCharacterPacketHandler(LobbyContext lobby)
        {
            LobbyContext = lobby;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new SelectCharacterPacket(msg);

            var targetPlayer = LobbyContext.GetPlayer(conn);

            if (targetPlayer != null) 
            {

                targetPlayer.SelectedCharacter = packet.CharacterName;
            }



            var currentPlayers = LobbyContext.GetPlayers();
            var characterSelection = new SendPlayerSelectedCharacterPacket(targetPlayer.UUID, packet.CharacterName);


            foreach (var player in currentPlayers)
            {
                player.Connection?.EnqueuePacket(characterSelection);
            }
        }
    }
}
