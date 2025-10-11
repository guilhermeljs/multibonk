

using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Actors;
using Il2CppAssets.Scripts.Actors.Enemies;
using Il2CppAssets.Scripts.Actors.Player;
using Il2CppAssets.Scripts.Game.MapGeneration;
using Il2CppAssets.Scripts.Inventory__Items__Pickups;
using Il2CppAssets.Scripts.Managers;
using Il2CppAssets.Scripts.Utility;
using Il2CppInventory__Items__Pickups.Xp_and_Levels;
using Il2CppTMPro;
using MelonLoader;
using Multibonk.Game.World.Session.Managers;
using Multibonk.Networking.Lobby;
using UnityEngine;
using UnityEngine.UI;

namespace Multibonk.Game.Patches
{
    public static class MainMenuPatches
    {
#pragma warning disable IDE0051
        private static bool IsHovered(MyButtonCharacter btn) => btn.hoverOverlay.activeSelf;


        [HarmonyPatch(typeof(MainMenu), "GoToMapSelection")]
        class ConfirmCharacterPatch
        {
            static bool Prefix()
            {
                if (LobbyPatchFlags.IsHosting)
                    return true;

                var ui = UnityEngine.Object.FindObjectOfType<CharacterInfoUI>();

                if (ui)
                {
                    var buttonConfirm = ui.GetComponentsInChildren<Button>(true)
                      .FirstOrDefault(b => b.name == "B_Confirm");

                    buttonConfirm.GetComponentInChildren<TMP_Text>().text = "Aguardando host...";
                    buttonConfirm.GetComponent<ResizeOnLocalization>().DelayedRefresh();

                    GamePatchEvents.TriggerConfirmCharacter();
                }

                //return false;
                return true;
            }
        }



        //[HarmonyPatch(typeof(MyPlayer), "FixedUpdate")]
        //class PlayerUpdatedPatch
        //{
        //    static void Postfix()
        //    {
        //        var myPlayer = MyPlayer.Instance;
        //        if (myPlayer == null) return;
        //        if (myPlayer.gameObject.IsNullOrDestroyed()) return;

        //        const float positionThreshold = 0.05f;
        //        const float rotationThreshold = 10f;

        //        if ((myPlayer.transform.position - GamePatchFlags.LastPlayerPosition).sqrMagnitude > positionThreshold * positionThreshold)
        //        {
        //            GamePatchFlags.LastPlayerPosition = myPlayer.transform.position;
        //            GameEvents.TriggerPlayerMoved(myPlayer.transform.position);
        //        }

        //        var rotation = myPlayer.playerRenderer.transform.rotation;

        //        if (Quaternion.Angle(rotation, GamePatchFlags.LastPlayerRotation) > rotationThreshold)
        //        {
        //            GamePatchFlags.LastPlayerRotation = rotation;
        //            GameEvents.TriggerPlayerRotated(rotation);
        //        }
        //    }
        //}


        [HarmonyPatch(typeof(MyTime), "Pause")]
        class PausePatch
        {
            static bool Prefix()
            {
                GamePatchEvents.TriggerPauseEvent();
                return true;
            }
        }

        [HarmonyPatch(typeof(MyTime), "Unpause")]
        class UnpausePatch
        {
            static bool Prefix()
            {
                if(GamePatchFlags.AllowUnpauseCall)
                {
                    return true;
                }

                GamePatchEvents.TriggerUnpauseEvent();
                return false;
            }
        }


        [HarmonyPatch(typeof(MyPlayer), "Start")]
        class GameStartedPatch
        {
            static void Postfix()
            {
                GamePatchEvents.TriggerGameLoadedEvent();
            }
        }

        [HarmonyPatch(typeof(MapEntry), "OnMapSelected")]
        class MapEntrySelectedPatch
        {
            static void Postfix(SelectionGroupToggleSingleButton __0, MapData __1)
            {
                GamePatchEvents.TriggerMapChanged(__1);
            }
        }

