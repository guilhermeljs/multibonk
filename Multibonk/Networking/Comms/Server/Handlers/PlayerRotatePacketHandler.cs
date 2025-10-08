using Multibonk.Game;
using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Networking.Comms.Server.Handlers
{
    /// <summary>
    /// Class copied from PlayerMovePacketHandler
    /// </summary>
    public class PlayerRotatePacketHandler : IServerPacketHandler
    {
        public byte PacketId => (byte)ClientSentPacketId.PLAYER_ROTATE_PACKET;
        private readonly LobbyContext _lobbyContext;

        public PlayerRotatePacketHandler(LobbyContext lobbyContext)
        {
            _lobbyContext = lobbyContext;
        }


        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new PlayerRotatePacket(msg);

            var playerId = _lobbyContext.GetPlayer(conn).UUID;

            GameDispatcher.Enqueue(() =>
            {
                var go = GameFunctions.GetSpawnedPlayerFromId(playerId);

                if (go != null)
                {
                    go.Rotate(packet.Rotation.eulerAngles);
                }
            });


            foreach (var player in _lobbyContext.GetPlayers())
            {
                if (player.Connection == null || player.UUID == playerId)
                    continue;

                var sendPacket = new SendPlayerRotatedPacket(
                    playerId,
                    packet.Rotation.eulerAngles
                );

                player.Connection.EnqueuePacket(sendPacket);
            }
        }
    }
}




