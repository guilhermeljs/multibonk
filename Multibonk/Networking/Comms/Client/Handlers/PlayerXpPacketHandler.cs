using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Game.Handlers;
using Multibonk.Game;
using Il2Cpp;

namespace Multibonk.Networking.Comms.Server.Handlers
{
    /// <summary>
    /// Handles incoming XP packets from clients.
    /// </summary>
    public class PlayerXpPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.XP_PACKET;

        public PlayerXpPacketHandler()
        {}

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new XpPacket(msg);

            GameDispatcher.Enqueue(() =>
            {
                try
                {
                    GamePatchFlags.AllowAddXpCall = true;
                    var inventory = GameManager.Instance.GetPlayerInventory();
                    inventory.playerXp.AddXp(packet.Xp);
                }
                finally
                {
                    GamePatchFlags.AllowAddXpCall = false;
                }
            });
        }
    }
}






