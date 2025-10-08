using Il2Cpp;
using Il2CppAssets.Scripts.Actors.Player;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Lobby;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    public class GameLoadedEventHandler : GameEventHandler
    {
        public GameLoadedEventHandler(LobbyContext lobbyContext)
        {

            GameEvents.GameLoadedEvent += () =>
            {
                if (!LobbyPatchFlags.IsHosting)
                    return;

                lobbyContext.GetPlayers()
                .Where(player => player.Connection != null)
                .ToList()
                .ForEach(player =>
                {
                    var character = Enum.Parse<ECharacter>(player.SelectedCharacter);
                    var data = GamePatchFlags.CharacterData.Find(d => d.eCharacter == character);
                    GameFunctions.SpawnNetworkPlayer(player.UUID, character, MyPlayer.Instance.transform.position, MyPlayer.Instance.transform.rotation);

                    lobbyContext.GetPlayers()
                        .Where(target => target != player)
                        .ToList()
                        .ForEach(target =>
                        {
                            var character = Enum.Parse<ECharacter>(target.SelectedCharacter);
                            var packet = new SendSpawnPlayerPacket(character, target.UUID, MyPlayer.Instance.transform.position, MyPlayer.Instance.transform.rotation);
                            player.Connection.EnqueuePacket(packet);
                        });
                });
            };
        }

    }
}
