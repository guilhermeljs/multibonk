using Multibonk.Game.Handlers;
using Multibonk.Game.World;
using Multibonk.Game.World.Session.Managers;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Networking.Comms.Server.Handlers
{
    internal class PauseGamePacketHandler : IServerPacketHandler
    {
        private readonly LobbyContext _lobbyContext;
        private readonly GameWorld _gameWorld;

        public PauseGamePacketHandler(LobbyContext lobbyContext, GameWorld gameWorld)
        {
            _lobbyContext = lobbyContext;
            _gameWorld = gameWorld;
        }

        public byte PacketId => (byte)ClientSentPacketId.PAUSE_GAME;

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var playerId = _lobbyContext.GetPlayer(conn).UUID;

            GameDispatcher.Enqueue(() =>
            {
                _gameWorld.CurrentSession?.PauseManager?.HandlePauseAction(playerId, PauseAction.Pause);
            });
        }
    }
}