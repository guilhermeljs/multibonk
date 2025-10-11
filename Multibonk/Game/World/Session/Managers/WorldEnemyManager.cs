using Il2Cpp;
using Il2CppAssets.Scripts.Actors.Enemies;
using Il2CppAssets.Scripts.Managers;
using Multibonk.Game.Utils;
using UnityEngine;

namespace Multibonk.Game.World.Session.Managers
{
    public class WorldEnemyManager
    {

        private static List<EnemyData> _indexedByDescription;
        private static List<EnemyData> IndexedByDescription
        {
            get
            {
                if (_indexedByDescription != null)
                    return _indexedByDescription;

                var allEnemies = Resources.FindObjectsOfTypeAll<EnemyData>();

                _indexedByDescription = allEnemies
                    .OrderBy(e => e.description)
                    .ToList();

                return _indexedByDescription;
            }
        }


        private EnemyDictionary<Enemy> _enemies;

        public WorldEnemyManager(byte myClientId)
        {
            _enemies = new EnemyDictionary<Enemy>(myClientId);
        }

        public uint RegisterEnemySpawned(Enemy e)
        {
            return _enemies.Add(e);
        }

        public static int GetEnemyPrefabIndex(Enemy e)
        {
            return IndexedByDescription.IndexOf(e.enemyData);
        }

        public static EnemyData GetEnemyData(int id)
        {
            return IndexedByDescription[id];
        }

        public uint GetIdFromEnemy(Enemy e)
        {
            return _enemies.GetId(e);
        }

        public Enemy GetEnemy(uint id)
        {
            return _enemies.Get(id);
        }

        /// <summary>
        /// Used to spawn enemies through our managed code
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="position"></param>
        /// <param name="flag"></param>
        /// <param name="canBeElite"></param>
        ///// <returns></returns>
        public Enemy SpawnEnemy(
            uint networkId, EnemyData data, Vector3 position, EEnemyFlag flag, bool canBeElite
        )
        {
            GamePatchFlags.AllowSpawnEnemyCall = true;

            var enemy = EnemyManager.Instance.SpawnEnemy(
                data,
                position,
                0,
                true,
                flag,
                canBeElite
            );

            GamePatchFlags.AllowSpawnEnemyCall = false;

            if (enemy == null)
                return null;

            _enemies.Add(networkId, enemy);

            return enemy;
        }

        public void KillEnemy(
            uint enemyId
        )
        {
            GetEnemy(enemyId)?.Kill();
        }
    }
}
