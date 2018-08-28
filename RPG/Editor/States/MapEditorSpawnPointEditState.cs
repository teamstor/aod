using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;
using TeamStor.Engine.Tween;
using TeamStor.RPG.Gameplay;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Editor.States
{
	public class MapEditorSpawnPointEditState : MapEditorModeState
	{
		public Team CurrentTeam { get; private set; } = Team.Red;
		
		public override bool PauseEditor
		{
			get { return false; }
		}
		
		public Point SelectedTile
		{
			get
			{
				Vector2 mousePos = Input.MousePosition / BaseState.Camera.Zoom;
				mousePos.X -= BaseState.Camera.Transform.Translation.X / BaseState.Camera.Zoom;
				mousePos.Y -= BaseState.Camera.Transform.Translation.Y / BaseState.Camera.Zoom;

				Point point = new Point((int)Math.Floor(mousePos.X / 16), (int)Math.Floor(mousePos.Y / 16));

				if(point.X < 0)
					point.X = 0;
				if(point.Y < 0)
					point.Y = 0;
				
				if(point.X >= BaseState.MapData.Width)
					point.X = BaseState.MapData.Width - 1;
				if(point.Y >= BaseState.MapData.Height)
					point.Y = BaseState.MapData.Height - 1;

				return point;
			}
		}

		public override void OnEnter(GameState previousState)
		{
			BaseState.Buttons.Add("redteam", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("textures/editor/spawnpoint_edit/redteam.png"),
				Position = new TweenedVector2(Game, new Vector2(48, 114 + 32 * 0)),
				
				Active = true,
				Clicked = (btn) => { CurrentTeam = Team.Red; },
				Font = Game.DefaultFonts.Normal
			});
			
			BaseState.Buttons.Add("blueteam", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("textures/editor/spawnpoint_edit/blueteam.png"),
				Position = new TweenedVector2(Game, new Vector2(48, 114 + 32 * 1)),
				
				Active = false,
				Clicked = (btn) => { CurrentTeam = Team.Blue; },
				Font = Game.DefaultFonts.Normal
			});
			
			BaseState.Buttons.Add("greenteam", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("textures/editor/spawnpoint_edit/greenteam.png"),
				Position = new TweenedVector2(Game, new Vector2(48, 114 + 32 * 2)),
				
				Active = false,
				Clicked = (btn) => { CurrentTeam = Team.Green; },
				Font = Game.DefaultFonts.Normal
			});
			
			BaseState.Buttons.Add("yellowteam", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("textures/editor/spawnpoint_edit/yellowteam.png"),
				Position = new TweenedVector2(Game, new Vector2(48, 114 + 32 * 3)),
				
				Active = false,
				Clicked = (btn) => { CurrentTeam = Team.Yellow; },
				Font = Game.DefaultFonts.Normal
			});
		}

		public override void OnLeave(GameState nextState)
		{
			BaseState.Buttons.Remove("redteam");
			BaseState.Buttons.Remove("blueteam");
			BaseState.Buttons.Remove("greenteam");
			BaseState.Buttons.Remove("yellowteam");
		}

		public override void Update(double deltaTime, double totalTime, long count)
		{
			BaseState.Buttons["redteam"].Active = CurrentTeam == Team.Red;
			BaseState.Buttons["blueteam"].Active = CurrentTeam == Team.Blue;
			BaseState.Buttons["greenteam"].Active = CurrentTeam == Team.Green;
			BaseState.Buttons["yellowteam"].Active = CurrentTeam == Team.Yellow;

			if(Input.MousePressed(MouseButton.Left) && !BaseState.IsPointObscured(Input.MousePosition))
				BaseState.MapData.SpawnPoints[CurrentTeam] = SelectedTile;
		}
		
		public override string CurrentHelpText
		{
			get
			{
				if(!BaseState.Buttons["redteam"].Active && BaseState.Buttons["redteam"].Rectangle.Contains(Input.MousePosition))
					return "Edit red team spawn";
				if(!BaseState.Buttons["blueteam"].Active && BaseState.Buttons["blueteam"].Rectangle.Contains(Input.MousePosition))
					return "Edit blue team spawn";
				if(!BaseState.Buttons["greenteam"].Active && BaseState.Buttons["greenteam"].Rectangle.Contains(Input.MousePosition))
					return "Edit green team spawn";
				if(!BaseState.Buttons["yellowteam"].Active && BaseState.Buttons["yellowteam"].Rectangle.Contains(Input.MousePosition))
					return "Edit yellow team spawn";

				return "";
			}
		}

		public override void FixedUpdate(long count)
		{
		}

		public override void Draw(SpriteBatch batch, Vector2 screenSize)
		{
			batch.Transform = BaseState.Camera.Transform;

			foreach(Team team in Enum.GetValues(typeof(Team)))
			{
				Vector2 point = new Vector2(BaseState.MapData.SpawnPoints[team].X * 16 + 8, BaseState.MapData.SpawnPoints[team].Y * 16 + 8);
				batch.Line(point, (Input.MousePosition - BaseState.Camera.Translation) / BaseState.Camera.Zoom, TeamColors.FromEnum(team) * 0.6f);
			}
			
			batch.Reset();
			batch.Circle(Input.MousePosition, 24 * BaseState.Camera.Zoom, Color.White, (int)BaseState.Camera.Zoom);
		
			batch.Transform = Matrix.CreateTranslation(BaseState.Camera.Transform.Translation);
			
			foreach(Team team in Enum.GetValues(typeof(Team)))
			{
				Vector2 point = new Vector2(BaseState.MapData.SpawnPoints[team].X * 16 + 8, BaseState.MapData.SpawnPoints[team].Y * 16 + 8);
				batch.Circle(point * BaseState.Camera.Zoom, 24 * BaseState.Camera.Zoom, TeamColors.FromEnum(team), (int)BaseState.Camera.Zoom);
			}
			
			batch.Reset();
		}
	}
}