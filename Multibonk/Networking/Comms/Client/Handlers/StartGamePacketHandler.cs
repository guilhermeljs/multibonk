using Il2Cpp;
using MelonLoader;
using Multibonk.Game;
using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Comms.Client.Handlers
{
    public class StartGamePacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.START_GAME;

        public StartGamePacketHandler()
        {

        }


        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new StartGamePacket(msg);
            GamePatchFlags.Seed = packet.Seed;

            GameDispatcher.Enqueue(() =>
            {
                var ui = UnityEngine.Object.FindObjectOfType<MapSelectionUi>();
                if (ui != null)
                {
                    GamePatchFlags.AllowStartMapCall = true;
                    MelonLogger.Msg("Starting map..");
                    ui.StartMap();

                    MelonLogger.Msg("Map started succesfully..");
                    GamePatchFlags.AllowStartMapCall = false;
                }
            });
        }
    }
}
