﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using System.IO;
using TeamStor.AOD.Gameplay;
using TeamStor.AOD.Gameplay.World;
using Microsoft.Xna.Framework.Graphics;

using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Menu
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
                Game.CurrentState = new WorldState(Map.Load(AppDomain.CurrentDomain.BaseDirectory + "data/maps/mapt.json"));
            };

            _menuController.Add(newGame);
            _menuController.Add(new MenuButton("Load Game"));
            _menuController.Add(new MenuButton("Options"));
            _menuController.Add(new MenuButton("Exit"));

            Game.IsMouseVisible = false;
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

            batch.Texture(new Vector2(0, 0), Assets.Get<Texture2D>("menu/bg.png"), Color.White);
            batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black * 0.5f);
            batch.Texture(new Vector2(screenSize.X / 2 - 160 / 2, 20), Assets.Get<Texture2D>("ui/logo.png"), Color.White);
            
            _menuController.Draw(batch, new Vector2(
                screenSize.X / 2 - _menuController.Size.X / 2, 
                screenSize.Y / 2 - _menuController.Size.Y / 2 + 40), true);

            Program.BlackBorders(batch);
        }
    }
}