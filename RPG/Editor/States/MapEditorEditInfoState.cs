using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;
using TeamStor.Engine.Tween;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Map.Editor.States
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
			
			return new Vector2(MathHelper.Clamp(x, 1, 500), MathHelper.Clamp(y, 1, 500));
		}

		public override void OnEnter(GameState previousState)
		{
			BaseState.TextFields.Add("name", new TextField
			{
				Label = "Name: ",
				Text = BaseState.MapData.Info.Name,
				Font = Game.DefaultFonts.Bold,
				Icon = Assets.Get<Texture2D>("textures/editor/info_edit/icon_name.png"),
				Position = new TweenedVector2(Game, new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 80)),
				FocusChanged = (field, focus) =>
				{
					if(!focus) BaseState.MapData.Info.Name = field.Text.TrimStart();
				},
				Width = 300
			});
			
			BaseState.TextFields.Add("creator", new TextField
			{
				Label = "Creator: ",
				Text = BaseState.MapData.Info.Creator,
				Font = Game.DefaultFonts.Bold,
				Icon = Assets.Get<Texture2D>("textures/editor/info_edit/icon_creator.png"),
				Position = new TweenedVector2(Game, new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 80 + 36)),
				FocusChanged = (field, focus) =>
				{
					if(!focus) BaseState.MapData.Info.Creator = field.Text.TrimStart();
				},
				Width = 300
			});
			
			BaseState.TextFields.Add("size", new TextField
			{
				Label = "Size: ",
				Text = BaseState.MapData.Width + "x" + BaseState.MapData.Height,
				Font = Game.DefaultFonts.Bold,
				Icon = Assets.Get<Texture2D>("textures/editor/info_edit/icon_size.png"),
				Position = new TweenedVector2(Game, new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 80 + 36 * 2)),
				FocusChanged = (field, focus) =>
				{
					Vector2? size = ParseSize();
					if(!focus)
					{
						if(size.HasValue)
							BaseState.MapData.Resize((int)size.Value.X, (int)size.Value.Y);

						field.Text = BaseState.MapData.Width + "x" + BaseState.MapData.Height;
					}
				},
				Width = 300
			});
		}

		public override void OnLeave(GameState nextState)
		{
			BaseState.MapData.Info.Name = BaseState.TextFields["name"].Text.TrimStart();
			BaseState.MapData.Info.Creator = BaseState.TextFields["creator"].Text.TrimStart();
			
			BaseState.TextFields.Remove("name");
			BaseState.TextFields.Remove("creator");
			BaseState.TextFields.Remove("size");
		}

		public override void Update(double deltaTime, double totalTime, long count)
		{
			BaseState.TextFields["name"].Position.TweenTo(new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 80), TweenEaseType.Linear, 0);
			BaseState.TextFields["creator"].Position.TweenTo(new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 80 + 36), TweenEaseType.Linear, 0);
			BaseState.TextFields["size"].Position.TweenTo(new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - 150, Game.GraphicsDevice.Viewport.Height / 2 - 80 + 36 * 2), TweenEaseType.Linear, 0);

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