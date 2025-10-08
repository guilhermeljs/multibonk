namespace Multibonk.Networking.Comms
{
    using System;
    using global::Multibonk.Networking.Comms.Base;
    using global::Multibonk.Networking.Comms.Client;
    using global::Multibonk.Networking.Comms.Client.Protocols;
    using global::Multibonk.Networking.Comms.Server;
    using global::Multibonk.Networking.Comms.Server.Protocols;

    namespace Multibonk.Networking.Comms
    {
        public enum NetworkState
        {
            None,
            Hosting,
            Connected
        }

        public class NetworkService
        {
            public NetworkState State { get; private set; } = NetworkState.None;

            private readonly ServerService server;
            private readonly ClientService client;
            public NetworkService(ServerProtocol serverPrt, ClientProtocol clientPrt)
            {
                server = new ServerService(25565, serverPrt);
                client = new ClientService(clientPrt);
            }

            public void Disconnect()
            {
                switch (State)
                {
                    case NetworkState.Connected:
                        client.Disconnect();
                        State = NetworkState.None;
                        break;
                    case NetworkState.Hosting:
                        server.Stop();
                        State = NetworkState.None;
                        break;
                    default:
                        throw new InvalidOperationException("Network not started.");
                }
            }


            public void StartServer()
            {
                if (State != NetworkState.None)
                    throw new InvalidOperationException("Network already started.");

                server.Start();
                State = NetworkState.Hosting;
            }

            public void StartClient(string ip, int port)
            {
                if (State != NetworkState.None)
                    throw new InvalidOperationException("Network already started.");

                client.SetIp(ip, port);
                client.Connect();
                State = NetworkState.Connected;
            }

            public IExposableClientService GetClientService()
            {
                return client;
            }
        }

        public class ServerService
        {
            private readonly Listener listener;

            public ServerService(int port, ServerProtocol protocol)
            {
                listener = new Listener(port, protocol);
            }

            public void Start() => listener.Start();
            public void Stop() => listener.Stop();
        }

        public interface IExposableClientService 
        {
            void Enqueue(OutgoingPacket packet);
        }


        public class ClientService : IExposableClientService
        {
            private readonly NetworkClient client;
            private string ip;
            private int port;

            public ClientService(ClientProtocol protocol)
            {
                client = new NetworkClient(protocol);
            }

            public void SetIp(string ip, int port)
            {
                this.ip = ip;
                this.port = port;
            }

            public void Connect() => client.Connect(ip, port);
            public void Disconnect() => client.Disconnect();
            public void Enqueue(OutgoingPacket packet) => client.GetConnection().EnqueuePacket(packet);
        }
    }
}
