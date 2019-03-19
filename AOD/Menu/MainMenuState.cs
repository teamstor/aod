using System;
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
        private MenuButton fullscreenButton, vsyncButton;

        public MenuUI UI;
        
        public override void OnEnter(GameState previousState)
        {
            MenuPage mainPage = new MenuPage(150);
            mainPage.Add(new MenuButton(mainPage, "Start New Game", "icons/start_game.png"));
            mainPage.Add(new MenuButton(mainPage, "Load Game", "icons/load_game.png"));
            mainPage.Add(new MenuSpacer(4));
            mainPage.Add(new MenuButton(mainPage, "Options", "icons/settings.png", "", "", true)).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) UI.SwitchPage("options", false); });
            mainPage.Add(new MenuButton(mainPage, "Exit Game", "icons/arrow_left.png")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) Game.Exit(); });

            MenuPage optionsPage = new MenuPage(150);
            fullscreenButton = optionsPage.Add(new MenuButton(optionsPage, "Fullscreen: ?", "icons/fullscreen.png", "", "Displays the game over the\nwhole monitor")) as MenuButton;
            fullscreenButton.RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) Game.Fullscreen = !Game.Fullscreen; });
            
            vsyncButton = optionsPage.Add(new MenuButton(optionsPage, "V-sync: ?", "icons/vsync.png", "", "Synchronizes the game to\nthe monitor")) as MenuButton;
            vsyncButton.RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) Game.VSync = !Game.VSync; });

            optionsPage.Add(new MenuSpacer(4));
            optionsPage.Add(new MenuButton(optionsPage, "Audio", "icons/audio.png", "", "", true)).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) UI.SwitchPage("options/audio", false); });
            optionsPage.Add(new MenuButton(optionsPage, "Input", "icons/input.png", "", "", true)).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) UI.SwitchPage("options/input", false); });
            optionsPage.Add(new MenuSpacer(4));
            optionsPage.Add(new MenuButton(optionsPage, "Back", "icons/arrow_left.png")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) UI.SwitchPage("main", true); });

            UI = new MenuUI(this, "main", mainPage, true);
            UI.AddPage("options", optionsPage);

            UI.Toggle();
            
            Game.IsMouseVisible = false;
        }

        public override void OnLeave(GameState nextState)
        {
        }


        public override void Update(double deltaTime, double totalTime, long count)
        {
            fullscreenButton.Label = "Fullscreen: " + (Game.Fullscreen ? "On" : "Off");
            vsyncButton.Label = "V-sync: " + (Game.VSync ? "On" : "Off");

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
