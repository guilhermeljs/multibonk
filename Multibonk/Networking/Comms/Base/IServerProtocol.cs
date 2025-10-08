namespace Multibonk.Networking.Comms.Base
{
    public interface IServerProtocol : IProtocol
    {
        event Action OnServerStarted;
        event Action OnServerStopped;
        event Action<Connection> OnClientConnected;
        event Action<Connection> OnClientDisconnected;

        void HandleClose(Connection connection);
        void HandleConnect(Connection connection);

        void ServerStarted();
        void ServerClosed();
    }
}
