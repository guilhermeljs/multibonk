using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Lobby;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Game.Handlers;
using Multibonk.Game;
using Multibonk.Game.World;

namespace Multibonk.Networking.Comms.Server.Handlers
{
    /// <summary>
    /// Class copied from PlayerMovePacketHandler
    /// </summary>
    public class PlayerRotatePacketHandler : IServerPacketHandler
    {
        public byte PacketId => (byte)ClientSentPacketId.PLAYER_ROTATE_PACKET;
        private readonly LobbyContext _lobbyContext;
        private readonly GameWorld _world;

        public PlayerRotatePacketHandler(LobbyContext lobbyContext, GameWorld world)
        {
            _lobbyContext = lobbyContext;
            _world = world;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new PlayerRotatePacket(msg);
            var playerId = _lobbyContext.GetPlayer(conn).UUID;

            GameDispatcher.Enqueue(() =>
            {
                var player = _world.CurrentSession?.PlayerManager.GetPlayer(playerId);
                if (player != null)
                {
                    player.Rotate(packet.Rotation.eulerAngles);
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




