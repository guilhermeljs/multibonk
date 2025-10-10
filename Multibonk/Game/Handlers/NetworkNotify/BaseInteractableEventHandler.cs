using Il2Cpp;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    /// <summary>
    /// Handler responsible for processing BaseInteractable events and sending them over the network
    /// </summary>
    public class BaseInteractableEventHandler : GameEventHandler
    {
        private readonly NetworkService _network;
        private readonly LobbyContext _lobbyContext;

        public BaseInteractableEventHandler(
            NetworkService network,
            LobbyContext lobbyContext
        )
        {
            _network = network;
            _lobbyContext = lobbyContext;

            // BaseInteractable spawn event (server/host only)
            GameEvents.BaseInteractableSpawnedEvent += (interactable) =>
            {
                if (LobbyPatchFlags.IsHosting)
                {
                    int instanceId = interactable.GetInstanceID();
                    
                    // Register the interactable in cache
                    GamePatchFlags.TrackInteractable(instanceId, interactable);

                    // Prepare data to send
                    string prefabName = interactable.gameObject.name.Replace("(Clone)", "").Trim();
                    var position = interactable.transform.position;
                    var rotation = interactable.transform.rotation;
                    var scale = interactable.transform.localScale;

                    // Create the packet
                    var spawnPacket = new SendSpawnInteractablePacket(
                        instanceId,
                        prefabName,
                        position,
                        rotation,
                        scale,
                        interactable.isItemSource,
                        interactable.showOutline
                    );

                    // Send to all clients
                    _lobbyContext.GetPlayers().ForEach(player =>
                    {
                        if (player.Connection != null)
                        {
                            player.Connection.EnqueuePacket(spawnPacket);
                        }
                    });
                }
            };

            // BaseInteractable destruction event
            GameEvents.BaseInteractableDestroyedEvent += (instanceId) =>
            {
                if (LobbyPatchFlags.IsHosting)
                {
                    // Remove from cache
                    GamePatchFlags.UntrackInteractable(instanceId);

                    // Create destruction packet
                    var destroyPacket = new SendDestroyInteractablePacket(instanceId);

                    // Send to all clients
                    _lobbyContext.GetPlayers().ForEach(player =>
                    {
                        if (player.Connection != null)
                        {
                            player.Connection.EnqueuePacket(destroyPacket);
                        }
                    });
                }
                else
                {
                    // Client notifies the server
                    var destroyPacket = new SendClientDestroyInteractablePacket(instanceId);
                    _network.GetClientService().Enqueue(destroyPacket);
                }
            };
        }
    }
}

