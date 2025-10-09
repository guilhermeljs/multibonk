using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    public class PlayerLevelEventHandler : GameEventHandler
    {
        public PlayerLevelEventHandler(
            NetworkService network,
            LobbyContext lobbyContext
        )
        {
            GameEvents.PlayerXpAddedEvent += (xp) =>
            {
                if (LobbyPatchFlags.IsHosting && Preferences.LevelSynchronization.Value)
                {
                    lobbyContext.GetPlayers().ForEach(player =>
                    {
                        var xpPacket = new SendXpPacket(xp);
                        player.Connection?.EnqueuePacket(xpPacket);
                    });
                }

                // TODO: Synchronize server preferences with client preferences
                // So we won't need to send this packet if LevelSynchronization is setted to false.
                // Today, if LevelSynchronization is disabled we only ignore the píckup packet in server side
                if(!LobbyPatchFlags.IsHosting)
                {
                    var pickupXpPacket = new SendPlayerPickupXpPacket(xp);
                    network.GetClientService().Enqueue(pickupXpPacket);
                }
            };
        }
    }
}
