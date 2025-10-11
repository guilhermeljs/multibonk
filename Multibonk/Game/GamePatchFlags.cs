using System.Collections.Concurrent;
using Il2Cpp;
using Il2CppAssets.Scripts.Actors.Enemies;
using Il2CppAssets.Scripts.Game.MapGeneration;
using Multibonk.Game.Utils;
using UnityEngine;

namespace Multibonk.Game
{

    public static class GamePatchFlags
    {
        public static bool AllowStartMapCall { get; set; } = false;
        public static bool AllowAddXpCall { get; set; } = false;
        public static bool AllowSpawnEnemyCall { get; set; } = false;
        public static bool AllowPauseCall { get; set; } = false;
        public static bool AllowUnpauseCall { get; set; } = false;
    }
}
