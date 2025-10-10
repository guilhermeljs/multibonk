using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base.Packet.Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    public class PlayerMovementEventHandler : GameEventHandler
    {
        public PlayerMovementEventHandler(
            NetworkService network,
            LobbyContext lobbyContext
        )
        {


            GameEvents.PlayerMoveEvent += (pos) =>
            {
                if (LobbyPatchFlags.IsHosting)
                {
                    lobbyContext.GetPlayers().ForEach(player =>
                    {
                        var moved = new SendPlayerMovedPacket(lobbyContext.GetMyself().UUID, pos);
                        player.Connection?.EnqueuePacket(moved);
                    });
                }

                if (!LobbyPatchFlags.IsHosting)
                {
                    var characterSelection = new SendPlayerMovePacket(pos);
                    network.GetClientService().Enqueue(characterSelection);
                }
            };

            GameEvents.PlayerRotateEvent += (rot) =>
            {
                if (LobbyPatchFlags.IsHosting)
                {
                    lobbyContext.GetPlayers().ForEach(player =>
                    {
                        var rotated = new SendPlayerRotatedPacket(lobbyContext.GetMyself().UUID, rot.eulerAngles);
                        player.Connection?.EnqueuePacket(rotated);
                    });
                }

                if (!LobbyPatchFlags.IsHosting)
                {
                    var characterSelection = new SendPlayerRotatePacket(rot);
                    network.GetClientService().Enqueue(characterSelection);
                }
            };
        }
    }
}
