using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Base.Packet
{
    public class SendEnemyDeathPacket : OutgoingPacket
    {
        public readonly byte Id = (byte)ServerSentPacketId.ENEMY_DEATH_PACKET;

        public SendEnemyDeathPacket(uint enemyId)
        {
            Message.WriteByte(Id);
            Message.WriteUInt(enemyId);
        }
    }

    internal class EnemyDeathPacket
    {
        public uint EnemyId { get; private set; }

        public EnemyDeathPacket(IncomingMessage msg)
        {
            EnemyId = msg.ReadUInt();
        }
    }
}
