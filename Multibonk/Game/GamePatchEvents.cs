using Il2Cpp;
using Il2CppAssets.Scripts.Actors.Enemies;
using Il2CppAssets.Scripts.Managers;
using UnityEngine;

namespace Multibonk.Game
{
    internal static class GamePatchEvents
    {
        public static event Action ConfirmCharacterEvent;
        public static event Action ConfirmMapEvent;
        public static event Action<CharacterData> CharacterChanged;
        public static event Action<MapData> MapChanged;
        public static event Action GameLoadedEvent;

        public static event Action<Vector3> PlayerMoveEvent;
        public static event Action<Quaternion> PlayerRotateEvent;

        public static event Action PlayerDieEvent;
        public static event Action PlayerTakeHitEvent;

        public static event Action BossSpawnEvent;
        public static event Action BossDamagedEvent;

        /// <summary>
        /// Enemies spawned by the network DO NOT call this event. This is for enemies that spawned in local only
        /// </summary>
        public static event Action<Enemy> EnemySpawnedEvent;
        public static event Action<Enemy> EnemyDiedEvent;

        public static event Action EnemyDieEvent;

        /// <summary>
        /// Pauses triggered by network won't call this
        /// </summary>
        public static event Action PauseEvent;
        /// <summary>
        /// Unpauses triggered by network won't call this.
        /// </summary>
        public static event Action UnpauseEvent;
        

        public static event Action UseShrineEvent;

        public static event Action SpawnDropEvent;
        public static event Action OpenChestEvent;

        public static event Action PlayerLevelUpEvent;
        public static event Action<int> PlayerXpAddedEvent;


        public static event Action<Animator, string, bool> SetBoolEvent;
        public static event Action<Animator, string, float> SetFloatEvent;
        public static event Action<Animator, string, int> SetIntEvent;
        public static event Action<Animator, string> SetTriggerEvent;

        /// <summary>
        /// This is called when the Spawn function is called by the game.
        /// If our managed code calls the EnemyManager.SpawnFunction, it will not trigger this event
        /// </summary>
        public static event Action<EnemyManager, EnemyData, int, bool, EEnemyFlag, bool> OnSpawnEnemyBasic;
        public static event Action<EnemyManager, EnemyData, Vector3, int, bool, EEnemyFlag, bool> OnSpawnEnemyWithPosition;


        public static void TriggerPauseEvent()
        {
            PauseEvent?.Invoke();
        }

        public static void TriggerUnpauseEvent()
        {
            UnpauseEvent?.Invoke();
        }

        public static void TriggerEnemyDie(Enemy enemy)
        {
            EnemyDiedEvent?.Invoke(enemy);
        }

        public static void TriggerEnemySpawned(Enemy e)
        {
            EnemySpawnedEvent?.Invoke(e);
        }


        public static void TriggerSpawnEnemy(EnemyManager manager, EnemyData data, int summonerId, bool forceSpawn, EEnemyFlag enemyFlag, bool useDirectionBias)
        {
            OnSpawnEnemyBasic?.Invoke(manager, data, summonerId, forceSpawn, enemyFlag, useDirectionBias);
        }

        public static void TriggerMapChanged(MapData data)
        {
            MapChanged?.Invoke(data);
        }

        public static void TriggerSpawnEnemy(EnemyManager manager, EnemyData data, Vector3 position, int waveNumber, bool forceSpawn, EEnemyFlag enemyFlag, bool canBeElite)
        {
            OnSpawnEnemyWithPosition?.Invoke(manager, data, position, waveNumber, forceSpawn, enemyFlag, canBeElite);
        }


        public static void TriggerSetBool(Animator animator, string param, bool value) => SetBoolEvent?.Invoke(animator, param, value);
        public static void TriggerSetFloat(Animator animator, string param, float value) => SetFloatEvent?.Invoke(animator, param, value);
        public static void TriggerSetInt(Animator animator, string param, int value) => SetIntEvent?.Invoke(animator, param, value);
        public static void TriggerSetTrigger(Animator animator, string param) => SetTriggerEvent?.Invoke(animator, param);


        public static void TriggerConfirmMap()
        {
            ConfirmMapEvent?.Invoke();
        }

        public static void TriggerConfirmCharacter()
        {
            ConfirmCharacterEvent?.Invoke();
        }

        public static void TriggerCharacterChanged(CharacterData character)
        {
            CharacterChanged?.Invoke(character);
        }

        public static void TriggerAddXpEvent(int amount)
        {
            PlayerXpAddedEvent?.Invoke(amount);
        }

        public static void TriggerGameLoadedEvent()
        {
            GameLoadedEvent?.Invoke();
        }
        public static void TriggerPlayerMoved(Vector3 newPosition)
        {
            PlayerMoveEvent?.Invoke(newPosition);
        }

        public static void TriggerPlayerRotated(Quaternion newRotation)
        {
            PlayerRotateEvent?.Invoke(newRotation);
        }
    }
}
