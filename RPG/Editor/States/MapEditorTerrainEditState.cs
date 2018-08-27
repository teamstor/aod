using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;
using TeamStor.Engine.Tween;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;
using System.Collections.Generic;
using System.Linq;

namespace TeamStor.RPG.Map.Editor.States
{
    public enum TerrainTool
    {
        PaintOne,
        PaintRadius,
        PaintRectangle
    }

	public class MapEditorTerrainEditState : MapEditorModeState
	{
		private TerrainTool _tool;
        private bool _decorationLayer;
		
		private float _radius = 4;

		private Point _startingTile = new Point(-1, -1);

		private Rectangle _rectangleToolRect
		{
			get
			{
				Point startPos = new Point(Math.Min(_startingTile.X, SelectedTile.X), Math.Min(_startingTile.Y, SelectedTile.Y));
				Point endPos = new Point(Math.Max(_startingTile.X, SelectedTile.X), Math.Max(_startingTile.Y, SelectedTile.Y));
				
				return new Rectangle(startPos.X, startPos.Y, endPos.X - startPos.X, endPos.Y - startPos.Y);
			}
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
		
		public override bool PauseEditor
		{
			get { return false; }
		}

        private void UpdateSelectTileMenu(bool doTween = false)
        {
            if(BaseState.SelectionMenus.ContainsKey("select-tile-menu"))
                BaseState.SelectionMenus.Remove("select-tile-menu");

            List<string> tiles = new List<string>();
            foreach(TerrainTile tile in TerrainTile.Tiles.Values.Where((t) => t.Decoration == _decorationLayer))
                tiles.Add(tile.Name);

            BaseState.SelectionMenus.Add("select-tile-menu", new SelectionMenu
            {
                Title = "Tiles",
                Entries = tiles,
                Rectangle = new TweenedRectangle(Game, new Rectangle(-250, 114, 210, 15 + 12))
            });

            BaseState.SelectionMenus["select-tile-menu"].Rectangle.TweenTo(new Rectangle(48, 114, 210, 15 + 12), TweenEaseType.EaseOutQuad, doTween ? 0.65f : 0f);

        }

        public override void OnEnter(GameState previousState)
		{
            UpdateSelectTileMenu(previousState == null);
			
			BaseState.Buttons.Add("tool-paintone", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("textures/editor/terrain_edit/icon_paintone.png"),
				Position = new TweenedVector2(Game, new Vector2(-250, 114 + 31)),
				
				Active = true,
				Clicked = (btn) => { _tool = TerrainTool.PaintOne; },
				Font = Game.DefaultFonts.Normal
			});
			
			BaseState.Buttons["tool-paintone"].Position.TweenTo(new Vector2(48, 114 + 31), TweenEaseType.EaseOutQuad, previousState == null ? 0.65f : 0f);
			
			BaseState.Buttons.Add("tool-rectangle", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("textures/editor/terrain_edit/icon_rectangle.png"),
				Position = new TweenedVector2(Game, new Vector2(-250, 114 + 31 + 32)),
				
				Active = false,
				Clicked = (btn) => { _tool = TerrainTool.PaintRectangle; },
				Font = Game.DefaultFonts.Normal
			});
			
			BaseState.Buttons["tool-rectangle"].Position.TweenTo(new Vector2(48, 114 + 31 + 32), TweenEaseType.EaseOutQuad, previousState == null ? 0.65f : 0f);

            BaseState.Buttons.Add("change-layer", new Button
            {
                Text = "",
                Icon = Assets.Get<Texture2D>("textures/editor/terrain_edit/icon_terrainlayer.png"),
                Position = new TweenedVector2(Game, new Vector2(-250, 118 + 31 + 32 * 2)),

                Active = false,
                Clicked = (btn) => 
                {
                    _decorationLayer = !_decorationLayer;
                    UpdateSelectTileMenu();
                },
                Font = Game.DefaultFonts.Normal
            });

            BaseState.Buttons["change-layer"].Position.TweenTo(new Vector2(48, 118 + 31 + 32 * 2), TweenEaseType.EaseOutQuad, previousState == null ? 0.65f : 0f);
        }

        public override void OnLeave(GameState nextState)
		{
            BaseState.SelectionMenus.Remove("select-tile-menu");
			
			BaseState.Buttons.Remove("tool-paintone");
			BaseState.Buttons.Remove("tool-rectangle");
            BaseState.Buttons.Remove("change-layer");
        }

