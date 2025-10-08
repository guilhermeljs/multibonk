namespace Multibonk.Game.Handlers
{
    public interface IGameEventHandler { }

    public abstract class GameEventHandler : IGameEventHandler
    {
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void LateUpdate() { }
    }
}
