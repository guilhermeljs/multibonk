using System.Net;
using System.Net.Sockets;
using Multibonk.Networking.Comms.Base;

namespace Multibonk.Networking.Comms.Server
{
    public class Listener
    {
        private readonly IServerProtocol protocol;

        private TcpListener tcpListener;
        private bool running = false;
        private int port;

        public Listener(int port, IServerProtocol protocol)
        {
            this.port = port;
            this.protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        }

        public void InternalStart()
        {
            tcpListener = new TcpListener(IPAddress.Any, port);

            tcpListener.Start();


            protocol.ServerStarted();

            _ = Task.Run(AcceptLoop);
        }

        public void Start()
        {
            if (running) return;
            running = true;

            new Thread(() => {
                InternalStart();
            }).Start();
        }

        public void Stop()
        {
            if (!running)
                return;

            running = false;
            protocol.ServerClosed();
            tcpListener.Stop();
        }

        private Connection CreateConnection(TcpClient client)
        {
            var connection = new Connection(client);

            protocol.HandleConnect(connection);

            connection.OnMessageReceived += (conn, packet) => protocol.HandleMessage(conn, packet, 0, packet.Length);

            connection.OnClose += conn => protocol.HandleClose(conn);

            return connection;
        }

        private async Task AcceptLoop()
        {
            while (running)
            {
                try
                {
                    var client = await tcpListener.AcceptTcpClientAsync();
                    var connection = CreateConnection(client);

                    connection.Start();
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }
        }
    }
}