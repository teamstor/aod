using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDL2;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;
using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Editor
{
	public class TextField
	{
		public string Label, Text;
		public Texture2D Icon;
		public TweenedVector2 Position;
		public int Width;
		public Font Font;
		public bool Focused;
		public Color TextColor = Color.White;
        public int MaxChars = 30;
        public int Lines = 1;

		public delegate void OnTextChanged(TextField field, string newText);
		public OnTextChanged TextChanged;
		
		public delegate void OnFocusChanged(TextField field, bool focus);
		public OnFocusChanged FocusChanged;

		public Rectangle Rectangle
		{
			get
			{
				return new Rectangle((int)Position.Value.X, (int)Position.Value.Y, Width, 32 + (15 + 4) * (Lines - 1));
			}
		}

		private bool _first = true;

		public void Update(Game game)
		{
			if(_first)
			{
				TextInputEXT.TextInput += OnTextInput;
				TextInputEXT.StartTextInput();
				game.OnStateChange += OnStateChange;
				_first = false;
			}

			if(game.Input.MousePressed(MouseButton.Left))
			{
				bool oldFocused = Focused;
				Focused = Rectangle.Contains(game.Input.MousePosition);
				
				if(Focused != oldFocused && FocusChanged != null)
					FocusChanged(this, Focused);
			}
		}

		private void OnStateChange(object sender, Game.ChangeStateEventArgs e)
		{
			TextInputEXT.TextInput -= OnTextInput;
			TextInputEXT.StopTextInput();
			((Game)sender).OnStateChange -= OnStateChange;
		}

		private void OnTextInput(char c)
		{
			if(Focused)
			{
				string oldText = Text;

				if(c == (char) 22)
					Text += SDL.SDL_GetClipboardText();

                if(c == '\b' && Text.Length > 0)
                    Text = Text.Substring(0, Text.Length - 1);
                else if(Char.IsLetterOrDigit(c) || Char.IsPunctuation(c) || c == ' ')
                    Text += c;

				Text = Text.TrimStart();

                string[] split = Text.Split('\n');
                int lineCount = split.Length;

				if(c == (char)13)
				{
                    if(Lines == 1)
                    {
                        Focused = false;
                        if(FocusChanged != null)
                            FocusChanged(this, false);
                    }
                    else if(lineCount < Lines)
                        Text += '\n';
				}

                if(split[split.Length - 1].Length > MaxChars)
                {
                    split[split.Length - 1] = split[split.Length - 1].Substring(0, MaxChars);
                    Text = String.Join("\n", split);
                    if(lineCount < Lines)
                        Text += '\n';
                }

                if(Text != oldText && TextChanged != null)
					TextChanged(this, Text);
			}
		}

		public void Draw(Game game)
		{
			bool hovered = Rectangle.Contains(game.Input.MousePosition);
			SpriteBatch batch = game.Batch;

			batch.Rectangle(Rectangle, Color.Black * 0.8f);

			if(Icon != null)
				batch.Texture(new Vector2(Position.Value.X + 4, Position.Value.Y + 4), Icon, TextColor * (Focused ? 1.0f : hovered ? 0.8f : 0.6f));

			batch.Text(Font, 15, 
				Label + Text + (Focused && (int)((game.Time * 4) % 2) == 0 ? "|" : ""), 
				new Vector2(Position.Value.X + (Icon != null ? Icon.Width + 8 : 8), Position.Value.Y + 6), 
				TextColor * (Focused ? 1.0f : hovered ? 0.8f : 0.6f));
		}
	}
}