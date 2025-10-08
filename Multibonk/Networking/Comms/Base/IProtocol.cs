using System.Net.Sockets;

namespace Multibonk.Networking.Comms.Base
{
    public interface IProtocol
    {
        void HandleMessage(Connection c, byte[] data, int start, int length);
    }
}
