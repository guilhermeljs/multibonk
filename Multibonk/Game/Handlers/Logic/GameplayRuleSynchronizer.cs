using MelonLoader;
using UnityEngine;

namespace Multibonk.Game.Handlers.Logic
{
    public class GameplayRuleSynchronizer : GameEventHandler
    {
        private GameplayRulesSnapshot lastSnapshot;
        private float lastLogTime;

        public GameplayRuleSynchronizer()
        {
            lastSnapshot = GameplayRulesSnapshot.FromPreferences();
            GamePatchFlags.SetGameplayRules(lastSnapshot);
            GameEvents.GameLoadedEvent += ApplyRulesToWorld;
            GameEvents.ConfirmMapEvent += ApplyRulesToWorld;
        }

        public override void Update()
        {
            if (Time.frameCount % 60 != 0)
            {
                return;
            }

            var current = GameplayRulesSnapshot.FromPreferences();
            if (current.Equals(lastSnapshot))
            {
                return;
            }

            lastSnapshot = current;
            GamePatchFlags.SetGameplayRules(current);
            ApplyRulesToWorld();
        }

        private void ApplyRulesToWorld()
        {
            GamePatchFlags.SetGameplayRules(lastSnapshot);

            if (Time.time - lastLogTime > 5f)
            {
                MelonLogger.Msg($"Applied gameplay rules: {lastSnapshot}.");
                lastLogTime = Time.time;
            }
        }
    }
}
