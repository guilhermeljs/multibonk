using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using UnityEngine;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendPlayerRotatedPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.PLAYER_ROTATED_PACKET;

        public SendPlayerRotatedPacket(ushort playerId, Vector3 eulerAngles)
        {
            Message.WriteByte(Id);
            Message.WriteUShort(playerId);

            Message.WriteByte((byte)(eulerAngles.x / 360f * 255f));
            Message.WriteByte((byte)(eulerAngles.y / 360f * 255f));
            Message.WriteByte((byte)(eulerAngles.z / 360f * 255f));
        }
    }

    internal class PlayerRotatedPacket
    {
        public ushort PlayerId { get; private set; }
        public Vector3 EulerAngles { get; private set; }

        public PlayerRotatedPacket(IncomingMessage msg)
        {
            PlayerId = msg.ReadUShort();

            EulerAngles = new Vector3(
                msg.ReadByte() / 255f * 360f,
                msg.ReadByte() / 255f * 360f,
                msg.ReadByte() / 255f * 360f
            );
        }
    }
}
