using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multibonk.Game.World;

namespace Multibonk.Game.Handlers.NetworkNotify
{
    public class GamePauseEventHandler : GameEventHandler
    {
        public GamePauseEventHandler(GameWorld world)
        {

            GamePatchEvents.PauseEvent += () =>
            {
                _pauseManager.Handle(playerId, PauseAction.Pause);
            };

            GamePatchEvents.UnpauseEvent += () =>
            {
                _pauseManager.Handle(playerId, PauseAction.Unpause);
            };
        }
    }
}
