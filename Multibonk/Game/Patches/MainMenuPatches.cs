using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Actors.Player;
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


        //[HarmonyPatch(typeof(SpawnInteractables), "SpawnChests")]
        //class ChestPatch
        //{
        //    static bool Prefix()
        //    {
        //        return false;
        //    }
        //}

        //[HarmonyPatch(typeof(SpawnInteractables), "SpawnOther")]
        //class SpawnOtherPatch
        //{
        //    static bool Prefix() => false;
        //}

        //[HarmonyPatch(typeof(SpawnInteractables), "SpawnRails")]
        //class SpawnRailsPatch
        //{
        //    static bool Prefix() => false;
        //}

        //[HarmonyPatch(typeof(SpawnInteractables), "SpawnShit")]
        //class SpawnShitPatch
        //{
        //    static bool Prefix() => false;
        //}

        //[HarmonyPatch(typeof(SpawnInteractables), "SpawnShrines")]
        //class SpawnShrinesPatch
        //{
        //    static bool Prefix() => false;
        //}







    }
}
