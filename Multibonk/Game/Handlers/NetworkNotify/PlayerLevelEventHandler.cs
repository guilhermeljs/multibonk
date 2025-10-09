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
                if (LobbyPatchFlags.IsHosting)
                {
                    lobbyContext.GetPlayers().ForEach(player =>
                    {
                        var xpPacket = new SendXpPacket(xp);
                        player.Connection?.EnqueuePacket(xpPacket);
                    });
                }

                if(!LobbyPatchFlags.IsHosting)
                {
                    var pickupXpPacket = new SendPlayerPickupXpPacket(xp);
                    network.GetClientService().Enqueue(pickupXpPacket);
                }
            };
        }
    }
}
