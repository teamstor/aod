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
		public string Label;
        public string[] Choices;
        public int Choice;
		public Texture2D Icon;
		public TweenedVector2 Position;
		public int Width;
		public Font Font;
		public Color TextColor = Color.White;

		public delegate void OnChoiceChanged(ChoiceField field, int newIndex, string newChoice);
		public OnChoiceChanged ChoiceChanged;
		
		public Rectangle Rectangle
		{
			get
			{
				return new Rectangle((int)Position.Value.X, (int)Position.Value.Y, Width, 32);
			}
		}

		public void Update(Game game)
		{
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
		}
	}
}