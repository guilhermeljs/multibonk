using Multibonk.Game;
using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Client.Handlers
{

    public class PlayerMovedPacketHandler : IClientPacketHandler
    {

        public byte PacketId => (byte)ServerSentPacketId.PLAYER_MOVED_PACKET;

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new PlayerMovedPacket(msg);

            GameDispatcher.Enqueue(() =>
            {
                var go = GameFunctions.GetSpawnedPlayerFromId(packet.PlayerId);

                if (go != null)
                {
                    go.Move(packet.Position);
                }
            });
        }
    }
}

