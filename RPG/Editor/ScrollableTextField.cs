using System;
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

			if(game.Input.MousePressed(MouseButton.Left))
			{
				bool oldFocused = Focused;
				Focused = Area.Value.Contains(game.Input.MousePosition);
				
				if(Focused != oldFocused && FocusChanged != null)
					FocusChanged(this, Focused);
			}
			
			Scroll = Vector2.Clamp(Scroll, Vector2.Zero, ScrollableAmount);
		}

		private void RecalculateTotalArea()
		{
			TotalArea = Vector2.Zero;

			if(Icon != null)
				TotalArea.X += Icon.Width + 8;

			TotalArea.X += Font.Measure(15, Label + Text).X;
			TotalArea.Y += Font.Measure(15, Label + Text).Y;

			TotalArea.X += 8;
			TotalArea.Y += 8;
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

				string[] split = Text.Split('\n');
				int lineCount = split.Length;

				if(e.Key == Keys.Enter)
					Text += '\n';

				if(Text != oldText && TextChanged != null)
				{
					TextChanged(this, Text);
					RecalculateTotalArea();
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
			
			if(ScrollableAmount.Y > 0)
			{
				float scrollBarHeight = 1000 / ScrollableAmount.Y;
				if(scrollBarHeight > Area.Value.Height - 16)
					scrollBarHeight = Area.Value.Height - 16;
				if(scrollBarHeight < 20)
					scrollBarHeight = 20;

				Rectangle fullScrollRectangle = new Rectangle(Area.TargetValue.Right - 18, Area.TargetValue.Y + 8, 20, Area.TargetValue.Height - 16);

				if(fullScrollRectangle.Contains(game.Input.MousePosition))
				{
					fullScrollRectangle.X = Area.TargetValue.Right - 10;
					fullScrollRectangle.Width = 2;

					game.Batch.Rectangle(fullScrollRectangle, Color.White * 0.4f);
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