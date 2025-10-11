using Il2Cpp;
using Il2CppAssets.Scripts.Actors.Player;
using MelonLoader;
using Multibonk.Game.World;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;
using UnityEngine;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    public class GameLoadedEventHandler : GameEventHandler
    {
        public LobbyContext LobbyContext { get; private set; }
        public NetworkService NetworkService { get; private set; }

        public GameLoadedEventHandler(LobbyContext lobbyContext, NetworkService networkService, GameWorld world)
        {
            LobbyContext = lobbyContext;
            NetworkService = networkService;

            world.GameLoaded += (session) =>
            {
                if (!LobbyPatchFlags.IsHosting)
                    return;

                lobbyContext.GetPlayers()
                .Where(player => player.Connection != null)
                .ToList()
                .ForEach(player =>
                {
                    var character = Enum.Parse<ECharacter>(player.SelectedCharacter);
                    world.CurrentSession.PlayerManager.SpawnPlayer(player.UUID, character, MyPlayer.Instance.transform.position, MyPlayer.Instance.transform.rotation);

                    lobbyContext.GetPlayers()
                        .Where(target => target != player)
                        .ToList()
                        .ForEach(target =>
                        {
                            var character = Enum.Parse<ECharacter>(target.SelectedCharacter);
                            var packet = new SendSpawnPlayerPacket(character, target.UUID, MyPlayer.Instance.transform.position, MyPlayer.Instance.transform.rotation);
                            player.Connection.EnqueuePacket(packet);
                        });
                });
            };

            world.GameLoaded += (session) =>
            {
                if (!LobbyPatchFlags.IsHosting)
                    return;


                var spawnedObjects = session.MapManager.GetAllSpawnedMapDataPrefabs();
                var maxChunkSize = 50;

                MelonLogger.Msg("All objects: " + spawnedObjects.Count);

                lobbyContext.GetPlayers()
                    .Where(player => player.Connection != null)
                    .ToList()
                    .ForEach(player =>
                    {
                        foreach (var entry in spawnedObjects)
                        {
                            int listId = entry.Key;
                            var allObjects = entry.Value;
                            int total = allObjects.Count;
                            int chunks = Mathf.CeilToInt((float)total / maxChunkSize);

                            if (total == 0)
                                MelonLogger.Msg($"Prefab with id {entry.Key} is empty");

                            for (int i = 0; i < chunks; i++)
                            {
                                var chunkObjects = allObjects
                                    .Skip(i * maxChunkSize)
                                    .Take(maxChunkSize)
                                    .ToList();

                                var chunkPacket = new SendMapObjectChunkPacket(listId, chunkObjects);
                                MelonLogger.Msg($"Sending chunk of {chunkObjects.Count} gameobjects from prefab {listId} packet length {chunkPacket.ToBuffer().Length + 2}");

                                player.Connection.EnqueuePacket(chunkPacket);
                            }
                        }

                        var finishedPacket = new SendMapFinishedLoadingPacket();
                        player.Connection.EnqueuePacket(finishedPacket);
                    });
            };

            // This event shouldn't be here. TODO: Fix this
            GameEvents.SetBoolEvent += (anim, param, v) =>
            {
                if (MyPlayer.Instance.playerRenderer.animator != anim)
                    return;

                if (LobbyPatchFlags.IsHosting)
                {
                    HandleHostingAnimatorEvent(param, v);
                }else
                {
                    HandleClientAnimatorEvent(param, v);
                }

            };

        }

        private void HandleClientAnimatorEvent(string param, bool v)
        {
            var animId = SendPlayerAnimatorPacket.StringToAnimationId(param);
            if (animId == null)
                return;

            var packet = new SendPlayerAnimatorPacket(
                animId.Value,
                v
            );

            NetworkService.GetClientService().Enqueue(packet);
        }

        private void HandleHostingAnimatorEvent(string param, bool v)
        {
            var animationId = SendPlayerAnimatorPacket.StringToAnimationId(param);
            if (animationId == null)
                return;

            var serverSentPacket = new SendPlayerAnimatorChangedPacket(
                LobbyContext.GetMyself().UUID,
                (PlayerAnimationId)animationId,
                v
            );

            foreach (var player in LobbyContext.GetPlayers())
            {
                if (player.Connection == null)
                    continue;

                player.Connection.EnqueuePacket(serverSentPacket);
            }
        }
    }

}
