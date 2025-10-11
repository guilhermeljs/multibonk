using MelonLoader;
using Multibonk.Game.World;
using Multibonk.Game.World.Session.Managers;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Lobby;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    public class StartGameEventHandler : GameEventHandler
    {
        public StartGameEventHandler(
            LobbyContext lobbyContext,
            GameWorld world
        )
        {
            GameEvents.ConfirmMapEvent += () =>
            {
                if (!LobbyPatchFlags.IsHosting)
                    return;

                world.StartGame();
            };

            world.SessionStarted += (session) =>
            {
                if (!LobbyPatchFlags.IsHosting)
                    return;

                MelonLogger.Msg($"Starting game with seed {MapManager.Seed}");

                var packet = new SendStartGamePacket(MapManager.Seed);

                foreach (var player in lobbyContext.GetPlayers())
                {
                    player.Connection?.EnqueuePacket(packet);
                }
            };

        }
    }
}
