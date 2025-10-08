using Il2Cpp;
using Il2CppRewired.Utils;
using UnityEngine;

namespace Multibonk.Game.Handlers.Logic
{
    public class UpdateNetworkPlayerAnimationsEventHandler : GameEventHandler
    {
        public UpdateNetworkPlayerAnimationsEventHandler()
        {

        }


        public override void FixedUpdate()
        {
            foreach (var player in GamePatchFlags.PlayersCache.Values)
            {
                if (player.PlayerObject.IsNullOrDestroyed())
                    continue;

                // Players with more than 300ms can appear lagged
                if (Time.time - player.LastTimeMoved > 0.3f)
                {
                    var renderer = player.PlayerObject.GetComponentInChildren<PlayerRenderer>();
                    if (renderer != null)
                        renderer.ForceMoving(false);
                }
            }
        }
    }
}
