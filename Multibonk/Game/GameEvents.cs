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


        public static event Action<Animator, string, bool> SetBoolEvent;
        public static event Action<Animator, string, float> SetFloatEvent;
        public static event Action<Animator, string, int> SetIntEvent;
        public static event Action<Animator, string> SetTriggerEvent;

        public static event Action<BaseInteractable> BaseInteractableSpawnedEvent;
        public static event Action<int> BaseInteractableDestroyedEvent;


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

        public static void TriggerBaseInteractableSpawned(BaseInteractable interactable)
        {
            BaseInteractableSpawnedEvent?.Invoke(interactable);
        }

        public static void TriggerBaseInteractableDestroyed(int instanceId)
        {
            BaseInteractableDestroyedEvent?.Invoke(instanceId);
        }
    }
}
