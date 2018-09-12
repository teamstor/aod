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
            
            _menuController.Add(new MenuButton("New Game"));
            _menuController.Add(new MenuButton("Load Game"));
            _menuController.Add(new MenuButton("Options"));
            _menuController.Add(new MenuButton("Exit"));
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

            batch.Text(Game.Assets.Get<Font>("fonts/Alkhemikal.ttf"), 16, "Version " + Program.VERSION, new Vector2(10, screenSize.Y - 16 - 10), Color.White * 0.6f);
            
            _menuController.Draw(batch, new Vector2(
                screenSize.X / 2 - _menuController.Measure().X / 2, 
                screenSize.Y / 2 - _menuController.Measure().Y / 2 + 20), true);
            
            Program.BlackBorders(batch);
        }
    }
}
