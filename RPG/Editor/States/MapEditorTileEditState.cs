using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;
using TeamStor.Engine.Tween;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;
using System.Collections.Generic;
using System.Linq;

namespace TeamStor.RPG.Editor.States
{
    public enum EditTool
    {
        PaintOne,
        PaintRadius,
        PaintRectangle
    }

	public class MapEditorTileEditState : MapEditorModeState
	{
		private EditTool _tool = EditTool.PaintOne;
		private Tile.MapLayer _layer = Tile.MapLayer.Terrain;
		private int _lastSelection = -1;

        /// <summary>
        /// The current selected layer.
        /// </summary>
        public Tile.MapLayer Layer
        {
            get
            {
                return _layer;
            }
        }
		
		private float _radius = 4;

		private Point _startingTile = new Point(-1, -1);

		private Rectangle _rectangleToolRect
		{
			get
			{
				Point startPos = new Point(Math.Min(_startingTile.X, SelectedTile.X), Math.Min(_startingTile.Y, SelectedTile.Y));
				Point endPos = new Point(Math.Max(_startingTile.X, SelectedTile.X), Math.Max(_startingTile.Y, SelectedTile.Y));
				
				Rectangle rect = new Rectangle(startPos.X, startPos.Y, endPos.X - startPos.X, endPos.Y - startPos.Y);

				if(rect.X < 0)
					rect.X = 0;
				if(rect.Y < 0)
					rect.Y = 0;

				while(rect.Right >= BaseState.Map.Width)
					rect.Width--;
				while(rect.Bottom >= BaseState.Map.Height)
					rect.Height--;

				return rect;
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
				
				if(point.X >= BaseState.Map.Width)
					point.X = BaseState.Map.Width - 1;
				if(point.Y >= BaseState.Map.Height)
					point.Y = BaseState.Map.Height - 1;

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
            foreach(Tile tile in Tile.Values(_layer))
            {
                if(tile.Filter(BaseState.Map.Info.Environment))
                {
                    if(tiles.Count > 0 && tile.ID - Tile.FindByName(tiles[tiles.Count - 1], _layer).ID > 1)
                        tiles.Add(SelectionMenu.SPACING);
                    tiles.Add(tile.Name());
                }
            }

            BaseState.SelectionMenus.Add("select-tile-menu", new SelectionMenu
            {
                Title = "Tiles",
                Entries = tiles,
                Rectangle = new TweenedRectangle(Game, new Rectangle(-250, 114, 210, 15 + 12)),
	            SelectionChanged = (menu, selected) =>
	            {
		            BaseState.SelectionMenus["select-tile-menu"].Title = "Tiles [" + BaseState.SelectionMenus["select-tile-menu"].SelectedValue + "]";
	            }
            });

            BaseState.SelectionMenus["select-tile-menu"].Rectangle.TweenTo(new Rectangle(48, 114, 210, 15 + 12), TweenEaseType.EaseOutQuad, doTween ? 0.65f : 0f);
	        BaseState.SelectionMenus["select-tile-menu"].Title = "Tiles [" + BaseState.SelectionMenus["select-tile-menu"].SelectedValue + "]";
        }

        public override void OnEnter(GameState previousState)
		{
            UpdateSelectTileMenu(previousState == null);
			
			BaseState.Buttons.Add("tool-paintone", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("editor/tile/paintone.png"),
				Position = new TweenedVector2(Game, new Vector2(-250, 114 + 31)),
				
				Active = true,
				Clicked = (btn) => { _tool = EditTool.PaintOne; },
				Font = Game.DefaultFonts.Normal
			});
			
			BaseState.Buttons["tool-paintone"].Position.TweenTo(new Vector2(48, 114 + 31), TweenEaseType.EaseOutQuad, previousState == null ? 0.65f : 0f);
			
			BaseState.Buttons.Add("tool-rectangle", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("editor/tile/rectangle.png"),
				Position = new TweenedVector2(Game, new Vector2(-250, 114 + 31 + 32)),
				
				Active = false,
				Clicked = (btn) => { _tool = EditTool.PaintRectangle; },
				Font = Game.DefaultFonts.Normal
			});
			
			BaseState.Buttons["tool-rectangle"].Position.TweenTo(new Vector2(48, 114 + 31 + 32), TweenEaseType.EaseOutQuad, previousState == null ? 0.65f : 0f);
			
			BaseState.Buttons.Add("layer-terrain", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("editor/tile/terrain.png"),
				Position = new TweenedVector2(Game, new Vector2(-250, 119 + 31 + 32 * 2)),
				
				Active = true,
				Clicked = (btn) => {
					_layer = Tile.MapLayer.Terrain;
					UpdateSelectTileMenu();
				},
				Font = Game.DefaultFonts.Normal
			});
			
