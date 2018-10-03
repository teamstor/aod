using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;
using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;
using System.Collections.Generic;

namespace TeamStor.RPG.Editor
{
	public class ChoiceField
	{
		public Game Game;
		public string Label;
        public string[] Choices;
        public int Choice;
		public Texture2D Icon;
		public TweenedVector2 Position;
		public int Width;
		public Font Font;
		public Color TextColor = Color.White;

        public int MinValue = 0;
        public int MaxValue = int.MaxValue;

		public delegate void OnChoiceChanged(ChoiceField field, int newIndex, string newChoice);
		public OnChoiceChanged ChoiceChanged;
		
		public Rectangle Rectangle
		{
			get
			{
				return new Rectangle((int)Position.Value.X, (int)Position.Value.Y, Width, 32);
			}
		}

		public Rectangle LeftButtonRectangle
		{
			get
			{
				Rectangle rect = new Rectangle((int)Position.Value.X + (Icon != null ? Icon.Width + 8 : 8) + (int)Font.Measure(15, Label).X + 12, (int)Position.Value.Y, 32, 32);

				if(rect.X < Position.Value.X + Width / 2.3f)
					rect.X = (int)(Position.Value.X + Width / 2.3f);

				return rect;
			}
		}
		
		public Rectangle RightButtonRectangle
		{
			get
			{
				return new Rectangle(Rectangle.X + Rectangle.Width - 32, (int)Position.Value.Y, 32, 32);
			}
		}

		public void Update(Game game)
		{
			if(Game.Input.MousePressed(MouseButton.Left))
			{
				if(LeftButtonRectangle.Contains(Game.Input.MousePosition))
				{
					Choice--;
					if(Choice < (Choices == null ? MinValue : 0))
						Choice = Choices == null ? MaxValue : Choices.Length - 1;

                    if(ChoiceChanged != null)
					    ChoiceChanged(this, Choice, Choices == null ? Choice.ToString() : Choices[Choice]);
				}
				
				if(RightButtonRectangle.Contains(Game.Input.MousePosition))
				{
					Choice++;
					if(Choice > (Choices == null ? MaxValue : Choices.Length - 1))
						Choice = Choices == null ? MinValue : 0;

                    if(ChoiceChanged != null)
                        ChoiceChanged(this, Choice, Choices == null ? Choice.ToString() : Choices[Choice]);
				}
			}
		}

		public void Draw(Game game)
		{
			bool hovered = Rectangle.Contains(game.Input.MousePosition);
			SpriteBatch batch = game.Batch;

			batch.Rectangle(Rectangle, Color.Black * 0.8f);

			if(Icon != null)
				batch.Texture(new Vector2(Position.Value.X + 4, Position.Value.Y + 4), Icon, TextColor * 0.6f);

			batch.Text(Font, 15, 
				Label, 
				new Vector2(Position.Value.X + (Icon != null ? Icon.Width + 8 : 8), Position.Value.Y + 6), 
				TextColor * 0.6f);
			
			batch.Texture(new Rectangle(LeftButtonRectangle.X + 4, LeftButtonRectangle.Y + 4, 24, 24), 
				Game.Assets.Get<Texture2D>("editor/arrow.png"), 
				Color.White * (LeftButtonRectangle.Contains(Game.Input.MousePosition) ? Game.Input.Mouse(MouseButton.Left) ? 1.0f : 0.8f : 0.6f), 
				null, 0, null, SpriteEffects.FlipHorizontally);
			batch.Texture(new Rectangle(RightButtonRectangle.X + 4, RightButtonRectangle.Y + 4, 24, 24), 
				Game.Assets.Get<Texture2D>("editor/arrow.png"), 
				Color.White * (RightButtonRectangle.Contains(Game.Input.MousePosition) ? Game.Input.Mouse(MouseButton.Left) ? 1.0f : 0.8f : 0.6f));

			Vector2 measure = Font.Measure(13, Choices == null ? Choice.ToString() : Choices[Choice]);
			int width = RightButtonRectangle.X - (LeftButtonRectangle.X + LeftButtonRectangle.Width);
			width -= 8;
			Rectangle rect = new Rectangle(LeftButtonRectangle.X + LeftButtonRectangle.Width + 4, LeftButtonRectangle.Y, width, 32);
			
			batch.Text(Font, 13,
                Choices == null ? Choice.ToString() : Choices[Choice], 
				new Vector2(rect.X + rect.Width / 2 - measure.X / 2, Position.Value.Y + 7), 
				TextColor * 0.6f);
		}
	}
}