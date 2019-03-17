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
        public MenuUI UI;
        
        public override void OnEnter(GameState previousState)
        {
            UI = new MenuUI(this, "main", new MenuPage(150, new MenuButton(null)), true);
            UI.SelectedPage.Elements.Add(new MenuButton(UI.SelectedPage));
            UI.SelectedPage.Elements.Add(new MenuButton(UI.SelectedPage));
            UI.SelectedPage.Elements.Add(new MenuButton(UI.SelectedPage));
            UI.SelectedPage.Elements.Add(new MenuButton(UI.SelectedPage));
            UI.SelectedPage.Elements.Add(new MenuButton(UI.SelectedPage));
            UI.SelectedPage.Elements.Add(new MenuButton(UI.SelectedPage));
            UI.SelectedPage.Elements.Add(new MenuButton(UI.SelectedPage));
            UI.SelectedPage.Elements.Add(new MenuButton(UI.SelectedPage));
            UI.SelectedPage.Elements.Add(new MenuButton(UI.SelectedPage));


            UI.Toggle();
            
            Game.IsMouseVisible = false;
        }

        public override void OnLeave(GameState nextState)
        {
        }


        public override void Update(double deltaTime, double totalTime, long count)
        {
            if(Input.Key(Keys.X) && UI.State != MenuUIState.Transitioning)
                UI.Toggle();
            
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

            Vector2 uiPos = screenSize / 2 - UI.Area.Value / 2 + new Vector2(0, 40);
            UI.Draw(uiPos, batch, screenSize);
            
            Program.BlackBorders(batch);
        }
    }
}
