using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Multibonk.Networking.Lobby;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Game.Handlers;
using Multibonk.Game.World.Session.Managers;
using Multibonk.Game.World;
using UnityEngine;
using MelonLoader;
using Il2CppAssets.Scripts.Actors.Enemies;

namespace Multibonk.Networking.Comms.Server.Handlers
{
    public class SpawnEnemyPacketHandler : IServerPacketHandler
    {
        private readonly LobbyContext _lobbyContext;
        private GameWorld _gameWorld;

        public SpawnEnemyPacketHandler(LobbyContext lobbyContext, GameWorld world)
        {
            _lobbyContext = lobbyContext;
            _gameWorld = world;
        }

        public byte PacketId => (byte)ClientSentPacketId.SPAWN_ENEMY_PACKET;

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new SpawnEnemyPacket(msg);

            var playerId = _lobbyContext.GetPlayer(conn).UUID;
            GameDispatcher.Enqueue(() =>
            {
                var enemyData = WorldEnemyManager.GetEnemyData(packet.EnemyIndex);
                var player = _gameWorld.CurrentSession.PlayerManager.GetPlayer(playerId);
                var rb = player.PlayerObject.GetComponent<Rigidbody>();

                if (enemyData == null)
                {
                    MelonLogger.Msg($"EnemyData for index {packet.EnemyIndex} was not found");
                    return;
                }

                Enemy enemy = _gameWorld.CurrentSession.EnemyManager.SpawnEnemy(packet.EnemyUUID, enemyData, packet.Position, EEnemyFlag.None, true);
                enemy.target = rb;


                MelonLogger.Msg("Telling others to spawn enemy " + playerId);

                var spawnedPacket = new SendEnemySpawnedPacket(packet.EnemyUUID, packet.EnemyIndex, packet.Position, playerId);
                foreach (var p in _lobbyContext.GetPlayers())
                {
                    if (p.Connection == null || p.UUID == playerId)
                        continue;

                    p.Connection.EnqueuePacket(spawnedPacket);
                }

            });


        }
    }
}