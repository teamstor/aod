using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;
using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Editor
{
	public class ScrollableTextField
	{
        private bool _isScrollingX = false;
        private bool _isScrollingY = false;
        private bool _defocusQueued = false;

        public string Label, Text;
		
		public Texture2D Icon;
		public TweenedRectangle Area;
		
		public Vector2 TotalArea;
		
		public Font Font;
		
		public bool Focused;
		public Color TextColor = Color.White;
		
		public Vector2 ScrollableAmount
		{
			get
			{
				return new Vector2(Math.Max(0, (int)TotalArea.X - Area.Value.Width), Math.Max(0, (int)TotalArea.Y - Area.Value.Height));
			}
		}
		
		public Vector2 Scroll;
		
		public delegate void OnTextChanged(ScrollableTextField field, string newText);
		public OnTextChanged TextChanged;
		
		public delegate void OnFocusChanged(ScrollableTextField field, bool focus);
		public OnFocusChanged FocusChanged;
		
		private bool _first = true;

		public void Update(Game game)
		{
			if(_first)
			{
				game.Window.TextInput += OnTextInput;
				game.OnStateChange += OnStateChange;
				_first = false;

				RecalculateTotalArea();
			}

			if(game.Input.MousePressed(MouseButton.Left) || _defocusQueued)
			{
                if(_isScrollingX || _isScrollingY)
                {
                    if(!_defocusQueued)
                        _defocusQueued = true;
                }
                else
                {
                    _defocusQueued = false;
                    bool oldFocused = Focused;
                    Focused = Area.Value.Contains(game.Input.MousePosition);

                    if(Focused != oldFocused && FocusChanged != null)
                        FocusChanged(this, Focused);
                }
			}

            if(Area.Value.Contains(game.Input.MousePosition))
            {
                if((game.Input.MouseScroll < 0 && Scroll.Y < ScrollableAmount.Y) ||
                    (game.Input.MouseScroll > 0 && Scroll.Y > 0))
                    Scroll.Y -= game.Input.MouseScroll / 4f;

                if(game.Input.Key(Keys.Up) && Scroll.Y > 0)
                    Scroll.Y = Math.Max(0, Scroll.Y - ((float)game.DeltaTime * 140f));
                if(game.Input.Key(Keys.Down) && Scroll.Y < ScrollableAmount.Y)
                    Scroll.Y = Math.Min(ScrollableAmount.Y, Scroll.Y + ((float)game.DeltaTime * 140f));
            }

            if(_isScrollingX)
            {
                float at = game.Input.MousePosition.X - (Area.Value.Left + 8);
                float percentage = at / (Area.Value.Width - 16);
                Scroll.X = percentage * ScrollableAmount.X;

                if(game.Input.MouseReleased(Engine.MouseButton.Left))
                    _isScrollingX = false;
            }


            if(_isScrollingY)
            {
                float at = game.Input.MousePosition.Y - (Area.Value.Top + 8);
                float percentage = at / (Area.Value.Height - 16);
                Scroll.Y = percentage * ScrollableAmount.Y;

                if(game.Input.MouseReleased(Engine.MouseButton.Left))
                    _isScrollingY = false;
            }

            Scroll = Vector2.Clamp(Scroll, Vector2.Zero, ScrollableAmount);
		}

		private void RecalculateTotalArea()
		{
			TotalArea = Vector2.Zero;

			if(Icon != null)
				TotalArea.X += Icon.Width + 8;

            TotalArea += Font.Measure(15, Label + Text + "|");

            TotalArea.X += 24;
			TotalArea.Y += 24;
		}
		
		private void OnStateChange(object sender, Game.ChangeStateEventArgs e)
		{
			((Game)sender).Window.TextInput -= OnTextInput;
			((Game)sender).OnStateChange -= OnStateChange;
		}

		private void OnTextInput(object sender, TextInputEventArgs e)
		{
			if(Focused)
			{
				string oldText = Text;

				if(e.Character == '\b' && Text.Length > 0)
					Text = Text.Substring(0, Text.Length - 1);
				else if(Char.IsLetterOrDigit(e.Character) || Char.IsPunctuation(e.Character) || e.Character == ' ')
					Text += e.Character;

				Text = Text.TrimStart();

				if(e.Key == Keys.Enter)
					Text += '\n';

				if(Text != oldText)
				{
                    if(TextChanged != null)
					    TextChanged(this, Text);

					RecalculateTotalArea();

                    string measureText = Text.Split('\n').Last();
                    if(Text.Split('\n').Length == 1)
                        measureText = Label + measureText;

                    Scroll.X = Math.Max(0, Font.Measure(15, measureText).X + 24 - Area.TargetValue.Width);
                    Scroll.Y = ScrollableAmount.Y;
                }
            }
		}

		public void Draw(Game game)
		{
			bool hovered = Area.Value.Contains(game.Input.MousePosition);
			SpriteBatch batch = game.Batch;

			batch.Scissor = Area.Value;

			batch.Rectangle(Area, Color.Black * 0.8f);
			
			Matrix oldTransform = game.Batch.Transform;
			game.Batch.Transform = Matrix.CreateTranslation(
				(int)-Scroll.X, 
				(int)-Scroll.Y, 
				0);

			if(Icon != null)
				batch.Texture(new Vector2(Area.Value.X + 4, Area.Value.Y + 4), Icon, TextColor * (Focused ? 1.0f : hovered ? 0.8f : 0.6f));

			batch.Text(Font, 15, 
				Label + Text + (Focused && (int)((game.Time * 4) % 2) == 0 ? "|" : ""), 
				new Vector2(Area.Value.X + (Icon != null ? Icon.Width + 8 : 8), Area.Value.Y + 6), 
				TextColor * (Focused ? 1.0f : hovered ? 0.8f : 0.6f));

			batch.Transform = oldTransform;

            if(ScrollableAmount.X > 0)
            {
                float scrollBarWidth = 1000 / ScrollableAmount.X;
                if(scrollBarWidth > Area.Value.Width - 16)
                    scrollBarWidth = Area.Value.Width - 16;
                if(scrollBarWidth < 20)
                    scrollBarWidth = 20;

                Rectangle fullScrollRectangle = new Rectangle(Area.TargetValue.X + 8, Area.TargetValue.Bottom - 18, Area.TargetValue.Width - 16, 20);

                if(fullScrollRectangle.Contains(game.Input.MousePosition) || _isScrollingX)
                {
                    fullScrollRectangle.Y = Area.TargetValue.Bottom - 10;
                    fullScrollRectangle.Height = 2;

                    game.Batch.Rectangle(fullScrollRectangle, Color.White * 0.4f);

                    if(game.Input.MousePressed(Engine.MouseButton.Left) && !_isScrollingX)
                        _isScrollingX = true;
                }

                game.Batch.Rectangle(new Rectangle(
                    (int)(MathHelper.Lerp(Area.TargetValue.X + 8, Area.TargetValue.Right - 8 - scrollBarWidth, Scroll.X / ScrollableAmount.X)),
                    Area.TargetValue.Bottom - 10,
                    (int)scrollBarWidth,
                    2),
                    Color.White * 0.5f);
            }

            if(ScrollableAmount.Y > 0)
			{
				float scrollBarHeight = 1000 / ScrollableAmount.Y;
				if(scrollBarHeight > Area.Value.Height - 16)
					scrollBarHeight = Area.Value.Height - 16;
				if(scrollBarHeight < 20)
					scrollBarHeight = 20;

				Rectangle fullScrollRectangle = new Rectangle(Area.TargetValue.Right - 18, Area.TargetValue.Y + 8, 20, Area.TargetValue.Height - 16);

				if(fullScrollRectangle.Contains(game.Input.MousePosition) || _isScrollingY)
				{
                    fullScrollRectangle.X = Area.TargetValue.Right - 10;
					fullScrollRectangle.Width = 2;

					game.Batch.Rectangle(fullScrollRectangle, Color.White * 0.4f);

                    if(game.Input.MousePressed(Engine.MouseButton.Left) && !_isScrollingY)
                        _isScrollingY = true;
                }

                game.Batch.Rectangle(new Rectangle(
					Area.TargetValue.Right - 10,
					(int)(MathHelper.Lerp(Area.TargetValue.Y + 8, Area.TargetValue.Bottom - 8 - scrollBarHeight, Scroll.Y / ScrollableAmount.Y)),
					2,
					(int)scrollBarHeight),
					Color.White * 0.5f);
			}

			batch.Scissor = null;
		}
	}
}