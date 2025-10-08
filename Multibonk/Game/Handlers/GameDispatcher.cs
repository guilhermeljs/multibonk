using MelonLoader;
using System.Collections.Concurrent;

namespace Multibonk.Game.Handlers
{
    public class GameDispatcher : GameEventHandler
    {
        private static readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        public override void Update()
        {
            while (_actions.TryDequeue(out var action))
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    MelonLogger.Msg($"Exception in GameDispatcher action: {ex}");
                }
            }
        }


        public static void Enqueue(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _actions.Enqueue(action);
        }
    }
}