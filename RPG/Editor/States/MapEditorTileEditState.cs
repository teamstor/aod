using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;
using TeamStor.Engine.Tween;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

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
		private Tile _lastSelection = null;

        /// <summary>
        /// The current selected layer.
        /// </summary>
        public Tile.MapLayer Layer { get; set; }

        /// <summary>
        /// The tile selection menu.
        /// </summary>
        public TileMenu Menu { get; private set; }

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

        public override void OnEnter(GameState previousState)
		{
            // Generate all font sizes for zooming
            //for(double i = 1; i <= 4; i += 0.01)
            //Game.DefaultFonts.MonoBold.Measure((uint)(8 * i), "");

            Menu = new TileMenu(BaseState.Map.Info.Environment, Game);
            Menu.Rectangle.TweenTo(new Rectangle(-250, 114, Menu.Rectangle.TargetValue.Width, Menu.Rectangle.TargetValue.Height), TweenEaseType.Linear, 0);
            Menu.Rectangle.TweenTo(new Rectangle(48, 114, Menu.Rectangle.TargetValue.Width, Menu.Rectangle.TargetValue.Height), TweenEaseType.EaseOutQuad, previousState == null ? 0.65f : 0f);

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
					Layer = Tile.MapLayer.Terrain;
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
                Disabled = false,
                Clicked = (btn) => {
                    Layer = Tile.MapLayer.Decoration;
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
                Disabled = false,
                Clicked = (btn) => { 					
					Layer = Tile.MapLayer.NPC;
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
					Layer = Tile.MapLayer.Control;
				},
				Font = Game.DefaultFonts.Normal
			});
			
			BaseState.Buttons["layer-control"].Position.TweenTo(new Vector2(48, 119 + 31 + 32 * 5), TweenEaseType.EaseOutQuad, previousState == null ? 0.65f : 0f);

        }

        public override void OnLeave(GameState nextState)
		{			
			BaseState.Buttons.Remove("tool-paintone");
			BaseState.Buttons.Remove("tool-rectangle");
			
			BaseState.Buttons.Remove("layer-terrain");
			BaseState.Buttons.Remove("layer-decoration");
			BaseState.Buttons.Remove("layer-npc");
			BaseState.Buttons.Remove("layer-control");
        }

        public override void Update(double deltaTime, double totalTime, long count)
        {
	        /* TODO if(Game.Input.KeyPressed(Keys.E))
	        {
		        _lastSelection = Menu.Selected;
		        Menu.Selected = 0;
	        }

	        if(Game.Input.KeyReleased(Keys.E))
	        {
		        Menu.Selected = _lastSelection;
		        _lastSelection = -1;
	        } */

	        if(Game.Input.KeyPressed(Keys.T))
	        {
		        MapEditorEditAttributesState state = new MapEditorEditAttributesState();
		        state.Layer = Layer;
		        BaseState.CurrentState = state;

		        return;
	        }

	        BaseState.Buttons["tool-paintone"].Active = _tool == EditTool.PaintOne;
	        BaseState.Buttons["tool-rectangle"].Active = _tool == EditTool.PaintRectangle;
	        
	        BaseState.Buttons["layer-terrain"].Active = Layer == Tile.MapLayer.Terrain;
	        BaseState.Buttons["layer-decoration"].Active = Layer == Tile.MapLayer.Decoration;
	        BaseState.Buttons["layer-npc"].Active = Layer == Tile.MapLayer.NPC;
	        BaseState.Buttons["layer-control"].Active = Layer == Tile.MapLayer.Control;

	        if(BaseState.Buttons["tool-paintone"].Position.IsComplete)
	        {
		        BaseState.Buttons["tool-paintone"].Position.TweenTo(new Vector2(48,
			        Menu.Rectangle.Value.Y +
                    Menu.Rectangle.Value.Height + 4), TweenEaseType.Linear, 0);
		        BaseState.Buttons["tool-rectangle"].Position.TweenTo(new Vector2(48,
                    Menu.Rectangle.Value.Y +
                    Menu.Rectangle.Value.Height + 4 + 32), TweenEaseType.Linear, 0);
		        
		        BaseState.Buttons["layer-terrain"].Position.TweenTo(new Vector2(48,
			        Menu.Rectangle.Value.Y +
			        Menu.Rectangle.Value.Height + 4 + 32 + 5 + 32), TweenEaseType.Linear, 0);
		        BaseState.Buttons["layer-decoration"].Position.TweenTo(new Vector2(48,
			        Menu.Rectangle.Value.Y +
			        Menu.Rectangle.Value.Height + 4 + 32 + 5 + 32 * 2), TweenEaseType.Linear, 0);
		        BaseState.Buttons["layer-npc"].Position.TweenTo(new Vector2(48,
			        Menu.Rectangle.Value.Y +
			        Menu.Rectangle.Value.Height + 4 + 32 + 5 + 32 * 3), TweenEaseType.Linear, 0);
		        BaseState.Buttons["layer-control"].Position.TweenTo(new Vector2(48,
			        Menu.Rectangle.Value.Y +
			        Menu.Rectangle.Value.Height + 4 + 32 + 5 + 32 * 4), TweenEaseType.Linear, 0);

		        float alpha = 1.0f;
		        if(Menu.Rectangle.Value.Contains(Input.MousePosition))
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
                            BaseState.Map[Menu.SelectedTile.Layer, SelectedTile.X, SelectedTile.Y] = Menu.SelectedTile;
				        break;
				        
			        case EditTool.PaintRectangle:
				        if(Input.MousePressed(MouseButton.Left))
					        _startingTile = SelectedTile;
				        if(Input.MouseReleased(MouseButton.Left))
				        {
					        for(int x = _rectangleToolRect.X; x <= _rectangleToolRect.X + _rectangleToolRect.Width; x++)
					        {
						        for(int y = _rectangleToolRect.Y; y <= _rectangleToolRect.Y + _rectangleToolRect.Height; y++)
                                    BaseState.Map[Menu.SelectedTile.Layer, x, y] = Menu.SelectedTile;
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

            Menu.Draw(Game);
			
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
							Layer == Tile.MapLayer.Terrain || Menu.SelectedTile.ID != "" ? Color.White * alpha : Color.DarkRed * alpha, 1, false);
					else
						batch.Outline(new Rectangle(SelectedTile.X * 16, SelectedTile.Y * 16, 16, 16),
							Layer == Tile.MapLayer.Terrain || Menu.SelectedTile.ID != "" || !Input.Mouse(MouseButton.Left) ? Color.White * alpha : Color.DarkRed * alpha, 1, false);

                    batch.Reset();

					if(!(_tool == EditTool.PaintRectangle && _startingTile.X != -1))
					{
						string str = "(" + SelectedTile.X + ", " + SelectedTile.Y + ") ";
						str += "[" + BaseState.Map[Layer, SelectedTile.X, SelectedTile.Y].Name(BaseState.Map.GetMetadata(Layer, SelectedTile.X, SelectedTile.Y), BaseState.Map.Info.Environment) + "]";

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