using Il2Cpp;
using UnityEngine;

namespace Multibonk.Game
{

    public static class GamePatchFlags
    {
        private static readonly System.Random _rng = new System.Random();

        public static bool CharacterDataInitialized = false;
        public static List<CharacterData> CharacterData = new List<CharacterData>();

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
        public static List<GameObject> MapDataIndexedPrefabs
        {
            get {
                if (SelectedMapData == null)
                    return new List<GameObject>();

                var list = new List<GameObject>();

                list.AddRange(SelectedMapData.shrines);

                return list.Concat
                    (
                        SelectedMapData.stages?
                                .SelectMany(s => s.randomMapObjects)
                                .SelectMany(d => d.prefabs).ToList()
                    ).ToList();
            }
        }

        public static Dictionary<ushort, SpawnedNetworkPlayer> PlayersCache = new Dictionary<ushort, SpawnedNetworkPlayer>();

        public static int Seed { get; set; } = _rng.Next(int.MinValue, int.MaxValue);

        public static bool AllowStartMapCall { get; set; } = false;
        public static bool AllowAddXpCall { get; set; } = false;

        public static Vector3 LastPlayerPosition { get; set; }
        public static Quaternion LastPlayerRotation { get; set; }
    }
}
