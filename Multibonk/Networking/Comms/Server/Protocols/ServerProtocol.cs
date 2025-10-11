using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Server.Protocols
{
    public class ServerProtocol : IServerProtocol
    {
        private readonly Dictionary<byte, IServerPacketHandler> handlers;

        public event Action<Connection> OnClientConnected;
        public event Action<Connection> OnClientDisconnected;
        public event Action OnServerStarted;
        public event Action OnServerStopped;

        public ServerProtocol(
            IEnumerable<IServerPacketHandler> packetHandlers
        )
        {
           handlers = packetHandlers.ToDictionary(h => h.PacketId);
        }

        public void HandleMessage(Connection conn, byte[] data, int start, int length)
        {
            if (length < 1) return;

            byte packetId = data[0];
            if (handlers.TryGetValue(packetId, out var handler))
            {
                var msg = new IncomingMessage(data);
                msg.ReadByte();

                handler.Handle(msg, conn);
            }
            else
            {
                Console.WriteLine($"No handler registered for packet ID {packetId}.");
            }
        }

        public void HandleClose(Connection connection)
        {
            OnClientConnected?.Invoke(connection);
        }

        public void HandleConnect(Connection connection)
        {
            // this is synchronous being called in an async context.. maybe we can enhance performance
            OnClientConnected?.Invoke(connection);
        }

        public void ServerStarted()
        {
            OnServerStarted?.Invoke();
        }

        public void ServerClosed()
        {
            OnServerStopped?.Invoke();
        }
    }
}
