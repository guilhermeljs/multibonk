using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using UnityEngine;
using Multibonk.Game.World;

namespace Multibonk.Networking.Comms.Client.Handlers
{
    public class MapObjectChunkPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.MAP_OBJECT_CHUNK_PACKET;

        private readonly GameWorld _world;

        public MapObjectChunkPacketHandler(GameWorld world)
        {
            _world = world;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new MapObjectChunkPacket(msg);

            GameDispatcher.Enqueue(() =>
            {
                var session = _world.CurrentSession;
                if (session == null) return;

                var mapManager = session.MapManager;
                var prefabId = packet.ChunkId;

                foreach (var netObj in packet.Objects)
                {
                    Vector3 position = netObj.Position;
                    Quaternion rotation = netObj.Rotation;
                    Vector3 scale = netObj.LocalScale;

                    mapManager.SpawnMapObject(prefabId, position, rotation, scale);
                }
            });
        }
    }
}
