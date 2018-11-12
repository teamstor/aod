using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TeamStor.Engine;
using TeamStor.Engine.Tween;
using System.Linq;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Editor.States
{
	public class MapEditorEditInfoState : MapEditorModeState
	{
		public override bool PauseEditor
		{
			get { return true; }
		}

		private Vector2? ParseSize()
		{
			string[] text = BaseState.TextFields["size"].Text.ToLowerInvariant().Split('x');
			if(text.Length != 2)
				return null;

			int x = 0;
			int y = 0;
			
			if(!int.TryParse(text[0], out x))
				return null;
			
			if(!int.TryParse(text[1], out y))
				return null;
			
			return new Vector2(MathHelper.Clamp(x, 1, 256), MathHelper.Clamp(y, 1, 256));
		}

		public override void OnEnter(GameState previousState)
		{
			BaseState.TextFields.Add("name", new TextField
			{
				Label = "Name: ",
				Text = BaseState.Map.Info.Name,
				Font = Game.DefaultFonts.Bold,
				Icon = Assets.Get<Texture2D>("editor/info/name.png"),
				Position = new TweenedVector2(Game, new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 72)),
				FocusChanged = (field, focus) =>
				{
					if(!focus) BaseState.Map.Info.Name = field.Text.TrimStart();
				},
				Width = 300
			});
			
			BaseState.TextFields.Add("size", new TextField
			{
				Label = "Size: ",
				Text = BaseState.Map.Width + "x" + BaseState.Map.Height,
				Font = Game.DefaultFonts.Bold,
				Icon = Assets.Get<Texture2D>("editor/info/size.png"),
				Position = new TweenedVector2(Game, new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 72 + 36)),
				FocusChanged = (field, focus) =>
				{
					Vector2? size = ParseSize();
					if(!focus)
					{
						if(size.HasValue)
							BaseState.Map.Resize((int)size.Value.X, (int)size.Value.Y);

						field.Text = BaseState.Map.Width + "x" + BaseState.Map.Height;
					}
				},
				Width = 300
			});

            BaseState.ChoiceFields.Add("environment", new ChoiceField
            {
	            Game = Game,
                Label = "Environment",
                Choices = Enum.GetNames(typeof(Map.Environment)),
                Choice = Enum.GetValues(typeof(Map.Environment)).Cast<Map.Environment>().ToList().IndexOf(BaseState.Map.Info.Environment),
                Font = Game.DefaultFonts.Bold,
                Icon = Assets.Get<Texture2D>("editor/info/environment.png"),
                Position = new TweenedVector2(Game, new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 72 + 36 * 2)),
                ChoiceChanged = (field, newIndex, newChoice) =>
                {
                    Enum.TryParse(newChoice, out BaseState.Map.Info.Environment);
                },
                Width = 300
            });

            BaseState.ChoiceFields.Add("weather", new ChoiceField
            {
	            Game = Game,
                Label = "Weather",
                Choices = Enum.GetNames(typeof(Map.Weather)),
                Choice = (int)BaseState.Map.Info.Weather,
                Font = Game.DefaultFonts.Bold,
                Icon = Assets.Get<Texture2D>("editor/info/weather.png"),
                Position = new TweenedVector2(Game, new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 72 + 36 * 3)),
                ChoiceChanged = (field, newIndex, newChoice) =>
                {
                    Enum.TryParse(newChoice, out BaseState.Map.Info.Weather);
                },
                Width = 300
            });
        }

		public override void OnLeave(GameState nextState)
		{
			BaseState.Map.Info.Name = BaseState.TextFields["name"].Text.TrimStart();
			
			BaseState.TextFields.Remove("name");
			BaseState.TextFields.Remove("size");

			BaseState.ChoiceFields.Remove("environment");
			BaseState.ChoiceFields.Remove("weather");
		}

		public override void Update(double deltaTime, double totalTime, long count)
		{
			BaseState.TextFields["name"].Position.TweenTo(new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 72), TweenEaseType.Linear, 0);
			BaseState.TextFields["size"].Position.TweenTo(new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 72 + 36), TweenEaseType.Linear, 0);
            BaseState.ChoiceFields["environment"].Position.TweenTo(new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 72 + 36 * 2), TweenEaseType.Linear, 0);
            BaseState.ChoiceFields["weather"].Position.TweenTo(new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 72 + 36 * 3), TweenEaseType.Linear, 0);

            BaseState.TextFields["size"].TextColor = ParseSize().HasValue ? Color.White : Color.DarkRed;
		}

		public override void FixedUpdate(long count)
		{
		}

		public override void Draw(SpriteBatch batch, Vector2 screenSize)
		{
			batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black * 0.6f);
		}
	}
}