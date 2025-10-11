using Multibonk.Game.Handlers;
using Multibonk.Game.World;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Client.Handlers
{
    internal class GamePausePacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.GAME_PAUSED;
        private readonly GameWorld _gameWorld;

        public GamePausePacketHandler(GameWorld world)
        {
            _gameWorld = world;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            GameDispatcher.Enqueue(() =>
            {
                _gameWorld.CurrentSession?.PauseManager?.ForcePauseGame();
            });
        }
    }
}
