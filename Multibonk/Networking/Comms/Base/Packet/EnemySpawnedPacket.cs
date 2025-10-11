using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using UnityEngine;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class EnemySpawnedPacket
    {
        public uint EnemyId { get; private set; }
        public int EnemyIndex { get; private set; }
        public Vector3 Position { get; private set; }
        public byte TargetPlayerId { get; private set; }

        public EnemySpawnedPacket(IncomingMessage msg)
        {
            EnemyId = msg.ReadUInt();
            EnemyIndex = msg.ReadInt();
            Position = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
            TargetPlayerId = msg.ReadPlayerId();
        }
    }

    public class SendEnemySpawnedPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.ENEMY_SPAWNED_PACKET;

        public SendEnemySpawnedPacket(uint enemyId, int enemyPrefabIndex, Vector3 position, byte targetPlayerId)
        {
            Message.WriteByte(Id);
            Message.WriteUInt(enemyId);
            Message.WriteInt(enemyPrefabIndex);
            Message.WriteFloat(position.x);
            Message.WriteFloat(position.y);
            Message.WriteFloat(position.z);
            Message.WriteByte(targetPlayerId);
        }
    }
}