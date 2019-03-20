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
using TeamStor.Engine.Tween;
using TeamStor.Engine.Coroutine;

namespace TeamStor.AOD.Menu
{
    public class MainMenuState : GameState
    {
        public MenuUI UI;
        public MenuOptions OptionsUI;

        private TweenedDouble _exitTransition;

        private IEnumerator<ICoroutineOperation> TransitionToStoryCoroutine()
        {
            foreach(MenuElement e in UI.SelectedPage)
            {
                if(e is MenuButton && (e as MenuButton).IconLeft == "icons/start_game.png")
                {
                    for(int i = 0; i < 3; i++)
                    {
                        e.OnDeselected(null);
                        yield return Wait.Seconds(Game, 0.08f);
                        e.OnSelected(null);
                        yield return Wait.Seconds(Game, 0.08f);
                    }

                    e.OnDeselected(null);
                    yield return Wait.Seconds(Game, 0.4f);
                }
            }

            UI.Toggle();
            yield return Wait.Seconds(Game, 0.3f);

            _exitTransition.TweenTo(1, TweenEaseType.Linear, 2f);
            yield return Wait.Seconds(Game, 2.5f);

            Game.CurrentState = new StoryMenuState();
        }
        
        public override void OnEnter(GameState previousState)
        {
            _exitTransition = new TweenedDouble(Game, 0);

            MenuPage mainPage = new MenuPage(150);
            mainPage.Add(new MenuButton(mainPage, "Start New Game", "icons/start_game.png")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => 
                {
                    if(!h && Coroutine.Active.Count == 0)
                        Coroutine.Start(TransitionToStoryCoroutine);
                });

            mainPage.Add(new MenuButton(mainPage, "Load Game", "icons/load_game.png"));
            mainPage.Add(new MenuSpacer(4));
            mainPage.Add(new MenuButton(mainPage, "Options", "icons/settings.png", "", "")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) OptionsUI.SwitchToOptionsPage(); });
            mainPage.Add(new MenuButton(mainPage, "Credits", "icons/credits.png", "", "")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) UI.SwitchPage("credits", false); });
            mainPage.Add(new MenuSpacer(4));
            mainPage.Add(new MenuButton(mainPage, "Exit Game", "icons/arrow_left.png")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) Game.Exit(); });
            
            MenuPage creditsPage = new MenuPage(250);
            creditsPage.Add(new MenuLabel(creditsPage, "Age of Darkness " + Program.VERSION));
            creditsPage.Add(new MenuLabel(creditsPage, "Made with MonoGame and Team STOR Engine"));
            creditsPage.Add(new MenuLabel(creditsPage, ""));
            creditsPage.Add(new MenuLabel(creditsPage, "Programming"));
            creditsPage.Add(new MenuLabel(creditsPage, "Hannes Mann", "", "", 0.8f));
            creditsPage.Add(new MenuLabel(creditsPage, ""));
            creditsPage.Add(new MenuLabel(creditsPage, "Game Design and Art"));
            creditsPage.Add(new MenuLabel(creditsPage, "Kasper Kjällström", "", "", 0.8f));
            creditsPage.Add(new MenuLabel(creditsPage, "Henrik Eriksson", "", "", 0.8f));
            creditsPage.Add(new MenuSpacer(4));
            creditsPage.Add(new MenuButton(creditsPage, "Back", "icons/arrow_left.png")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) UI.SwitchPage("main", true); });

            UI = new MenuUI(this, "main", mainPage, true);
            UI.AddPage("credits", creditsPage);
            
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

            batch.Texture(new Vector2(0, 0), Assets.Get<Texture2D>("menu/bg.png", true), Color.White);
            batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black * 0.5f);

            Font font = Assets.Get<Font>("fonts/Poco.ttf", true);
            string copyrightString = "Copyright (C) 2018-2019 Hannes Mann, Kasper Kjällström, Henrik Eriksson";
            Vector2 measure = font.Measure(10, copyrightString);

            batch.Text(font, 10, copyrightString, new Vector2(screenSize.X / 2 - measure.X / 2, screenSize.Y - 35), Color.White * 0.5f);

            Vector2 uiPos = screenSize / 2 - UI.Area.Value / 2 + new Vector2(0, 20);
            UI.Draw(uiPos, batch, screenSize);

            batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black * _exitTransition);
            batch.Texture(new Vector2(screenSize.X / 2 - 160 / 2, 20), Assets.Get<Texture2D>("ui/logo.png", true), Color.White);

            Program.BlackBorders(batch);
        }
    }
}
