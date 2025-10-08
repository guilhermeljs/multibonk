using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class MapFinishedLoadingPacket
    {
        public MapFinishedLoadingPacket(IncomingMessage msg) { }
    }

    public class SendMapFinishedLoadingPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.MAP_FINISHED_LOADING;

        public SendMapFinishedLoadingPacket()
        {
            Message.WriteByte(Id);
        }
    }
}
