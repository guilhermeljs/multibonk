using Il2CppAssets.Scripts.Actors.Player;
using Il2CppRewired.Utils;
using UnityEngine;

namespace Multibonk.Game.Handlers.Logic
{
    public class PlayerMoveEventTrigger : GameEventHandler
    {
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        private Vector3 lastScale;

        public override void LateUpdate()
        {
            var player = MyPlayer.Instance;
            if (
                player == null || 
                player.gameObject.IsNullOrDestroyed() || 
                player.playerRenderer == null || 
                player.playerRenderer.gameObject == null || 
                player.playerRenderer.gameObject.IsNullOrDestroyed()
                ) 
                return;

            var t = player.transform;
            if (Vector3.Distance(t.position, lastPosition) >= 0.1f)
            {
                lastPosition = t.position;
                GamePatchEvents.TriggerPlayerMoved(t.position);
            }


            var renderer = player.playerRenderer.transform;
            if (Quaternion.Angle(renderer.rotation, lastRotation) >= 15f)
            {
                lastRotation = renderer.rotation;
                GamePatchEvents.TriggerPlayerRotated(renderer.rotation);
            }

            if (Vector3.Distance(t.localScale, lastScale) >= 0.01f)
            {
                lastScale = t.localScale;
                // STILL NOT IMPLEMENTED
            }
        }
    }
}
