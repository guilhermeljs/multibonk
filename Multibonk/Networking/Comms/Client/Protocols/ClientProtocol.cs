using MelonLoader;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Client.Protocols
{
    public class ClientProtocol : IClientProtocol
    {
        private readonly Dictionary<byte, IClientPacketHandler> handlers;

        public event Action<Connection> OnConnected;
        public event Action<Connection> OnDisconnected;

        public ClientProtocol(
            IEnumerable<IClientPacketHandler> clientHandlers
        )
        {
            handlers = clientHandlers.ToDictionary(h => h.PacketId);
        }


        public void HandleMessage(Connection c, byte[] data, int start, int length)
        {
            if (length < 1) return;

            byte packetId = data[0];
            if (handlers.TryGetValue(packetId, out var handler))
            {
                var msg = new IncomingMessage(data);
                msg.ReadByte();

                MelonLogger.Msg($"Handling packet with ID {packetId}");
                handler.Handle(msg, c);
            }
            else
            {
                Console.WriteLine($"No handler registered for packet ID {packetId}");
            }
        }

        public void OnConnect(Connection c)
        {
            OnConnected?.Invoke(c);
        }

        public void OnDisconnect(Connection c)
        {
            OnDisconnected?.Invoke(c);
        }
    }
}
