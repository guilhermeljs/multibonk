using Multibonk.Networking.Lobby;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Game.Handlers;
using Multibonk.Game;
using Il2Cpp;
using Multibonk.Networking.Comms.Base.Packet;

namespace Multibonk.Networking.Comms.Server.Handlers
{
    public class PlayerPickupXpPacketHandler : IServerPacketHandler
    {
        public byte PacketId => (byte)ClientSentPacketId.PICKUP_XP_PACKET;
        private readonly LobbyContext _lobbyContext;

        public PlayerPickupXpPacketHandler(LobbyContext lobbyContext)
        {
            _lobbyContext = lobbyContext;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new PlayerPickupXpPacket(msg);

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


            var playerId = _lobbyContext.GetPlayer(conn).UUID;
            foreach (var player in _lobbyContext.GetPlayers())
            {
                if (player.Connection == null || player.UUID == playerId)
                    continue;

                var sendPacket = new SendXpPacket(packet.Xp);
                player.Connection.EnqueuePacket(sendPacket);
            }
        }
    }
}