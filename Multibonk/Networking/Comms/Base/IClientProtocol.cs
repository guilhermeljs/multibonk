namespace Multibonk.Networking.Comms.Base
{
    public interface IClientProtocol : IProtocol
    {
        event Action<Connection> OnConnected;
        event Action<Connection> OnDisconnected;

        void OnConnect(Connection c);
        void OnDisconnect(Connection c);
    }
}
