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
        public static Vector3 LastPlayerPosition { get; set; }
        public static Quaternion LastPlayerRotation { get; set; }
    }
}