        public override void Update(double deltaTime, double totalTime, long count)
        {
            BaseState.SelectionMenus["select-tile-menu"].Title = "Tiles (selected: " + BaseState.SelectionMenus["select-tile-menu"].SelectedValue + ")";
	        
	        BaseState.Buttons["tool-paintone"].Active = _tool == TerrainTool.PaintOne;
	        BaseState.Buttons["tool-rectangle"].Active = _tool == TerrainTool.PaintRectangle;

            BaseState.Buttons["change-layer"].Icon = Assets.Get<Texture2D>("textures/editor/terrain_edit/icon_" + (_decorationLayer ? "decorationlayer" : "terrainlayer") + ".png");

	        if(BaseState.Buttons["tool-paintone"].Position.IsComplete)
	        {
		        BaseState.Buttons["tool-paintone"].Position.TweenTo(new Vector2(48,
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Y +
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Height + 4), TweenEaseType.Linear, 0);
		        BaseState.Buttons["tool-rectangle"].Position.TweenTo(new Vector2(48,
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Y +
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Height + 4 + 32), TweenEaseType.Linear, 0);
                BaseState.Buttons["change-layer"].Position.TweenTo(new Vector2(48,
                    BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Y +
                    BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Height + 8 + 32 * 2), TweenEaseType.Linear, 0);
            }

            if(!BaseState.IsPointObscured(Input.MousePosition))
	        {
		        switch(_tool)
		        {
			        case TerrainTool.PaintOne:
				        if(Input.Mouse(MouseButton.Left))
							BaseState.MapData.SetTileIdAt(_decorationLayer, SelectedTile.X, SelectedTile.Y,
								TerrainTile.FindByName(BaseState.SelectionMenus["select-tile-menu"].SelectedValue).Id);
				        break;
				        
			        case TerrainTool.PaintRectangle:
				        if(Input.MousePressed(MouseButton.Left))
					        _startingTile = SelectedTile;
				        if(Input.MouseReleased(MouseButton.Left))
				        {
					        for(int x = _rectangleToolRect.X; x <= _rectangleToolRect.X + _rectangleToolRect.Width; x++)
					        {
						        for(int y = _rectangleToolRect.Y; y <= _rectangleToolRect.Y + _rectangleToolRect.Height; y++)
							        BaseState.MapData.SetTileIdAt(_decorationLayer, x, y,
								        TerrainTile.FindByName(BaseState.SelectionMenus["select-tile-menu"].SelectedValue).Id);
					        }

					        _startingTile = new Point(-1, -1);
				        }

				        break;
		        }
	        }
        }

        public override void FixedUpdate(long count)
		{
		}

		public override string CurrentHelpText
		{
			get
			{
				if(!BaseState.Buttons["tool-paintone"].Active && BaseState.Buttons["tool-paintone"].Rectangle.Contains(Input.MousePosition))
					return "Place tiles";
				if(!BaseState.Buttons["tool-rectangle"].Active && BaseState.Buttons["tool-rectangle"].Rectangle.Contains(Input.MousePosition))
					return "Place in rectangle";
                if(BaseState.Buttons["change-layer"].Rectangle.Contains(Input.MousePosition))
                    return _decorationLayer ? "Click to use terrain layer" : "Click to use decoration layer";

				return "";
			}
		}

		public override void Draw(SpriteBatch batch, Vector2 screenSize)
		{
			if(!BaseState.IsPointObscured(Input.MousePosition))
			{
				batch.SamplerState = SamplerState.PointWrap;
				batch.Transform = BaseState.Camera.Transform;

				if(_tool == TerrainTool.PaintRadius)
				{
					batch.Reset();
					batch.Circle(Input.MousePosition, _radius * 4f * BaseState.Camera.Zoom, Color.White, 2);
				}
				else
				{
					float alpha = Input.Mouse(MouseButton.Left) ? 1.0f : 0.6f;

					if(Input.Mouse(MouseButton.Left))
						alpha = MathHelper.Clamp(alpha + (float)Math.Sin(Game.Time * 10f) * 0.4f, 0, 1);
							
					if(Input.Mouse(MouseButton.Left) && _tool == TerrainTool.PaintRectangle)
						batch.Outline(new Rectangle(_rectangleToolRect.X * 16, _rectangleToolRect.Y * 16, _rectangleToolRect.Width * 16 + 16, _rectangleToolRect.Height * 16 + 16),
							Color.White * alpha, 1, false);
					else
						batch.Outline(new Rectangle(SelectedTile.X * 16, SelectedTile.Y * 16, 16, 16),
							Color.White * alpha, 1, false);

					batch.Reset();

					if(_tool == TerrainTool.PaintOne)
						batch.Text(
							SpriteBatch.FontStyle.MonoBold,
							(uint)(8 * BaseState.Camera.Zoom),
							"(" + SelectedTile.X + ", " + SelectedTile.Y + ")",
							new Vector2(SelectedTile.X * 16, SelectedTile.Y * 16) * BaseState.Camera.Zoom + BaseState.Camera.Translation -
							new Vector2(0, 12 * BaseState.Camera.Zoom),
							Color.White * alpha);
				}
			}
		}
	}
}