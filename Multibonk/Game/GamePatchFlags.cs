using Il2Cpp;
using UnityEngine;

namespace Multibonk.Game
{

    public static class GamePatchFlags
    {
        private static readonly System.Random _rng = new System.Random();

        public static bool CharacterDataInitialized = false;
        public static List<CharacterData> CharacterData = new List<CharacterData>();

        public static Dictionary<ushort, SpawnedNetworkPlayer> PlayersCache = new Dictionary<ushort, SpawnedNetworkPlayer>();

        public static int Seed { get; set; } = _rng.Next(int.MinValue, int.MaxValue);

        public static bool AllowStartMapCall { get; set; } = false; 

        public static Vector3 LastPlayerPosition { get; set; }
        public static Quaternion LastPlayerRotation { get; set; }
    }
}
