using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Menu
{
    public class MenuButton
    {
        private double _selectTime;
        
        public string Text;
        public Font Font;

        public MenuController Controller;

        public delegate void OnClicked(MenuButton button);
        public OnClicked Clicked;

        public delegate void OnLeftButton(MenuButton button);
        public OnLeftButton LeftButton;

        public delegate void OnRightButton(MenuButton button);
        public OnRightButton RightButton;

        public bool IsSelected
        {
            get; protected set;
        }

        public MenuButton(string text, Font font = null)
        {
            Text = text;
            Font = font;
        }

        public virtual void OnSelected(MenuButton previous)
        {
            _selectTime = Controller.Game.Time;
            IsSelected = true;
        }

        public virtual void OnDeselected(MenuButton next)
        {
            IsSelected = false;
        }

        public virtual void Update(MenuController controller, double deltaTime, double totalTime, long count)
        {
        }

        public virtual Vector2 Measure()
        {
            return Controller.Game.Assets.Get<Texture2D>("ui/button/normal.png").Bounds.Size.ToVector2();
        }

        // TODO: NÄR MAN KLICKAR PÅ KNAPPEN SKA DEN BLINKA TILL TVÅ GGR INNAN NÅGOT HÄNDER

        public virtual void Draw(SpriteBatch batch, Vector2 pos)
        {
            Texture2D texture = Controller.Game.Assets.Get<Texture2D>("ui/button/normal.png");
            if(IsSelected)
                texture = Controller.Game.Assets.Get<Texture2D>("ui/button/hover.png");

            batch.Texture(pos, texture, Color.White);

            Vector2 measure = Font.Measure(16, Text);
            batch.Text(Font, 16, Text, pos + Measure() / 2 - measure / 2 - new Vector2(0, 2), 
                IsSelected ? new Color(233, 188, 255) : new Color(59, 54, 54));
        }
    }
}
