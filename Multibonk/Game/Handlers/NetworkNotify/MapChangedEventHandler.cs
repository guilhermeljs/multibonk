using MelonLoader;
using Multibonk.Game.World.Session.Managers;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    public class MapChangedEventHandler : GameEventHandler
    {
        public MapChangedEventHandler(
            NetworkService network,
            LobbyContext lobbyContext
        )
        {
            GamePatchEvents.MapChanged += (map) =>
            {
                MelonLogger.Msg("Choosing map " + map.name);
                MapManager.SelectedMapData = map;
            };
        }
    }
}
