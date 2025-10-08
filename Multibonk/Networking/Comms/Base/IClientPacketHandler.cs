namespace Multibonk.Networking.Comms.Base
{
    public interface IClientPacketHandler : IPacketHandler
    {
        public byte PacketId { get; }
    }
}
