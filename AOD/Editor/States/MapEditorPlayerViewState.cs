using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Editor.States
{
	public class MapEditorPlayerViewState : MapEditorModeState
	{
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
				
				if(point.X >= BaseState.Map.Width)
					point.X = BaseState.Map.Width - 1;
				if(point.Y >= BaseState.Map.Height)
					point.Y = BaseState.Map.Height - 1;

				return point;
			}
		}
		
		public override void OnEnter(GameState previousState)
		{
		}

		public override void OnLeave(GameState nextState)
		{
		}

		public override void Update(double deltaTime, double totalTime, long count)
		{
		}

		public override void FixedUpdate(long count)
		{
		}

		public override void Draw(SpriteBatch batch, Vector2 screenSize)
		{
			batch.Transform = BaseState.Camera.Transform;
			batch.SamplerState = SamplerState.PointClamp;
			
			Rectangle rect = new Rectangle(SelectedTile.X * 16 - 480 / 2, SelectedTile.Y * 16 - 270 / 2, 480, 270);
			batch.Outline(rect, Color.Blue * 0.8f);
			batch.Rectangle(rect, Color.Blue * 0.2f);
			
			batch.Texture(new Vector2(SelectedTile.X * 16, SelectedTile.Y * 16 - 16), Assets.Get<Texture2D>("player/front0.png"), Color.White * 0.6f);
			
			batch.Reset();
		}

		public override bool PauseEditor
		{
			get { return false; }
		}
	}
}