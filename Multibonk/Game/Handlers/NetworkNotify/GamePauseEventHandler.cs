using System.Diagnostics;
using MelonLoader;
using Multibonk.Game.World;
using Multibonk.Game.World.Session.Managers;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    public class GamePauseEventHandler : GameEventHandler
    {
        public GamePauseEventHandler(LobbyContext context, NetworkService network, GameWorld world)
        {

            GamePatchEvents.PauseEvent += () =>
            {
                MelonLogger.Msg("Called --------------");
                var playerId = context.GetMyself().UUID;

                if (!LobbyPatchFlags.IsHosting)
                {
                    network.GetClientService().Enqueue(new SendPauseGamePacket());
                    return;
                }

                var pauseManager = world.CurrentSession?.PauseManager;
                pauseManager?.HandlePauseAction(playerId, World.Session.Managers.PauseAction.Pause);
            };

            GamePatchEvents.UnpauseEvent += () =>
            {
                var playerId = context.GetMyself().UUID;

                if (!LobbyPatchFlags.IsHosting)
                {
                    network.GetClientService().Enqueue(new SendUnpauseGamePacket());
                    return;
                }

                var pauseManager = world.CurrentSession?.PauseManager;
                pauseManager?.HandlePauseAction(playerId, World.Session.Managers.PauseAction.Unpause);
            };

            PauseManager.OnGamePaused += () =>
            {
                if (!LobbyPatchFlags.IsHosting)
                    return;

                var packet = new SendGamePausedPacket();

                foreach (var p in context.GetPlayers())
                    if (p.Connection != null)
                        p.Connection.EnqueuePacket(packet);
            };

            PauseManager.OnGameUnpaused += () =>
            {
                if (!LobbyPatchFlags.IsHosting)
                    return;

                var packet = new SendGameUnpausedPacket();

                foreach (var p in context.GetPlayers())
                    if (p.Connection != null)
                        p.Connection.EnqueuePacket(packet);
            };

        }
    }
}
