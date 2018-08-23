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
            
            _menuController.Add(new MenuButton("Test"));
            _menuController.Add(new MenuButton("Test 2"));
            _menuController.Add(new MenuButton("Test 333"));
            _menuController.Add(new MenuButton("Test 4"));
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
            screenSize = Program.ScaleBatch(batch);
            
            _menuController.Draw(batch, new Vector2(screenSize.X / 2 - _menuController.Measure().X / 2, screenSize.Y / 2 - _menuController.Measure().Y / 2), true);
            
            Program.BlackBorders(batch);
        }
    }
}
