using Multibonk.Game;
using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base.Packet.Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Networking.Comms.Server.Handlers
{
    public class PlayerMovePacketHandler : IServerPacketHandler
    {

        public byte PacketId => (byte)ClientSentPacketId.PLAYER_MOVE_PACKET;

        private readonly LobbyContext _lobbyContext;

        public PlayerMovePacketHandler(LobbyContext lobbyContext)
        {
            _lobbyContext = lobbyContext;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new PlayerMovePacket(msg);

            var playerId = _lobbyContext.GetPlayer(conn).UUID;

            GameDispatcher.Enqueue(() =>
            {
                var go = GameFunctions.GetSpawnedPlayerFromId(playerId);

                if (go != null)
                {
                    go.Move(packet.Position);
                }
            });

            foreach (var player in _lobbyContext.GetPlayers())
            {
                if (player.Connection == null || player.UUID == playerId)
                    continue;

                var sendPacket = new SendPlayerMovedPacket(
                    playerId,
                    packet.Position
                );

                player.Connection.EnqueuePacket(sendPacket);
            }

        }
    }
}
