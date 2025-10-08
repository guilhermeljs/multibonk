namespace Multibonk.Networking.Comms.Base
{
    public interface IServerPacketHandler : IPacketHandler
    {
        public byte PacketId { get; }
    }
}
