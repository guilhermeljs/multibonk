using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using UnityEngine;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendPlayerRotatePacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ClientSentPacketId.PLAYER_ROTATE_PACKET;

        public SendPlayerRotatePacket(Quaternion rotation)
        {
            Message.WriteByte(Id);

            // Converter quaternion para Euler angles em Vector3
            Vector3 euler = rotation.eulerAngles;
            Message.WriteFloat(euler.x);
            Message.WriteFloat(euler.y);
            Message.WriteFloat(euler.z);
        }
    }

    internal class PlayerRotatePacket
    {
        public Quaternion Rotation { get; private set; }

        public PlayerRotatePacket(IncomingMessage msg)
        {
            Vector3 euler = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
            Rotation = Quaternion.Euler(euler);
        }
    }
}
