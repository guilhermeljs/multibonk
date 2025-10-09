

using System.Runtime.CompilerServices;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Actors.Player;
using Il2CppAssets.Scripts.Game.MapGeneration;
using Il2CppAssets.Scripts.Inventory__Items__Pickups;
using Il2CppInventory__Items__Pickups.Xp_and_Levels;
using Il2CppRewired.Utils;
using Il2CppTMPro;
using MelonLoader;
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

                    GameEvents.TriggerConfirmCharacter();
                }

                //return false;
                return true;
            }
        }



        [HarmonyPatch(typeof(MyPlayer), "FixedUpdate")]
        class PlayerUpdatedPatch
        {
            static void Postfix()
            {
                var myPlayer = MyPlayer.Instance;
                if (myPlayer == null) return;
                if (myPlayer.gameObject.IsNullOrDestroyed()) return;

                const float positionThreshold = 0.05f;
                const float rotationThreshold = 10f;

                if ((myPlayer.transform.position - GamePatchFlags.LastPlayerPosition).sqrMagnitude > positionThreshold * positionThreshold)
                {
                    GamePatchFlags.LastPlayerPosition = myPlayer.transform.position;
                    GameEvents.TriggerPlayerMoved(myPlayer.transform.position);
                }

                var rotation = myPlayer.playerRenderer.transform.rotation;

                if (Quaternion.Angle(rotation, GamePatchFlags.LastPlayerRotation) > rotationThreshold)
                {
                    GamePatchFlags.LastPlayerRotation = rotation;
                    GameEvents.TriggerPlayerRotated(rotation);
                }
            }
        }

        [HarmonyPatch(typeof(MapEntry), "OnMapSelected")]
        class MapEntrySelectedPatch
        {
            static void Postfix(SelectionGroupToggleSingleButton __0, MapData __1)
            {
                GamePatchFlags.SelectedMapData = __1;
            }
        }

        [HarmonyPatch(typeof(MyPlayer), "Start")]
        class GameStartedPatch
        {
            static void Postfix()
            {
                GameEvents.TriggerGameLoadedEvent();
            }
        }


        [HarmonyPatch(typeof(MapSelectionUi), "StartMap")]
        class ConfirmMapPatch
        {
            static bool Prefix()
            {
                if (!LobbyPatchFlags.IsHosting && !GamePatchFlags.AllowStartMapCall)
                    return false;

                GameEvents.TriggerConfirmMap();

                return true;
            }
        }

        [HarmonyPatch(typeof(CharacterInfoUI), "OnCharacterSelected")]
        class CharacterSelectionPatch
        {
            static void Postfix(MyButtonCharacter btn)
            {
                if (btn != null)
                {
                    GameEvents.TriggerCharacterChanged(btn.characterData);
                }
            }
        }


        [HarmonyPatch(typeof(ProceduralTileGeneration), "Generate")]
        class MapGenerationPatch
        {
            static void Prefix(Il2Cpp.ProceduralTileGeneration __instance, UnityEngine.Vector3 __result, ref UnityEngine.Vector3 __0, Il2Cpp.StageData __1, Il2CppAssets.Scripts.MapGeneration.ProceduralTiles.MapParameters __2, ref bool __3)
            {
                MelonLogger.Msg("Patching debug seed");
                __instance.debugSeed = GamePatchFlags.Seed;
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

                GameEvents.TriggerAddXpEvent(__0);

                return true;
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
                if(LobbyPatchFlags.IsHosting)
                    return true;

                MelonLogger.Msg("Generate called - prefabs");

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
