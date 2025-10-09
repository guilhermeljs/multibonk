using Il2Cpp;
using Il2CppAssets.Scripts.Actors;
using Il2CppAssets.Scripts.Inventory__Items__Pickups;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime;
using Il2CppRewired.Utils;
using MelonLoader;
using Multibonk.Networking.Comms.Base.Packet;
using UnityEngine;

namespace Multibonk.Game
{
    public class GameFunctions
    {

        public static void GetCharacterDataFromMainMenu()
        {
            if (GamePatchFlags.CharacterDataInitialized)
                return;

            GamePatchFlags.CharacterData.Clear();

            MyButtonCharacter[] buttons = UnityEngine.Object.FindObjectsOfType<MyButtonCharacter>();
            GamePatchFlags.CharacterData = buttons.Select(b => b.characterData).ToList();
            GamePatchFlags.CharacterDataInitialized = true;
        }

        public static Dictionary<int, List<GameObject>> GetAllSpawnedMapDataPrefabs()
        {
            var output = new Dictionary<int, List<GameObject>>();

            var allObjects = GameObject.FindObjectsOfType<GameObject>();

            var prefabs = GamePatchFlags.MapDataIndexedPrefabs;
            for (int i = 0; i < prefabs.Count; i++)
            {
                var prefab = prefabs[i];
                string targetName = prefab.name + "(Clone)";

                var instances = allObjects
                    // this can possibly instantiate wrong prefabs if the game dev instantiates two different prefabs with the exact same name on the same
                    // TODO: find a better way to find all those objects in the scene 
                    .Where(go => go.name == targetName || go.name == prefab.name)
                    .ToList();

                output.Add(i, instances);
            }

            return output;
        }

        public static SpawnedNetworkPlayer GetSpawnedPlayerFromId(ushort playerId)
        {
            if (!GamePatchFlags.PlayersCache.TryGetValue(playerId, out var obj) || obj.PlayerObject == null || obj.PlayerObject.IsNullOrDestroyed())
            {
                GamePatchFlags.PlayersCache.Remove(playerId);
                return null;
            }

            return obj;
        }


        /// <summary>
        /// Spawns a player in the map
        /// </summary>
        /// <param name="playerId">The id of the player in the lobby</param>
        /// <param name="character">Character chosen by the player</param>
        /// <param name="position">Spawn position</param>
        /// <param name="rotation">Spawn rotation</param>
        public static void SpawnNetworkPlayer(ushort playerId, ECharacter character, Vector3 position, Quaternion rotation)
        {
            var data = GamePatchFlags.CharacterData.Find(data => data.eCharacter == character);

            var player = new GameObject("player-from-id-" + playerId.ToString());
            player.transform.position = position;
            player.transform.rotation = rotation;

            var rendererContainer = new GameObject("NetworkPlayer");
            rendererContainer.transform.SetParent(player.transform);
            rendererContainer.layer = LayerMask.NameToLayer("Default");

            var renderer = rendererContainer.AddComponent<PlayerRenderer>();

            var inv = new PlayerInventory(data);
            renderer.SetCharacter(data, inv, position);
            renderer.CreateMaterials(1);

            rendererContainer.transform.localPosition = new Vector3(0, -(data.colliderHeight / 2), 0);
            rendererContainer.transform.localRotation = Quaternion.identity;

            GamePatchFlags.PlayersCache.Add(playerId, new SpawnedNetworkPlayer(player));
        }
    }


    public class SpawnedNetworkPlayer
    {
        public GameObject PlayerObject { get; set; }
        public float LastSeenTime { get; set; }

        public float LastTimeMoved { get; set; }

        public SpawnedNetworkPlayer(GameObject playerObject)
        {
            PlayerObject = playerObject;
            LastSeenTime = Time.time;
            LastTimeMoved = Time.time;
        }

        public bool IsNullOrDestroyed()
        {
            return PlayerObject == null || PlayerObject.Equals(null);
        }

        public void Move(Vector3 position)
        {
            PlayerObject.transform.position = position;
            LastTimeMoved = Time.time;

            var renderer = PlayerObject.GetComponentInChildren<PlayerRenderer>();

            if (!renderer.moving)
            {
                renderer.moving = true;
                renderer.ForceMoving(true);
            }
        }
        public void Rotate(Vector3 rotation)
        {
            PlayerObject.transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
 