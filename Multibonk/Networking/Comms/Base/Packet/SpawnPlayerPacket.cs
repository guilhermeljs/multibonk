
using Il2Cpp;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using UnityEngine;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendSpawnPlayerPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.SPAWN_PLAYER_PACKET;

        public SendSpawnPlayerPacket(ECharacter character, byte playerId, Vector3 position, Quaternion rotation)
        {
            Message.WriteByte(Id);
            Message.WriteByte((byte)character);
            Message.WritePlayerId(playerId);

            Message.WriteFloat(position.x);
            Message.WriteFloat(position.y);
            Message.WriteFloat(position.z);

            Message.WriteFloat(rotation.x);
            Message.WriteFloat(rotation.y);
            Message.WriteFloat(rotation.z);
            Message.WriteFloat(rotation.w);
        }
    }

    internal class SpawnPlayerPacket
    {
        public ECharacter Character { get; private set; }
        public byte PlayerId { get; private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }

        public SpawnPlayerPacket(IncomingMessage msg)
        {
            Character = (ECharacter)msg.ReadByte();
            PlayerId = msg.ReadPlayerId();

            Position = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
            Rotation = new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        }
    }
}
