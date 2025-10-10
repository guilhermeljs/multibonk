using Multibonk.Game.Handlers;

namespace Multibonk.Game
{
    public class EventHandlerExecutor
    {
        private readonly IEnumerable<IGameEventHandler> handlers;

        public EventHandlerExecutor(IEnumerable<IGameEventHandler> handlers)
        {
            this.handlers = handlers;
        }

        public void Update()
        {
            foreach (var h in handlers.OfType<GameEventHandler>())
                h.Update();
        }

        public void FixedUpdate()
        {
            foreach (var h in handlers.OfType<GameEventHandler>())
                h.FixedUpdate();
        }

        public void LateUpdate()
        {
            foreach (var h in handlers.OfType<GameEventHandler>())
                h.LateUpdate();
        }
    }
}
