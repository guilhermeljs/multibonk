using UnityEngine;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendPlayerMovedPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.PLAYER_MOVED_PACKET;

        public SendPlayerMovedPacket(byte playerId, Vector3 position)
        {
            Message.WriteByte(Id);
            Message.WritePlayerId(playerId);
            Message.WriteFloat(position.x);
            Message.WriteFloat(position.y);
            Message.WriteFloat(position.z);
        }
    }

    internal class PlayerMovedPacket
    {
        public byte PlayerId { get; private set; }
        public Vector3 Position { get; private set; }

        public PlayerMovedPacket(IncomingMessage msg)
        {
            PlayerId = msg.ReadPlayerId();
            Position = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        }
    }
}