			BaseState.Buttons["layer-terrain"].Position.TweenTo(new Vector2(48, 119 + 31 + 32 * 2), TweenEaseType.EaseOutQuad, previousState == null ? 0.65f : 0f);
			
			BaseState.Buttons.Add("layer-decoration", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("editor/tile/decoration.png"),
				Position = new TweenedVector2(Game, new Vector2(-250, 119 + 31 + 32 * 3)),
				
				Active = false,
				Clicked = (btn) => { 					
					_layer = Tile.MapLayer.Decoration;
					UpdateSelectTileMenu();
				},
				Font = Game.DefaultFonts.Normal
			});
			
			BaseState.Buttons["layer-decoration"].Position.TweenTo(new Vector2(48, 119 + 31 + 32 * 3), TweenEaseType.EaseOutQuad, previousState == null ? 0.65f : 0f);

			BaseState.Buttons.Add("layer-npc", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("editor/tile/npc.png"),
				Position = new TweenedVector2(Game, new Vector2(-250, 119 + 31 + 32 * 4)),
				
				Active = false,
				Clicked = (btn) => { 					
					_layer = Tile.MapLayer.NPC;
					UpdateSelectTileMenu();
				},
				Font = Game.DefaultFonts.Normal
			});
			
			BaseState.Buttons["layer-npc"].Position.TweenTo(new Vector2(48, 119 + 31 + 32 * 4), TweenEaseType.EaseOutQuad, previousState == null ? 0.65f : 0f);
			
			BaseState.Buttons.Add("layer-control", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("editor/tile/control.png"),
				Position = new TweenedVector2(Game, new Vector2(-250, 119 + 31 + 32 * 5)),
				
				Active = false,
				Clicked = (btn) => { 					
					_layer = Tile.MapLayer.Control;
					UpdateSelectTileMenu();
				},
				Font = Game.DefaultFonts.Normal
			});
			
			BaseState.Buttons["layer-control"].Position.TweenTo(new Vector2(48, 119 + 31 + 32 * 5), TweenEaseType.EaseOutQuad, previousState == null ? 0.65f : 0f);

        }

        public override void OnLeave(GameState nextState)
		{
            BaseState.SelectionMenus.Remove("select-tile-menu");
			
			BaseState.Buttons.Remove("tool-paintone");
			BaseState.Buttons.Remove("tool-rectangle");
			
			BaseState.Buttons.Remove("layer-terrain");
			BaseState.Buttons.Remove("layer-decoration");
			BaseState.Buttons.Remove("layer-npc");
			BaseState.Buttons.Remove("layer-control");
        }

        public override void Update(double deltaTime, double totalTime, long count)
        {
	        if(Game.Input.KeyPressed(Microsoft.Xna.Framework.Input.Keys.E))
	        {
		        _lastSelection = BaseState.SelectionMenus["select-tile-menu"].Selected;
		        BaseState.SelectionMenus["select-tile-menu"].Selected = 0;
		        BaseState.SelectionMenus["select-tile-menu"].Title = _layer != Tile.MapLayer.Terrain ? "Tiles [erase mode]" : "Tiles [" + BaseState.SelectionMenus["select-tile-menu"].SelectedValue + "]";
	        }

	        if(Game.Input.KeyReleased(Microsoft.Xna.Framework.Input.Keys.E))
	        {
		        BaseState.SelectionMenus["select-tile-menu"].Selected = _lastSelection;
		        _lastSelection = -1;
		        BaseState.SelectionMenus["select-tile-menu"].Title = "Tiles [" + BaseState.SelectionMenus["select-tile-menu"].SelectedValue + "]";
	        }

	        BaseState.Buttons["tool-paintone"].Active = _tool == EditTool.PaintOne;
	        BaseState.Buttons["tool-rectangle"].Active = _tool == EditTool.PaintRectangle;
	        
	        BaseState.Buttons["layer-terrain"].Active = _layer == Tile.MapLayer.Terrain;
	        BaseState.Buttons["layer-decoration"].Active = _layer == Tile.MapLayer.Decoration;
	        BaseState.Buttons["layer-npc"].Active = _layer == Tile.MapLayer.NPC;
	        BaseState.Buttons["layer-control"].Active = _layer == Tile.MapLayer.Control;

	        if(BaseState.Buttons["tool-paintone"].Position.IsComplete)
	        {
		        BaseState.Buttons["tool-paintone"].Position.TweenTo(new Vector2(48,
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Y +
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Height + 4), TweenEaseType.Linear, 0);
		        BaseState.Buttons["tool-rectangle"].Position.TweenTo(new Vector2(48,
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Y +
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Height + 4 + 32), TweenEaseType.Linear, 0);
		        
		        BaseState.Buttons["layer-terrain"].Position.TweenTo(new Vector2(48,
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Y +
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Height + 4 + 32 + 5 + 32), TweenEaseType.Linear, 0);
		        BaseState.Buttons["layer-decoration"].Position.TweenTo(new Vector2(48,
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Y +
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Height + 4 + 32 + 5 + 32 * 2), TweenEaseType.Linear, 0);
		        BaseState.Buttons["layer-npc"].Position.TweenTo(new Vector2(48,
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Y +
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Height + 4 + 32 + 5 + 32 * 3), TweenEaseType.Linear, 0);
		        BaseState.Buttons["layer-control"].Position.TweenTo(new Vector2(48,
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Y +
			        BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Height + 4 + 32 + 5 + 32 * 4), TweenEaseType.Linear, 0);

		        float alpha = 1.0f;
		        if(BaseState.SelectionMenus["select-tile-menu"].Rectangle.Value.Contains(Input.MousePosition))
			        alpha = 0.0f;

		        BaseState.Buttons["tool-paintone"].Alpha = MathHelper.Lerp(BaseState.Buttons["tool-paintone"].Alpha, alpha, (float)deltaTime * 25f);
		        BaseState.Buttons["tool-rectangle"].Alpha = MathHelper.Lerp(BaseState.Buttons["tool-rectangle"].Alpha, alpha, (float)deltaTime * 25f);

		        BaseState.Buttons["layer-terrain"].Alpha = MathHelper.Lerp(BaseState.Buttons["layer-terrain"].Alpha, alpha, (float)deltaTime * 25f);
		        BaseState.Buttons["layer-decoration"].Alpha = MathHelper.Lerp(BaseState.Buttons["layer-decoration"].Alpha, alpha, (float)deltaTime * 25f);
		        BaseState.Buttons["layer-npc"].Alpha = MathHelper.Lerp(BaseState.Buttons["layer-npc"].Alpha, alpha, (float)deltaTime * 25f);
		        BaseState.Buttons["layer-control"].Alpha = MathHelper.Lerp(BaseState.Buttons["layer-control"].Alpha, alpha, (float)deltaTime * 25f);
	        }

            if(!BaseState.IsPointObscured(Input.MousePosition))
	        {
		        switch(_tool)
		        {
			        case EditTool.PaintOne:
				        if(Input.Mouse(MouseButton.Left))
							BaseState.Map[_layer, SelectedTile.X, SelectedTile.Y] = Tile.FindByName(BaseState.SelectionMenus["select-tile-menu"].SelectedValue, _layer).ID;
				        break;
				        
			        case EditTool.PaintRectangle:
				        if(Input.MousePressed(MouseButton.Left))
					        _startingTile = SelectedTile;
				        if(Input.MouseReleased(MouseButton.Left))
				        {
					        for(int x = _rectangleToolRect.X; x <= _rectangleToolRect.X + _rectangleToolRect.Width; x++)
					        {
						        for(int y = _rectangleToolRect.Y; y <= _rectangleToolRect.Y + _rectangleToolRect.Height; y++)
                                    BaseState.Map[_layer, x, y] = Tile.FindByName(BaseState.SelectionMenus["select-tile-menu"].SelectedValue, _layer).ID;
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
				
				if(!BaseState.Buttons["layer-terrain"].Active && BaseState.Buttons["layer-terrain"].Rectangle.Contains(Input.MousePosition))
					return "Select the terrain layer";
				if(!BaseState.Buttons["layer-decoration"].Active && BaseState.Buttons["layer-decoration"].Rectangle.Contains(Input.MousePosition))
					return "Select the decoration layer";
				if(!BaseState.Buttons["layer-npc"].Active && BaseState.Buttons["layer-npc"].Rectangle.Contains(Input.MousePosition))
					return "Select the NPC layer";
				if(!BaseState.Buttons["layer-control"].Active && BaseState.Buttons["layer-control"].Rectangle.Contains(Input.MousePosition))
					return "Select the control layer";

				return "";
			}
		}

		public override void Draw(SpriteBatch batch, Vector2 screenSize)
		{
			batch.SamplerState = SamplerState.PointWrap;
			
			if(!BaseState.IsPointObscured(Input.MousePosition))
			{
				batch.Transform = BaseState.Camera.Transform;

				if(_tool == EditTool.PaintRadius)
				{
					batch.Reset();
					batch.Circle(Input.MousePosition, _radius * 4f * BaseState.Camera.Zoom, Color.White, 2);
				}
				else
				{
					float alpha = Input.Mouse(MouseButton.Left) ? 1.0f : 0.6f;

					if(Input.Mouse(MouseButton.Left))
						alpha = MathHelper.Clamp(alpha + (float)Math.Sin(Game.Time * 10f) * 0.4f, 0, 1);

					if(Input.Mouse(MouseButton.Left) && _tool == EditTool.PaintRectangle)
						batch.Outline(new Rectangle(_rectangleToolRect.X * 16, _rectangleToolRect.Y * 16, _rectangleToolRect.Width * 16 + 16, _rectangleToolRect.Height * 16 + 16),
							_layer == Tile.MapLayer.Terrain || BaseState.SelectionMenus["select-tile-menu"].Selected != 0 ? Color.White * alpha : Color.DarkRed * alpha, 1, false);
					else
						batch.Outline(new Rectangle(SelectedTile.X * 16, SelectedTile.Y * 16, 16, 16),
							_layer == Tile.MapLayer.Terrain || BaseState.SelectionMenus["select-tile-menu"].Selected != 0 || !Input.Mouse(MouseButton.Left) ? Color.White * alpha : Color.DarkRed * alpha, 1, false);

                    batch.Reset();

					if(!(_tool == EditTool.PaintRectangle && _startingTile.X != -1))
					{
						string str = "(" + SelectedTile.X + ", " + SelectedTile.Y + ") ";
						str += "[" + Tile.Find(BaseState.Map[_layer, SelectedTile.X, SelectedTile.Y], _layer).Name(BaseState.Map.GetMetadata(_layer, SelectedTile.X, SelectedTile.Y)) + "]";

						Vector2 pos = new Vector2(SelectedTile.X * 16, SelectedTile.Y * 16) * BaseState.Camera.Zoom + BaseState.Camera.Translation -
						              new Vector2(0, 12 * BaseState.Camera.Zoom);
						
						if(pos.X < 5 * BaseState.Camera.Zoom)
							pos.X = 5 * BaseState.Camera.Zoom;

						if(pos.Y < 5 * BaseState.Camera.Zoom)
						{
							pos.Y = 5 * BaseState.Camera.Zoom;
							pos.X += 3 * BaseState.Camera.Zoom;
						}
						
						batch.Text(
							SpriteBatch.FontStyle.MonoBold,
							(uint) (8 * BaseState.Camera.Zoom),
							str,
							pos,
							Color.White * alpha);
					}
					else
					{
						string str = "(" + _startingTile.X + " -> " + SelectedTile.X + ", " + _startingTile.Y + " -> " + SelectedTile.Y + ")";
						
						int tilesX = Math.Abs(SelectedTile.X - _startingTile.X) + 1;
						int tilesY = Math.Abs(SelectedTile.Y - _startingTile.Y) + 1;
						str += " [" + (tilesX * tilesY) + " tile(s)]";

						Vector2 pos = new Vector2(Math.Min(_startingTile.X, SelectedTile.X) * 16, Math.Min(_startingTile.Y, SelectedTile.Y) * 16) * BaseState.Camera.Zoom + BaseState.Camera.Translation -
						              new Vector2(0, 12 * BaseState.Camera.Zoom);

						if(pos.X < 5 * BaseState.Camera.Zoom)
							pos.X = 5 * BaseState.Camera.Zoom;

						if(pos.Y < 5 * BaseState.Camera.Zoom)
						{
							pos.Y = 5 * BaseState.Camera.Zoom;
							pos.X += 3 * BaseState.Camera.Zoom;
						}

						batch.Text(
							SpriteBatch.FontStyle.MonoBold,
							(uint)(8 * BaseState.Camera.Zoom),
							str,
							pos,
							Color.White * alpha);

                        Vector2 measure = Game.DefaultFonts.MonoBold.Measure(7, (_rectangleToolRect.Width + 1) + "x" + (_rectangleToolRect.Height + 1));
                        Vector2 sizePos = new Vector2((_rectangleToolRect.Width * 16 + 16) / 2 - measure.X / 2, (_rectangleToolRect.Height * 16 + 16) / 2 - measure.Y / 2);
                        sizePos += _rectangleToolRect.Location.ToVector2() * 16;
                        sizePos -= new Vector2(0, 1);
                        sizePos *= BaseState.Camera.Zoom;
                        sizePos += BaseState.Camera.Translation;

                        batch.Text(
                            SpriteBatch.FontStyle.MonoBold,
                            (uint)(7 * BaseState.Camera.Zoom),
                            (_rectangleToolRect.Width + 1) + "x" + (_rectangleToolRect.Height + 1),
                            sizePos,
                            Color.White * alpha * 0.6f);
                    }
                }
			}
		}
	}
}