using UnityEngine;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SpawnEnemyPacket
    {
        public uint EnemyUUID { get; set; }
        public int EnemyIndex { get; private set; }
        public Vector3 Position { get; private set; }

        public SpawnEnemyPacket(IncomingMessage msg)
        {
            EnemyUUID = msg.ReadUInt();
            EnemyIndex = msg.ReadInt();
            Position = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        }
    }

    public class SendSpawnEnemyPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ClientSentPacketId.SPAWN_ENEMY_PACKET;

        public SendSpawnEnemyPacket(uint enemyID, int enemyPrefabIndex, Vector3 position)
        {

            Message.WriteByte(Id);
            Message.WriteUInt(enemyID);
            Message.WriteInt(enemyPrefabIndex);
            Message.WriteFloat(position.x);
            Message.WriteFloat(position.y);
            Message.WriteFloat(position.z);
        }
    }
}





