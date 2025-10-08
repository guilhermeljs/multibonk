using Multibonk.Networking.Comms.Base;
using System.Net.Sockets;

namespace Multibonk.Networking.Comms.Client
{
    public class NetworkClient
    {
        private TcpClient tcpClient;
        private Connection connection;
        private IClientProtocol protocol;


        public bool IsConnected => tcpClient?.Connected ?? false;

        public NetworkClient(IClientProtocol protocol)
        {
            tcpClient = new TcpClient();
            this.protocol = protocol;

            connection = new Connection(tcpClient);
        }

        private void InternalConnect(string ip, int port)
        {
            tcpClient.Connect(ip, port);

            connection.Start();

            protocol.OnConnect(connection);
            connection.OnMessageReceived += (conn, packet) =>
            {
                protocol.HandleMessage(conn, packet, 0, packet.Length);
            };
            connection.OnClose += conn =>
            {
                protocol.OnDisconnect(conn);
            };
        }
        public void Connect(string ip, int port)
        {
            if (IsConnected) throw new InvalidOperationException("Client already connected.");

            new Thread(() =>
            {
                InternalConnect(ip, port);
            }).Start();
        }

        public Connection GetConnection()
        {
            return connection;
        }

        public void Disconnect()
        {
            connection?.Close();
            tcpClient = new TcpClient();
            connection = null;
        }

    }
}
