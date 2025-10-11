using Il2Cpp;
using UnityEngine;

namespace Multibonk.Game.World.Session.Managers
{
    public class MapManager
    {
        private static readonly System.Random _rng = new System.Random();
        public static int Seed { get; set; } = _rng.Next(int.MinValue, int.MaxValue);
        public static MapData SelectedMapData { get; set; }

        /// <summary>
        /// Returns a deterministic list of prefab GameObjects for the currently selected map.
        /// If SelectedMapData is unchanged, the list will always contain the same objects in the same order.
        /// 
        /// The list includes:
        /// - All shrines first.
        /// - All prefabs from each stage's random map objects, preserving order.
        ///
        /// When the client's MapData matches the server's, this ensures the same indexed prefab map.
        /// Prefabs can be shared by ID: a prefab indexed as ID 1 on the server will correspond to the same prefab indexed as ID 1 on the client.
        /// </summary>
        private static List<GameObject> MapDataIndexedPrefabs
        {
            get
            {
                if (SelectedMapData == null)
                    return new List<GameObject>();

                var list = new List<GameObject>();

                list.AddRange(SelectedMapData.shrines);

                return list
                 .Concat(
                     SelectedMapData.stages?
                         .SelectMany(s => s.randomMapObjects)
                         .SelectMany(d => d.prefabs)
                         .ToList()
                 )
                 .Concat(
                    SelectedMapData.stages?
                         .Select(s => s.stageTilePrefabs)
                         .SelectMany(t => t.flatTilePrefabs)
                         .SelectMany(prefab =>
                            Enumerable.Range(0, prefab.transform.childCount)
                                      .Select(i => prefab.transform.GetChild(i).gameObject))
                         .ToList()
                 )
                 .ToList();
            }
        }

        public Dictionary<int, List<GameObject>> GetAllSpawnedMapDataPrefabs()
        {
            var output = new Dictionary<int, List<GameObject>>();

            var allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

            var prefabs = MapDataIndexedPrefabs;
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

        public void SpawnMapObject(int prefabId, Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            var prefab = MapDataIndexedPrefabs[prefabId];
            var instance = UnityEngine.Object.Instantiate(prefab, position, rotation);
            instance.transform.localScale = localScale;
        }
    }
}
