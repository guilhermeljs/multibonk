using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Game;
using UnityEngine;

namespace Multibonk.Networking.Comms.Client.Handlers
{
    public class MapObjectChunkPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.MAP_OBJECT_CHUNK_PACKET;

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new MapObjectChunkPacket(msg);

            GameDispatcher.Enqueue(() =>
            {
                var prefabs = GamePatchFlags.MapDataIndexedPrefabs;
                var prefab = prefabs[packet.ChunkId];

                foreach (var netObj in packet.Objects)
                {
                    Vector3 position = netObj.Position;
                    Quaternion rotation = netObj.Rotation;
                    Vector3 scale = netObj.LocalScale;

                    var obj = UnityEngine.Object.Instantiate(prefab, position, rotation);
                    obj.transform.localScale = scale;
                }
            });
        }
    }
}
