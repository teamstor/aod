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
using Microsoft.Xna.Framework.Input;
using TeamStor.AOD.Menu.Elements;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Menu
{
    public class MenuState : GameState
    {
        public MenuUI UI;
        public MenuOptions OptionsUI;
        
        public override void OnEnter(GameState previousState)
        {
            MenuPage mainPage = new MenuPage(150);
            mainPage.Add(new MenuButton(mainPage, "Start New Game", "icons/start_game.png"));
            mainPage.Add(new MenuButton(mainPage, "Load Game", "icons/load_game.png"));
            mainPage.Add(new MenuSpacer(4));
            mainPage.Add(new MenuButton(mainPage, "Options", "icons/settings.png", "", "", true)).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) OptionsUI.SwitchToOptionsPage(); });
            mainPage.Add(new MenuButton(mainPage, "Exit Game", "icons/arrow_left.png")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) Game.Exit(); });

            UI = new MenuUI(this, "main", mainPage, true);
            OptionsUI = new MenuOptions(UI, "main");

            UI.Toggle();
            
            Game.IsMouseVisible = false;
        }

        public override void OnLeave(GameState nextState)
        {
        }


        public override void Update(double deltaTime, double totalTime, long count)
        {
            OptionsUI.UpdateLabels();
            UI.Update(deltaTime, Input);
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

            Font font = Assets.Get<Font>("fonts/Poco.ttf", true);
            string copyrightString = "Copyright (C) 2018-2019 Hannes Mann, Kasper Kjällström, Henrik Eriksson";
            Vector2 measure = font.Measure(10, copyrightString);

            batch.Text(font, 10, copyrightString, new Vector2(screenSize.X / 2 - measure.X / 2, screenSize.Y - 35), Color.White * 0.5f);

            Vector2 uiPos = screenSize / 2 - UI.Area.Value / 2 + new Vector2(0, 20);
            UI.Draw(uiPos, batch, screenSize);
            
            Program.BlackBorders(batch);
        }
    }
}
