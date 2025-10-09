using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public enum PlayerAnimationId : byte
    {
        Moving = 0,
        Grounded = 1,
        Jumping = 2,
        Grinding = 3
    }

    public class SendPlayerAnimatorPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ClientSentPacketId.PLAYER_ANIMATOR_PACKET;

        public static PlayerAnimationId? StringToAnimationId(string param)
        {
            return param?.ToLower() switch
            {
                "moving" => PlayerAnimationId.Moving,
                "grounded" => PlayerAnimationId.Grounded,
                "jumping" => PlayerAnimationId.Jumping,
                "grinding" => PlayerAnimationId.Grinding,
                _ => null
            };
        }

        public static string AnimationIdToString(PlayerAnimationId animationId)
        {
            return animationId switch
            {
                PlayerAnimationId.Moving => "moving",
                PlayerAnimationId.Grounded => "grounded",
                PlayerAnimationId.Jumping => "jumping",
                PlayerAnimationId.Grinding => "grinding",
                _ => null
            };
        }

        public SendPlayerAnimatorPacket(PlayerAnimationId animationId, bool active)
        {
            Message.WriteByte(Id);
            Message.WriteByte((byte)animationId);
            Message.WriteByte((byte)(active ? 1 : 0));
        }
    }

    internal class PlayerAnimatorPacket
    {
        public PlayerAnimationId AnimationId { get; private set; }
        public bool Active { get; private set; }

        public PlayerAnimatorPacket(IncomingMessage msg)
        {
            AnimationId = (PlayerAnimationId)msg.ReadByte();
            Active = msg.ReadByte() != 0;
        }
    }
}