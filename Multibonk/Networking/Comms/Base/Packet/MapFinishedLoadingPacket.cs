using MelonLoader;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using UnityEngine;

namespace Multibonk.Networking.Comms.Base.Packet
{

    public class SendMapObjectChunkPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.MAP_OBJECT_CHUNK_PACKET;

        public SendMapObjectChunkPacket(int chunkId, List<GameObject> objects)
        {
            Message.WriteByte(Id);
            Message.WriteInt(chunkId);
            Message.WriteInt(objects.Count);

            foreach (var obj in objects)
            {
                Vector3 pos = obj.transform.position;
                Vector3 e = obj.transform.rotation.eulerAngles;
                Vector3 scale = obj.transform.localScale;

                Message.WriteFloat(pos.x);
                Message.WriteFloat(pos.y);
                Message.WriteFloat(pos.z);

                Message.WriteFloat(e.x);
                Message.WriteFloat(e.y);
                Message.WriteFloat(e.z);

                Message.WriteFloat(scale.x);
                Message.WriteFloat(scale.y);
                Message.WriteFloat(scale.z);
            }
        }
    }

    public class MapObjectChunkPacket
    {
        public int ChunkId { get; }
        public List<NetworkObjectPosition> Objects { get; }

        public MapObjectChunkPacket(IncomingMessage msg)
        {
            ChunkId = msg.ReadInt();
            int count = msg.ReadInt();
            Objects = new List<NetworkObjectPosition>(count);

            for (int i = 0; i < count; i++)
            {
                float x = msg.ReadFloat();
                float y = msg.ReadFloat();
                float z = msg.ReadFloat();

                float eulerX = msg.ReadFloat();
                float eulerY = msg.ReadFloat();
                float eulerZ = msg.ReadFloat();

                float scaleX = msg.ReadFloat();
                float scaleY = msg.ReadFloat();
                float scaleZ = msg.ReadFloat();

                var pos = new Vector3(x, y, z);
                var rot = Quaternion.Euler(eulerX, eulerY, eulerZ);
                var localPos = new Vector3(scaleX, scaleY, scaleZ);

                Objects.Add(new NetworkObjectPosition(pos, rot, localPos));
            }
        }
    }

    public class SendMapFinishedLoadingPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.MAP_FINISHED_LOADING;

        public SendMapFinishedLoadingPacket()
        {
            Message.WriteByte(Id);
        }
    }

    public class MapFinishedLoadingPacket
    {
        public MapFinishedLoadingPacket(IncomingMessage msg)
        {
        }
    }

    public struct NetworkObjectPosition
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Vector3 LocalScale { get; }

        public NetworkObjectPosition(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            LocalScale = scale;
        }
    }

}
