using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base
{
    public interface IPacketHandler
    {
        void Handle(IncomingMessage msg, Connection conn);
    }
}
