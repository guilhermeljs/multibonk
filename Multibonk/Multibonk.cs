using MelonLoader;
using Microsoft.Extensions.DependencyInjection;
using Multibonk.Game;
using Multibonk.Game.Handlers;
using Multibonk.Game.Handlers.Logic;
using Multibonk.Game.Handlers.NetworkNotify;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Client.Handlers;
using Multibonk.Networking.Comms.Client.Protocols;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;
using Multibonk.Networking.Comms.Server.Handlers;
using Multibonk.Networking.Comms.Server.Protocols;
using Multibonk.Networking.Lobby;
using Multibonk.UserInterface.Window;

namespace Multibonk
{
    public class MultibonkMod : MelonMod
    {
        UIManager manager;
        EventHandlerExecutor executor;

        public override void OnGUI()
        {
            if (manager != null)
                manager.OnGUI();

        }

        public override void OnUpdate()
        {
            executor.Update();
        }

        public override void OnFixedUpdate()
        {
            executor.FixedUpdate();
        }

        public override void OnLateUpdate()
        {
            executor.LateUpdate();
        }


        public override void OnInitializeMelon()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IGameEventHandler, CharacterChangedEventHandler>();
            services.AddSingleton<IGameEventHandler, GameLoadedEventHandler>();
            services.AddSingleton<IGameEventHandler, PlayerMovementEventHandler>();
            services.AddSingleton<IGameEventHandler, StartGameEventHandler>();
            services.AddSingleton<IGameEventHandler, UpdateNetworkPlayerAnimationsEventHandler>();
            services.AddSingleton<IGameEventHandler, GameDispatcher>();

            services.AddSingleton<EventHandlerExecutor>();

            services.AddSingleton<IServerPacketHandler, JoinLobbyPacketHandler>();
            services.AddSingleton<IServerPacketHandler, SelectCharacterPacketHandler>();
            services.AddSingleton<IServerPacketHandler, PlayerMovePacketHandler>();
            services.AddSingleton<IServerPacketHandler, PlayerRotatePacketHandler>();
            services.AddSingleton<IServerPacketHandler, GameLoadedPacketHandler>();

            services.AddSingleton<IClientPacketHandler, LobbyPlayerListPacketHandler>();
            services.AddSingleton<IClientPacketHandler, PlayerSelectedCharacterPacketHandler>();
            services.AddSingleton<IClientPacketHandler, SpawnPlayerPacketHandler>();
            services.AddSingleton<IClientPacketHandler, StartGamePacketHandler>();
            services.AddSingleton<IClientPacketHandler, PlayerMovedPacketHandler>();
            services.AddSingleton<IClientPacketHandler, PlayerRotatedPacketHandler>();

            services.AddSingleton<ClientProtocol>();
            services.AddSingleton<ServerProtocol>();

            services.AddSingleton<LobbyContext>();

            // Packet Handlers cannot call services. Otherwise, it will cause circular dependency
            services.AddSingleton<NetworkService>();
            services.AddSingleton<LobbyService>();

            services.AddSingleton<ClientLobbyWindow>();
            services.AddSingleton<ConnectionWindow>();
            services.AddSingleton<HostLobbyWindow>();

            services.AddSingleton<UIManager>();

            var serviceProvider = services.BuildServiceProvider();

            manager = serviceProvider.GetService<UIManager>();
            executor = serviceProvider.GetService<EventHandlerExecutor>();

            var _lobbyContext = serviceProvider.GetService<LobbyContext>();

            base.OnInitializeMelon();
        }
    }


}
