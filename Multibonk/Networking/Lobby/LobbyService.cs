using MelonLoader;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;

namespace Multibonk.Networking.Lobby
{
    public class LobbyService
    {
        private NetworkService NetworkService { get; }
        private LobbyContext CurrentLobby { get; }

        public LobbyService(NetworkService service, LobbyContext context)
        {
            NetworkService = service;
            CurrentLobby = context;
        }

        public void CreateLobby(string myName)
        {
            MelonLogger.Msg($"Creating lobby");
            try
            {
                NetworkService.StartServer();
            }
            catch (Exception e)
            {
                MelonLogger.Msg($"Failed to start lobby {e.Message}");
                CurrentLobby.TriggerLobbyJoinFailed($"Failed to start lobby {e.Message}");
                return;
            }

            CurrentLobby.GetPlayers().Clear();
            CurrentLobby.SetMyself(new LobbyPlayer(name: myName));
            CurrentLobby.SetState(LobbyState.Hosting);
            CurrentLobby.TriggerLobbyCreated();
            LobbyPatchFlags.IsHosting = true;
        }

        public void JoinLobby(string ip, int port, string myName)
        {
            MelonLogger.Msg($"Joining lobby {ip}:{port} with the username: {myName}");

            try
            {
                var joinGamePacket = new SendJoinLobbyPacket(100, myName);
                NetworkService.GetClientService().Enqueue(joinGamePacket);
                NetworkService.StartClient(ip, port);
            }
            catch (Exception e)
            {
                CurrentLobby.TriggerLobbyJoinFailed($"Couldn't connect to lobby {e.Message}");
                return;
            }

            CurrentLobby.GetPlayers().Clear();
            CurrentLobby.SetState(LobbyState.Connected);
            CurrentLobby.TriggerLobbyJoin();
            LobbyPatchFlags.IsHosting = false;
        }

        public void AddPlayer(string playerName)
        {
            CurrentLobby.AddPlayer(playerName);
        }

        public void RemovePlayer(Guid uuid)
        {
            CurrentLobby.RemovePlayer(uuid);
        }

        public void CloseLobby()
        {
            try
            {
                NetworkService.Disconnect();
            }
            catch (Exception e)
            {
                MelonLogger.Msg($"NetworkService failed to disconnect {e}");
                return;
            }
            finally
            {
                CurrentLobby.TriggerLobbyClosed();
                CurrentLobby.GetPlayers().Clear();
                CurrentLobby.SetState(LobbyState.None);
                LobbyPatchFlags.IsHosting = false;
            }

        }
    }

}
