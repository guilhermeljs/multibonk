using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendKillEnemyPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ClientSentPacketId.KILL_ENEMY_PACKET;

        public SendKillEnemyPacket(uint enemyId)
        {
            Message.WriteByte(Id);  
            Message.WriteUInt(enemyId);
        }
    }

    internal class KillEnemyPacket
    {
        public uint EnemyId { get; private set; }

        public KillEnemyPacket(IncomingMessage msg)
        {
            EnemyId = msg.ReadUInt();
        }
    }
}