        [HarmonyPatch(typeof(MapSelectionUi), "StartMap")]
        class ConfirmMapPatch
        {
            static bool Prefix()
            {
                if (!GamePatchFlags.AllowStartMapCall)
                {
                    GamePatchEvents.TriggerConfirmMap();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch]
        class EnemyDiedPatch
        {
            [HarmonyPatch(typeof(Enemy), "EnemyDied", new Type[] { })]
            [HarmonyPostfix]
            static void Postfix_NoParams(Enemy __instance)
            {
                GamePatchEvents.TriggerEnemyDie(__instance);
            }

            [HarmonyPatch(typeof(Enemy), "EnemyDied", new Type[] { typeof(DamageContainer) })]
            [HarmonyPostfix]
            static void Postfix_WithDamage(Enemy __instance)
            {
                GamePatchEvents.TriggerEnemyDie(__instance);
            }
        }

        [HarmonyPatch(typeof(CharacterInfoUI), "OnCharacterSelected")]
        class CharacterSelectionPatch
        {
            static void Postfix(MyButtonCharacter btn)
            {
                if (btn != null)
                {
                    GamePatchEvents.TriggerCharacterChanged(btn.characterData);
                }
            }
        }


        [HarmonyPatch(typeof(ProceduralTileGeneration), "Generate")]
        class MapGenerationPatch
        {
            static void Prefix(Il2Cpp.ProceduralTileGeneration __instance, UnityEngine.Vector3 __result, ref UnityEngine.Vector3 __0, Il2Cpp.StageData __1, Il2CppAssets.Scripts.MapGeneration.ProceduralTiles.MapParameters __2, ref bool __3)
            {
                MelonLogger.Msg("Patching debug seed");
                __instance.debugSeed = MapManager.Seed;
                __3 = true;
            }
        }

        /// <summary>
        /// Spawn all chests generated randomly
        /// </summary>
        [HarmonyPatch(typeof(SpawnInteractables), "SpawnChests")]
        class ChestPatch
        {
            static bool Prefix() => LobbyPatchFlags.IsHosting;
        }

        /// <summary>
        /// Spawns some trees, interactables, doesn't seem to have a pattern
        /// </summary>
        [HarmonyPatch(typeof(SpawnInteractables), "SpawnOther")]
        class SpawnOtherPatch
        {
            static bool Prefix() => LobbyPatchFlags.IsHosting;
        }
        /// <summary>
        /// ???
        /// </summary>

        [HarmonyPatch(typeof(SpawnInteractables), "SpawnRails")]
        class SpawnRailsPatch
        {
            static bool Prefix() => LobbyPatchFlags.IsHosting;
        }

        /// <summary>
        /// ???
        /// </summary>
        [HarmonyPatch(typeof(SpawnInteractables), "SpawnShit")]
        class SpawnShitPatch
        {
            static bool Prefix() => LobbyPatchFlags.IsHosting;
        }

        /// <summary>
        /// Spawns random kind of interactable shrines, those where you spawn bosses or interact pressing E.
        /// The ChargeShrines (those who give you stats looks like it is not included in this function, looks like they're instanced using RandomObjectSpawner)
        /// </summary>
        [HarmonyPatch(typeof(SpawnInteractables), "SpawnShrines")]
        class SpawnShrinesPatch
        {
            static bool Prefix() => LobbyPatchFlags.IsHosting;
        }

        [HarmonyPatch(typeof(RandomObjectPlacer), "Generate")]
        class GenerateHook
        {
            static bool Prefix() => LobbyPatchFlags.IsHosting;
        }


        [HarmonyPatch(typeof(PlayerRenderer), "OnDamage")]
        class DamageWrapperPatch
        {
            static bool Prefix(PlayerRenderer __instance)
            {
                if (__instance.gameObject.layer == LayerMask.NameToLayer("Player"))
                    return true;

                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerXp), "AddXp")]
        class AddXpPatch
        {
            static bool Prefix(int __0)
            {
                // This means that our managed code is calling AddXp and not the game. So the function must run correctly
                // This guarantees that we won't trigger the AddXpEvent from a Xp that was received from the network
                if (GamePatchFlags.AllowAddXpCall)
                    return true;

                GamePatchEvents.TriggerAddXpEvent(__0);

                return true;
            }
        }

        [HarmonyPatch(typeof(Animator), "SetBool", new System.Type[] { typeof(string), typeof(bool) })]
        class SetBoolPatch
        {
            static void Prefix(Animator __instance, string __0, bool __1)
            {
                bool currentValue = __instance.GetBool(__0);
                if (currentValue != __1)
                {
                    GamePatchEvents.TriggerSetBool(__instance, __0, __1);
                }
            }
        }

        [HarmonyPatch(typeof(Animator), "SetFloat", new System.Type[] { typeof(string), typeof(float) })]
        class SetFloatPatch
        {
            static void Prefix(Animator __instance, string __0, float __1)
            {
                float currentValue = __instance.GetFloat(__0);
                if (currentValue != __1)
                {
                    GamePatchEvents.TriggerSetFloat(__instance, __0, __1);
                }
            }
        }

