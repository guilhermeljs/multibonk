using UnityEngine;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class PlayerAnimatorChangedPacket
    {
        public ushort PlayerId { get; private set; }
        public PlayerAnimationId AnimationId { get; private set; }
        public bool Active { get; private set; }

        public PlayerAnimatorChangedPacket(IncomingMessage msg)
        {
            PlayerId = msg.ReadUShort();
            AnimationId = (PlayerAnimationId)msg.ReadByte();
            Active = msg.ReadByte() != 0;
        }
    }

    public class SendPlayerAnimatorChangedPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.PLAYER_ANIMATOR_CHANGED_PACKET;

        public SendPlayerAnimatorChangedPacket(ushort playerId, PlayerAnimationId animationId, bool active)
        {
            Message.WriteByte(Id);
            Message.WriteUShort(playerId);
            Message.WriteByte((byte)animationId);
            Message.WriteByte((byte)(active ? 1 : 0));
        }
    }
}






