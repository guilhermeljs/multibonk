using Il2CppAssets.Scripts.Utility;

namespace Multibonk.Game.World.Session.Managers
{
    public enum PauseAction
    {
        Pause,
        Unpause
    }

    public enum GamePauseState
    {
        Running,
        Paused
    }

    /// <summary>
    /// Currently, only host uses this class. Clients use ForcePause and ForceResume
    /// </summary>
    public sealed class PauseManager
    {
        private readonly HashSet<byte> pausedPlayers = new();
        public GamePauseState PauseState { get; set; } = GamePauseState.Running;

        public static event Action OnGamePaused;
        public static event Action OnGameUnpaused;
        public static event Action<byte> OnPlayerPaused;
        public static event Action<byte> OnPlayerPendingUnpause;

        public void HandlePauseAction(byte playerId, PauseAction action)
        {
            MelonLoader.MelonLogger.Msg($"HandlePauseAction called: playerId={playerId}, action={action}, currentState={PauseState}");
            switch (action)
            {
                case PauseAction.Pause:
                    HandlePause(playerId);
                    break;
                case PauseAction.Unpause:
                    HandleUnpause(playerId);
                    break;
            }
        }

        public void ForcePauseGame()
        {
            MelonLoader.MelonLogger.Msg("ForcePauseGame called");
            PauseGame();
        }

        public void ForceResumeGame()
        {
            MelonLoader.MelonLogger.Msg("ForceResumeGame called");
            ResumeGame();
        }

        private void PauseGame()
        {
            MelonLoader.MelonLogger.Msg("PauseGame starting");
            GamePatchFlags.AllowPauseCall = true;
            MyTime.Pause();
            GamePatchFlags.AllowPauseCall = false;
            PauseState = GamePauseState.Paused;
            MelonLoader.MelonLogger.Msg("PauseGame finished, PauseState=Paused");
        }

        private void ResumeGame()
        {
            MelonLoader.MelonLogger.Msg("ResumeGame starting");
            GamePatchFlags.AllowUnpauseCall = true;
            MyTime.Unpause();
            GamePatchFlags.AllowUnpauseCall = false;
            PauseState = GamePauseState.Running;
            MelonLoader.MelonLogger.Msg("ResumeGame finished, PauseState=Running");
        }

        private void HandlePause(byte playerId)
        {
            MelonLoader.MelonLogger.Msg($"HandlePause called: playerId={playerId}");
            if (!pausedPlayers.Add(playerId))
            {
                MelonLoader.MelonLogger.Msg($"Player {playerId} already in pausedPlayers");
                return;
            }

            MelonLoader.MelonLogger.Msg($"Player {playerId} added to pausedPlayers");
            OnPlayerPaused?.Invoke(playerId);

            if (PauseState == GamePauseState.Running)
            {
                MelonLoader.MelonLogger.Msg("Game was running, pausing now");
                PauseState = GamePauseState.Paused;
                PauseGame();
                OnGamePaused?.Invoke();
            }
            else
            {
                MelonLoader.MelonLogger.Msg("Game already paused, skipping PauseGame");
            }
        }

        private void HandleUnpause(byte playerId)
        {
            MelonLoader.MelonLogger.Msg($"HandleUnpause called: playerId={playerId}");
            if (!pausedPlayers.Remove(playerId))
            {
                MelonLoader.MelonLogger.Msg($"Player {playerId} not in pausedPlayers, skipping");
                return;
            }

            MelonLoader.MelonLogger.Msg($"Player {playerId} removed from pausedPlayers");

            if (pausedPlayers.Count == 0 && PauseState == GamePauseState.Paused)
            {
                MelonLoader.MelonLogger.Msg("No paused players remaining, resuming game");
                PauseState = GamePauseState.Running;
                ResumeGame();
                OnGameUnpaused?.Invoke();
            }
            else
            {
                MelonLoader.MelonLogger.Msg($"Paused players remaining: {pausedPlayers.Count}, game remains paused");
            }
        }
    }

}
