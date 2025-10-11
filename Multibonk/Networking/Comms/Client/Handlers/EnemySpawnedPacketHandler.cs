using UnityEngine;
using Multibonk.Networking.Comms.Base;
using Multibonk.Networking.Comms.Base.Packet;
using Il2CppAssets.Scripts.Actors.Enemies;
using Multibonk.Game.Handlers;
using Multibonk.Networking.Comms.Packet.Base.Multibonk.Networking.Comms;
using Multibonk.Networking.Lobby;
using MelonLoader;
using Multibonk.Game.World.Session.Managers;
using Multibonk.Game.World;

namespace Multibonk.Networking.Comms.Client.Handlers
{
    internal class EnemySpawnedPacketHandler : IClientPacketHandler
    {
        public byte PacketId => (byte)ServerSentPacketId.ENEMY_SPAWNED_PACKET;
        private readonly LobbyContext _lobbyContext;
        private readonly GameWorld _gameWorld;

        public EnemySpawnedPacketHandler(LobbyContext lobby, GameWorld world)
        {
            _lobbyContext = lobby;
            _gameWorld = world;
        }

        public void Handle(IncomingMessage msg, Connection conn)
        {
            var packet = new EnemySpawnedPacket(msg);

            GameDispatcher.Enqueue(() =>
            {
                var enemyData = WorldEnemyManager.GetEnemyData(packet.EnemyIndex);
                if (enemyData == null)
                {
                    MelonLogger.Msg($"EnemyData from index {packet.EnemyIndex} was not found");
                    return;
                }

                var enemy = _gameWorld.CurrentSession.EnemyManager.SpawnEnemy(packet.EnemyId, enemyData, packet.Position, EEnemyFlag.None, true);

                if (packet.TargetPlayerId == _lobbyContext.GetMyself().UUID)
                    return;

                var target = _gameWorld.CurrentSession.PlayerManager.GetPlayer(packet.TargetPlayerId)?.PlayerObject;
                if (target != null)
                {
                    var rb = target.GetComponent<Rigidbody>();
                    enemy.target = rb;
                }
            });
        }
    }
}