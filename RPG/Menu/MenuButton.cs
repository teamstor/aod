using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;

namespace TeamStor.RPG.Menu
{
    public class MenuButton
    {
        protected TweenedDouble _offset;
        
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
            IsSelected = true;
            
            _offset = new TweenedDouble(Controller.Game, -4);
            _offset.TweenTo(0, TweenEaseType.EaseOutSine, 0.1);
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
            return Font.Measure(16, Text);
        }

        public virtual void Draw(SpriteBatch batch, Vector2 pos)
        {
            batch.Text(Font, 16, Text, pos + new Vector2(0, _offset.CompletionTime < 0.1 ? 0 : (int)_offset), Color.White);
        }
    }
}
