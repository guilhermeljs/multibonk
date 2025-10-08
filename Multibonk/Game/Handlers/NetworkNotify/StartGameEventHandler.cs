using MelonLoader;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Lobby;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    public class StartGameEventHandler : GameEventHandler
    {
        public StartGameEventHandler(
            LobbyContext lobbyContext
        )
        {
            GameEvents.ConfirmMapEvent += () =>
            {
                MelonLogger.Msg($"Starting game with seed {GamePatchFlags.Seed}");
                var packet = new SendStartGamePacket(GamePatchFlags.Seed);

                foreach (var player in lobbyContext.GetPlayers())
                {
                    player.Connection?.EnqueuePacket(packet);
                }
            };

        }
    }
}
