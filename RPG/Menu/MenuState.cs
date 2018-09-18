using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using System.IO;
using TeamStor.RPG.Gameplay;
using TeamStor.RPG.Gameplay.World;

namespace TeamStor.RPG.Menu
{
    public class MenuState : GameState
    {
        private MenuController _menuController;

        public override void OnEnter(GameState previousState)
        {
            _menuController = new MenuController(Game);

            MenuButton newGame = new MenuButton("New Game");
            newGame.Clicked = (btn) =>
            {
                Game.CurrentState = new WorldState(Map.Load(File.OpenRead("data/maps/gameplay_test.map")));
            };

            _menuController.Add(newGame);
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
            
            _menuController.Draw(batch, new Vector2(
                screenSize.X / 2 - _menuController.Size.X / 2, 
                screenSize.Y / 2 - _menuController.Size.Y / 2 + 20), true);
            
            Program.BlackBorders(batch);
        }
    }
}
