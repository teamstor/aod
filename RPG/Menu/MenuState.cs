using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;

namespace TeamStor.RPG.Menu
{
    public class MenuState : GameState
    {
        private MenuController _menuController;

        public override void OnEnter(GameState previousState)
        {
            _menuController = new MenuController(Game);
        }

        public override void OnLeave(GameState nextState)
        {
            
        }

        public override void Update(double deltaTime, double totalTime, long count)
        {
            _menuController.Update(deltaTime, totalTime, count);
        }

        public override void FixedUpdate(long count)
        {
            
        }

        public override void Draw(SpriteBatch batch, Vector2 screenSize)
        {
            _menuController.Draw(batch, new Vector2(screenSize.X / 4 - _menuController.Measure().X / 2, screenSize.Y / 4 - _menuController.Measure().Y / 2));
        }
    }
}
