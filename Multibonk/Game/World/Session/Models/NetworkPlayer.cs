using Multibonk.Networking.Comms.Base.Packet;
using UnityEngine;

namespace Multibonk.Game.World.Session.Models
{
    public class NetworkPlayer
    {
        public GameObject PlayerObject { get; private set; }
        public float LastSeenTime { get; private set; }
        public float LastTimeMoved { get; private set; }

        public NetworkPlayer(GameObject playerObject)
        {
            PlayerObject = playerObject;
            LastSeenTime = Time.time;
            LastTimeMoved = Time.time;
        }

        public bool IsNullOrDestroyed()
        {
            return PlayerObject == null || PlayerObject.Equals(null);
        }

        /// <summary>
        /// Updates the Animator state based on a PlayerAnimationId and boolean value.
        /// </summary>
        public void SetAnimation(PlayerAnimationId animationId, bool active)
        {
            if (PlayerObject == null) return;

            var animator = PlayerObject.GetComponentInChildren<Animator>();
            if (animator == null) return;

            switch (animationId)
            {
                case PlayerAnimationId.Moving:
                    animator.SetBool("moving", active);
                    break;
                case PlayerAnimationId.Grounded:
                    animator.SetBool("grounded", active);
                    break;
                case PlayerAnimationId.Jumping:
                    animator.SetBool("jumping", active);
                    break;
                case PlayerAnimationId.Grinding:
                    animator.SetBool("grinding", active);
                    break;
            }
        }

        public void Move(Vector3 position)
        {
            PlayerObject.transform.position = position;
            LastTimeMoved = Time.time;
        }

        public void Rotate(Vector3 rotation)
        {
            PlayerObject.transform.rotation = Quaternion.Euler(rotation);
        }
    }

}
