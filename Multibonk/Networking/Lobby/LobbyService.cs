using System;
using MelonLoader;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;
using Multibonk.Networking.Steam;

namespace Multibonk.Networking.Lobby
{
    public class LobbyService
    {
        private NetworkService NetworkService { get; }
        private LobbyContext CurrentLobby { get; }
        private SteamTunnelService SteamTunnelService { get; }

        public LobbyService(NetworkService service, LobbyContext context, SteamTunnelService steamTunnelService)
        {
            NetworkService = service;
            CurrentLobby = context;
            SteamTunnelService = steamTunnelService;
        }

        public void CreateLobby(string myName, int port)
        {
            MelonLogger.Msg($"Creating lobby on port {port}");
            try
            {
                NetworkService.StartServer(port);
            }
            catch (Exception e)
            {
                MelonLogger.Msg($"Failed to start lobby {e.Message}");
                CurrentLobby.TriggerLobbyJoinFailed($"Failed to start lobby: {e.Message}");
                return;
            }

            SteamTunnelService.ClearEndpoints();

            CurrentLobby.GetPlayers().Clear();
            CurrentLobby.SetMyself(new LobbyPlayer(name: myName));
            CurrentLobby.SetState(LobbyState.Hosting);
            CurrentLobby.TriggerLobbyCreated();
            LobbyPatchFlags.IsHosting = true;
        }

        public void JoinLobby(string ip, int port, string myName)
        {
            if (SteamTunnelService.TryConsumeEndpoint(out var endpoint))
            {
                ip = endpoint.Address;
                port = endpoint.Port;
                MelonLogger.Msg($"Using Steam tunnel endpoint {endpoint}.");
            }

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
                SteamTunnelService.ClearEndpoints();
                CurrentLobby.TriggerLobbyClosed();
                CurrentLobby.GetPlayers().Clear();
                CurrentLobby.SetState(LobbyState.None);
                LobbyPatchFlags.IsHosting = false;
            }
        }
    }
}
