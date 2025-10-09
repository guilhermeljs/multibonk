using Il2Cpp;
using Il2CppAssets.Scripts.Actors.Player;
using MelonLoader;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Lobby;
using UnityEngine;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    public class GameLoadedEventHandler : GameEventHandler
    {
        public GameLoadedEventHandler(LobbyContext lobbyContext)
        {

            GameEvents.GameLoadedEvent += () =>
            {
                if (!LobbyPatchFlags.IsHosting)
                    return;

                lobbyContext.GetPlayers()
                .Where(player => player.Connection != null)
                .ToList()
                .ForEach(player =>
                {
                    var character = Enum.Parse<ECharacter>(player.SelectedCharacter);
                    var data = GamePatchFlags.CharacterData.Find(d => d.eCharacter == character);
                    GameFunctions.SpawnNetworkPlayer(player.UUID, character, MyPlayer.Instance.transform.position, MyPlayer.Instance.transform.rotation);

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

            GameEvents.GameLoadedEvent += () =>
            {
                var spawnedObjects = GameFunctions.GetAllSpawnedMapDataPrefabs();


                var maxChunkSize = 50;

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
        }

    }
}
