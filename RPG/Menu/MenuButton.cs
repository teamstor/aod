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
        public Color Outline;

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

        public MenuButton(string text, Color? color = null, Font font = null)
        {
            Text = text;
            Font = font;
            Outline = color.HasValue ? color.Value : Color.RoyalBlue;
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
            return Font.Measure(16, Text);
        }

        public virtual void Draw(SpriteBatch batch, Vector2 pos)
        {
            Color darkColor = Color.RoyalBlue;
            darkColor.R = (byte) (darkColor.R * 0.6);
            darkColor.G = (byte) (darkColor.G * 0.6);
            darkColor.B = (byte) (darkColor.B * 0.6);
                        
            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                    batch.Text(Font, 16, Text, 
                        pos + new Vector2(x, y), 
                        IsSelected ? Color.RoyalBlue : darkColor);
            }
            
            batch.Text(Font, 16, Text, pos, Color.White);

            if(IsSelected)
            {
                Texture2D tex = Controller.Game.Assets.Get<Texture2D>("ui/arrow.png");
                Vector2 tpos = new Vector2(pos.X - tex.Width - 6 - (Math.Sin((Controller.Game.Time - _selectTime) * 10) > 0 ? 1 : 0), pos.Y + 9);
                
                for(int x = -1; x <= 1; x++)
                {
                    for(int y = -1; y <= 1; y++)
                        batch.Texture(tpos + new Vector2(x, y), tex, Color.RoyalBlue);
                }
                
                batch.Texture(tpos, tex, Color.White);

                tpos = new Vector2(pos.X + Measure().X + 6 + (Math.Sin((Controller.Game.Time - _selectTime) * 10) > 0 ? 1 : 0), pos.Y + 9);
                
                for(int x = -1; x <= 1; x++)
                {
                    for(int y = -1; y <= 1; y++)
                        batch.Texture(tpos + new Vector2(x, y), tex, Color.RoyalBlue, null, null, 0, null, SpriteEffects.FlipHorizontally);
                }
                
                batch.Texture(tpos, tex, Color.White, null, null, 0, null, SpriteEffects.FlipHorizontally);
            }
        }
    }
}