        [HarmonyPatch(typeof(Animator), "SetInteger", new System.Type[] { typeof(string), typeof(int) })]
        class SetIntegerPatch
        {
            static void Prefix(Animator __instance, string __0, int __1)
            {
                int currentValue = __instance.GetInteger(__0);
                if (currentValue != __1)
                {
                    GamePatchEvents.TriggerSetInt(__instance, __0, __1);
                }
            }
        }

        [HarmonyPatch(typeof(Animator), "SetTrigger", new System.Type[] { typeof(string) })]
        class SetTriggerPatch
        {
            static void Prefix(Animator __instance, string __0)
            {
                GamePatchEvents.TriggerSetTrigger(__instance, __0);
            }
        }

        [HarmonyPatch(typeof(EnemyManager), "SpawnEnemy", new System.Type[] { typeof(EnemyData), typeof(int), typeof(bool), typeof(EEnemyFlag), typeof(bool) })]
        class SpawnEnemyPatch1
        {
            static bool Prefix(EnemyManager __instance, EnemyData __0, int __1, bool __2, EEnemyFlag __3, bool __4)
            {
                // only triggers the spawn enemy if the game called it
                if (!GamePatchFlags.AllowSpawnEnemyCall)
                {
                    GamePatchEvents.TriggerSpawnEnemy(__instance, __0, __1, __2, __3, __4);
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(EnemyManager), "SpawnEnemy", new System.Type[] { typeof(EnemyData), typeof(Vector3), typeof(int), typeof(bool), typeof(EEnemyFlag), typeof(bool) })]
        class SpawnEnemyPatch2
        {
            static bool Prefix(EnemyManager __instance, EnemyData __0, Vector3 __1, int __2, bool __3, EEnemyFlag __4, bool __5)
            {
                if (!GamePatchFlags.AllowSpawnEnemyCall)
                {
                    GamePatchEvents.TriggerSpawnEnemy(__instance, __0, __1, __2, __3, __4, __5);
                }


                return true;
            }
        }


        [HarmonyPatch(typeof(EnemyManager), "SpawnEnemy", new System.Type[] { typeof(EnemyData), typeof(int), typeof(bool), typeof(EEnemyFlag), typeof(bool) })]
        class SpawnEnemyPostfix1
        {
            static void Postfix(Enemy __result)
            {
                /// 
                if (__result != null && !GamePatchFlags.AllowSpawnEnemyCall)
                {
                    GamePatchEvents.TriggerEnemySpawned(__result);
                }
            }
        }

        [HarmonyPatch(typeof(EnemyManager), "SpawnEnemy", new System.Type[] { typeof(EnemyData), typeof(Vector3), typeof(int), typeof(bool), typeof(EEnemyFlag), typeof(bool) })]
        class SpawnEnemyPostfix2
        {
            static void Postfix(Enemy __result)
            {
                if (__result != null && !GamePatchFlags.AllowSpawnEnemyCall)
                {
                    GamePatchEvents.TriggerEnemySpawned(__result);
                }
            }
        }

        //[HarmonyPatch(typeof(MapGenerationController), "Awake")]
        //class GeneratorAll
        //{
        //    static bool Prefix(RandomObjectPlacer __instance)
        //    {
        //        MelonLogger.Msg("Generatig controller called");

        //        return false;
        //    }
        //}

        [HarmonyPatch(typeof(RandomObjectPlacer), "GenerateInteractables")]
        class GenerateHookPatch2
        {
            static bool Prefix(RandomObjectPlacer __instance)
            {
                if(LobbyPatchFlags.IsHosting)
                    return true;

                MelonLogger.Msg("Generating interactables called");

                return false;
            }
        }


        /// <summary>
        /// Spawns a List<GameObject> of objects in the tile meshes.
        /// Common prefabs spawned are: Some trees, boxes, pilars and castles
        /// </summary>
        [HarmonyPatch(typeof(GenerateTileObjects), "Generate")]
        class GenerateTileObjectsT
        {
            static bool Prefix(ref List<GameObject> __0)
            {

                if (LobbyPatchFlags.IsHosting)
                    return true;



                __0 = new List<GameObject>();

                return false;
            }
        }

        /// <summary>
        /// Spawns a lot of objects that come from MapData and StageData
        /// </summary>

        [HarmonyPatch(typeof(RandomObjectPlacer), "RandomObjectSpawner")]
        class GenerateHookPatch3
        {
            static bool Prefix(RandomMapObject __0)
            {
                if (LobbyPatchFlags.IsHosting)
                    return true;

                MelonLogger.Msg($"Spawning group ({__0.prefabs.Count}) prefabs:");
                foreach (var prefab in __0.prefabs)
                    MelonLogger.Msg("  - " + prefab.name);

                return false;
            }
        }
    }
}
