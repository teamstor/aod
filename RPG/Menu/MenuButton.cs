using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine.Graphics;

namespace TeamStor.RPG.Menu
{
    public class MenuButton
    {
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
            Font = font != null ? font : Controller.Game.Assets.Get<Font>("fonts/bitcell.ttf");
        }

        public virtual void OnSelected(MenuButton previous)
        {
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
            return Font.Measure(8, Text);
        }

        public virtual void Draw(SpriteBatch batch, Vector2 pos)
        {

        }
    }
}
