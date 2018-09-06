using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;
using Game = TeamStor.Engine.Game;

namespace TeamStor.RPG.Editor
{
	public class SelectionMenu
	{
        public const string SPACING = "[space]";

        public string Title;
		public List<string> Entries;
        public int MaxHeight = -1;

		public int Selected = 0;

        public string SelectedValue
        {
            get
            {
                return Entries[Selected];
            }
        }

        public int Hovered(Game game)
        {
            int y = Rectangle.Value.Y + 15 + 12;
            
            for(int i = 0; i < Entries.Count; i++)
            {
                string s = Entries[i];

                if(s == SPACING)
                    y += 8;
                else
                {
                    Vector2 measure = game.DefaultFonts.Bold.Measure(15, s);
                    Rectangle rectangle = new Rectangle(Rectangle.Value.X, y, Rectangle.Value.Width, (int)measure.Y);

                    if(rectangle.Contains(game.Input.MousePosition))
                        return i;

                    y += 15 + 4;
                }
            }

            return -1;
        }

        public TweenedRectangle Rectangle;
		
		public delegate void OnSelectionChanged(SelectionMenu menu, int newSelected);
		public OnSelectionChanged SelectionChanged;

        public void Update(Game game)
		{
            if(Rectangle.Value.Contains(game.Input.MousePosition) && !Rectangle.Value.Contains(game.Input.PreviousMousePosition))
            {
                int height = 15 + 12 + 4;
                foreach(string s in Entries)
                {
                    if(s == SPACING)
                        height += 8;
                    else
                        height += 15 + 4;
                }

                Rectangle.TweenTo(new Rectangle(Rectangle.TargetValue.X, Rectangle.TargetValue.Y, Rectangle.TargetValue.Width, height), TweenEaseType.EaseOutQuad, 0.1f);
            }
            else if(!Rectangle.Value.Contains(game.Input.MousePosition) && Rectangle.Value.Contains(game.Input.PreviousMousePosition))
                Rectangle.TweenTo(new Rectangle(Rectangle.TargetValue.X, Rectangle.TargetValue.Y, Rectangle.TargetValue.Width, 15 + 12), TweenEaseType.EaseOutQuad, 0.1f);

            int y = Rectangle.Value.Y + 15 + 12;
            foreach(string s in Entries)
            {
                if(s == SPACING)
                    y += 8;
                else
                {
                    Vector2 measure = game.DefaultFonts.Bold.Measure(15, s);
                    Rectangle rectangle = new Rectangle(Rectangle.Value.X, y, Rectangle.Value.Width, (int)measure.Y);

                    if(SelectedValue != s && Rectangle.Value.Contains(game.Input.MousePosition) && rectangle.Contains(game.Input.MousePosition) && game.Input.MousePressed(MouseButton.Left))
                    {
                        Selected = Entries.IndexOf(s);

                        if(SelectionChanged != null)
                            SelectionChanged(this, Selected);
                    }

                    y += 15 + 4;
                }
            }
        }

        public void Draw(Game game)
		{
            game.Batch.Scissor = Rectangle;

            game.Batch.Rectangle(Rectangle, Color.Black * 0.85f);

            Vector2 measure = game.DefaultFonts.Bold.Measure(15, Title);
            game.Batch.Text(SpriteBatch.FontStyle.Bold, 15, Title, new Vector2(Rectangle.Value.X + 8, Rectangle.Value.Y + 4), 
                Color.White * (Rectangle.Value.Contains(game.Input.MousePosition) ? 1.0f : 0.6f));

            int y = Rectangle.Value.Y + 15 + 12;
            foreach(string s in Entries)
            {
                if(s == SPACING)
                    y += 8;
                else
                {
                    measure = game.DefaultFonts.Bold.Measure(15, s);
                    Rectangle rectangle = new Rectangle(Rectangle.Value.X, y, Rectangle.Value.Width, (int)measure.Y);

                    bool hovered = rectangle.Contains(game.Input.MousePosition);

                    game.Batch.Text(SpriteBatch.FontStyle.Bold, 15, s, new Vector2(Rectangle.Value.X + 8, y),
                        Color.White * (s == SelectedValue ? 0.8f : hovered ? 0.6f : 0.4f));
                    y += 15 + 4;
                }
            }

            game.Batch.Scissor = null;
        }
	}
}