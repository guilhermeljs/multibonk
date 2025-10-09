using Il2Cpp;
using UnityEngine;

namespace Multibonk.Game
{
    internal static class GameEvents
    {
        public static event Action ConfirmCharacterEvent;
        public static event Action ConfirmMapEvent;
        public static event Action<CharacterData> CharacterChanged;
        public static event Action GameLoadedEvent;

        public static event Action<Vector3> PlayerMoveEvent;
        public static event Action<Quaternion> PlayerRotateEvent;

        public static event Action PlayerDieEvent;
        public static event Action PlayerTakeHitEvent;

        public static event Action BossSpawnEvent;
        public static event Action BossDamagedEvent;

        public static event Action EnemySpawnEvent;
        public static event Action EnemyDieEvent;

        public static event Action InGamePauseEvent;
        public static event Action InGameUnpauseEvent;

        public static event Action UseShrineEvent;

        public static event Action SpawnDropEvent;
        public static event Action OpenChestEvent;

        public static event Action PlayerLevelUpEvent;
        public static event Action<int> PlayerXpAddedEvent;


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
