using MelonLoader;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    public class CharacterChangedEventHandler : GameEventHandler
    {
        public CharacterChangedEventHandler(
            NetworkService network,
            LobbyContext lobbyContext
        )
        {
            GameEvents.CharacterChanged += (c) =>
            {
                MelonLogger.Msg($"Choosing character {c.eCharacter.ToString()}");
                if (network.State == NetworkState.Connected)
                {
                    var characterSelection = new SendSelectCharacterPacket(c.eCharacter.ToString());
                    network.GetClientService().Enqueue(characterSelection);
                }

                if (network.State == NetworkState.Hosting)
                {
                    var v = lobbyContext.GetMyself().UUID;
                    lobbyContext.GetMyself().SelectedCharacter = c.eCharacter.ToString();

                    var characterSelection = new SendPlayerSelectedCharacterPacket(lobbyContext.GetMyself().UUID, c.eCharacter.ToString());


                    foreach (var player in lobbyContext.GetPlayers())
                    {
                        player.Connection?.EnqueuePacket(characterSelection);
                    }
                }
            };
        }
    }
}
