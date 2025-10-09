using Multibonk.Game;
using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using UnityEngine;

namespace Multibonk.Networking.Comms.Client.Handlers
{
    public class PlayerAnimatorChangedPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.PLAYER_ANIMATOR_CHANGED_PACKET;

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new PlayerAnimatorChangedPacket(msg);

            GameDispatcher.Enqueue(() =>
            {
                var go = GameFunctions.GetSpawnedPlayerFromId(packet.PlayerId);
                if (go != null)
                {
                    var animator = go.PlayerObject.GetComponentInChildren<Animator>();
                    if (animator != null)
                    {
                        switch (packet.AnimationId)
                        {
                            case PlayerAnimationId.Moving:
                                animator.SetBool("moving", packet.Active);
                                break;
                            case PlayerAnimationId.Grounded:
                                animator.SetBool("grounded", packet.Active);
                                break;
                            case PlayerAnimationId.Jumping:
                                animator.SetBool("jumping", packet.Active);
                                break;
                            case PlayerAnimationId.Grinding:
                                animator.SetBool("grinding", packet.Active);
                                break;
                        }
                    }
                }
            });
        }
    }
}