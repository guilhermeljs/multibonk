using UnityEngine;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Lobby;
using Multibonk.Game.World;

namespace Multibonk.Networking.Comms.Server.Handlers
{
    internal class PlayerAnimatorPacketHandler: IServerPacketHandler
    {
        private readonly LobbyContext _lobbyContext;
        private readonly GameWorld _world;

        public PlayerAnimatorPacketHandler(LobbyContext lobbyContext, GameWorld world)
        {
            _lobbyContext = lobbyContext;
            _world = world;
        }

        public byte PacketId => (byte)ClientSentPacketId.PLAYER_ANIMATOR_PACKET;

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new PlayerAnimatorPacket(msg);

            var playerId = _lobbyContext.GetPlayer(conn).UUID;

            GameDispatcher.Enqueue(() =>
            {
                var go = _world.CurrentSession?.PlayerManager.GetPlayer(playerId);

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

            foreach (var player in _lobbyContext.GetPlayers())
            {
                if (player.Connection == null || player.UUID == playerId)
                    continue;

                var sendPacket = new SendPlayerAnimatorChangedPacket(
                    playerId,
                    packet.AnimationId,
                    packet.Active
                );

                player.Connection.EnqueuePacket(sendPacket);
            }
        }
    }
}