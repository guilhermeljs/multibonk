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

    public sealed class PauseManager
    {
        private readonly HashSet<byte> pausedPlayers = new();
        private readonly HashSet<byte> pendingUnpause = new();
        public GamePauseState PauseState { get; set; } = GamePauseState.Running;

        public event Action OnGamePaused;
        public event Action OnGameUnpaused;
        public event Action<byte> OnPlayerPaused;
        public event Action<byte> OnPlayerPendingUnpause;

        public void Handle(byte playerId, PauseAction action)
        {
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

        private void PauseGame()
        {
            MyTime.Pause();
        }

        private void ResumeGame()
        {
            MyTime.Unpause();
        }

        private void HandlePause(byte playerId)
        {
            if (pausedPlayers.Contains(playerId))
                return;

            pausedPlayers.Add(playerId);
            pendingUnpause.Remove(playerId);
            OnPlayerPaused?.Invoke(playerId);

            if (PauseState == GamePauseState.Running)
            {
                PauseState = GamePauseState.Paused;
                PauseGame();
                OnGamePaused?.Invoke();
            }
        }

        private void HandleUnpause(byte playerId)
        {
            if (!pausedPlayers.Contains(playerId))
                return;

            pausedPlayers.Remove(playerId);
            pendingUnpause.Add(playerId);
            OnPlayerPendingUnpause?.Invoke(playerId);

            if (pausedPlayers.Count == 0 && PauseState == GamePauseState.Paused)
            {
                PauseState = GamePauseState.Running;
                ResumeGame();
                pendingUnpause.Clear();
                OnGameUnpaused?.Invoke();
            }
        }
    }
}
