using Multibonk.Game;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using UnityEngine;
using Multibonk.Game.Handlers;

namespace Multibonk.Networking.Comms.Client.Handlers
{
    public class SpawnPlayerPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.SPAWN_PLAYER_PACKET;

        public SpawnPlayerPacketHandler() { }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new SpawnPlayerPacket(msg);

            GameDispatcher.Enqueue(() =>
            {   
                GameFunctions.SpawnNetworkPlayer(packet.PlayerId, packet.Character, packet.Position, packet.Rotation);
            });
        }
    }
}
