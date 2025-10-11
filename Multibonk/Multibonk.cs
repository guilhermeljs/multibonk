using MelonLoader;
using Multibonk.UserInterface.Window;
using Microsoft.Extensions.DependencyInjection;
using Multibonk.Networking.Lobby;
using Multibonk.Networking.Comms.Server.Protocols;
using Multibonk.Networking.Comms.Client.Protocols;
using Multibonk.Networking.Comms.Multibonk.Networking.Comms;
using Multibonk.Networking.Comms.Server.Handlers;
using Multibonk.Networking.Comms.Client.Handlers;
using Multibonk.Game.Handlers;
using Multibonk.Game;
using Multibonk.Networking.Comms.Base;
using Multibonk.Game.Handlers.NetworkNotify;
using Il2Cpp;
using Multibonk.Networking.Comms.Client.Handlers.Player;
using Multibonk.Game.World;
using Multibonk.Game.Handlers.Logic;

namespace Multibonk
{
    public class MultibonkMod : MelonMod
    {
        UIManager manager;
        EventHandlerExecutor executor;

        public override void OnGUI()
        {
            if(manager != null)
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

            services.AddSingleton<GameWorld>();

            services.AddSingleton<IGameEventHandler, CharacterChangedEventHandler>();
            services.AddSingleton<IGameEventHandler, GameLoadedEventHandler>();
            services.AddSingleton<IGameEventHandler, MapChangedEventHandler>();
            services.AddSingleton<IGameEventHandler, PlayerMovementEventHandler>();
            services.AddSingleton<IGameEventHandler, EnemySpawnEventHandler>();
            services.AddSingleton<IGameEventHandler, EnemyDeathEventHandler>();
            services.AddSingleton<IGameEventHandler, StartGameEventHandler>();
            services.AddSingleton<IGameEventHandler, PlayerMoveEventTrigger>();
            services.AddSingleton<IGameEventHandler, PlayerLevelEventHandler>();
            services.AddSingleton<IGameEventHandler, GamePauseEventHandler>();
            services.AddSingleton<IGameEventHandler, GameDispatcher>();

            services.AddSingleton<EventHandlerExecutor>();

            services.AddSingleton<IServerPacketHandler, JoinLobbyPacketHandler>();
            services.AddSingleton<IServerPacketHandler, SelectCharacterPacketHandler>();
            services.AddSingleton<IServerPacketHandler, PlayerMovePacketHandler>();
            services.AddSingleton<IServerPacketHandler, PlayerRotatePacketHandler>();
            services.AddSingleton<IServerPacketHandler, PlayerAnimatorPacketHandler>();
            services.AddSingleton<IServerPacketHandler, SpawnEnemyPacketHandler>();
            services.AddSingleton<IServerPacketHandler, GameLoadedPacketHandler>();
            services.AddSingleton<IServerPacketHandler, PauseGamePacketHandler>();
            services.AddSingleton<IServerPacketHandler, UnpauseGamePacketHandler>();
            services.AddSingleton<IServerPacketHandler, KillEnemyPacketHandler>();
            services.AddSingleton<IServerPacketHandler, PlayerPickupXpPacketHandler>();

            services.AddSingleton<IClientPacketHandler, LobbyPlayerListPacketHandler>();
            services.AddSingleton<IClientPacketHandler, PlayerSelectedCharacterPacketHandler>();
            services.AddSingleton<IClientPacketHandler, SpawnPlayerPacketHandler>();
            services.AddSingleton<IClientPacketHandler, PlayerAnimatorChangedPacketHandler>();
            services.AddSingleton<IClientPacketHandler, StartGamePacketHandler>();
            services.AddSingleton<IClientPacketHandler, PlayerXpPacketHandler>();
            services.AddSingleton<IClientPacketHandler, PlayerMovedPacketHandler>();
            services.AddSingleton<IClientPacketHandler, GamePausePacketHandler>();
            services.AddSingleton<IClientPacketHandler, GameUnpausePacketHandler>();
            services.AddSingleton<IClientPacketHandler, PlayerRotatedPacketHandler>();
            services.AddSingleton<IClientPacketHandler, EnemySpawnedPacketHandler>();
            services.AddSingleton<IClientPacketHandler, EnemyDeathPacketHandler>();
            services.AddSingleton<IClientPacketHandler, MapFinishedLoadingPacketHandler>();
            services.AddSingleton<IClientPacketHandler, MapObjectChunkPacketHandler>();

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


            var _world = serviceProvider.GetService<GameWorld>();
            manager = serviceProvider.GetService<UIManager>();
            executor = serviceProvider.GetService<EventHandlerExecutor>();

            var _lobbyContext = serviceProvider.GetService<LobbyContext>();

            base.OnInitializeMelon();
        }
    }


}
