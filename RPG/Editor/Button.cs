using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;
using Game = TeamStor.Engine.Game;

namespace TeamStor.RPG.Map.Editor
{
    public class Button
    {
        public string Text;
        public Texture2D Icon;
        public TweenedVector2 Position;
        public Font Font;

        public delegate void OnClicked(Button button);
        public OnClicked Clicked;

        public Rectangle Rectangle
        {
            get
            {
                Rectangle rectangle = new Rectangle((int)Position.Value.X, (int)Position.Value.Y, 4, 32);

                if(Icon != null)
                    rectangle.Width += Icon.Width + 4;

                if(Text != "")
                    rectangle.Width += (int)Font.Measure(15, Text).X + 4;

                return rectangle;
            }
        }

        public bool Active;

        public void Update(Game game)
        {
            bool hovered = !Active && Rectangle.Contains(game.Input.MousePosition);

            if(hovered && game.Input.MousePressed(MouseButton.Left) && Clicked != null)
                Clicked(this);
        }

        public void Draw(Game game)
        {
            SpriteBatch batch = game.Batch;
            bool hovered = Active || Rectangle.Contains(game.Input.MousePosition);

            batch.Rectangle(Rectangle, Color.Black * 0.8f);

            if(Icon != null)
                batch.Texture(new Vector2(Position.Value.X + 4, Position.Value.Y + 4), Icon, Color.White * (hovered ? 1f : 0.6f));

            if(Text != "")
                batch.Text(Font, 15, Text, new Vector2(Position.Value.X + (Icon != null ? Icon.Width + 8 : 4), Position.Value.Y + 6), Color.White * (hovered ? 1f : 0.6f));
        }
    }
}
