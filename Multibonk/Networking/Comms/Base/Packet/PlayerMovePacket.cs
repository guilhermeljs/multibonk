using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using UnityEngine;

namespace Multibonk.Networking.Comms.Base.Packet
{

    namespace Multibonk.Networking.Comms.Base.Packet
    {
        // TODO: Reduce this packet size.
        public class SendPlayerMovePacket : OutgoingPacket
        {
            public readonly byte Id = (byte)ClientSentPacketId.PLAYER_MOVE_PACKET;

            public SendPlayerMovePacket(Vector3 position)
            {
                Message.WriteByte(Id);
                Message.WriteFloat(position.x);
                Message.WriteFloat(position.y);
                Message.WriteFloat(position.z);
            }
        }

        internal class PlayerMovePacket
        {
            public Vector3 Position { get; private set; }

            public PlayerMovePacket(IncomingMessage msg)
            {
                Position = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
            }
        }
    }
}
