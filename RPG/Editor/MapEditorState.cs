using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;
using TeamStor.RPG.Editor.States;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using static TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Editor
{
	public class MapEditorState : GameState
	{
		private enum EditMode
		{
			Tiles,
            Attributes,
			Info,
			Keybinds
		}
		
        private TweenedDouble _topTextY;
        private TweenedDouble _fade;
        private TweenedDouble _topTextFade;

        private EditMode _editMode = EditMode.Tiles;

		private MapEditorModeState _state;

        private enum DataOperation
        {
            None,
            Saving,
            Loading
        }

        private DataOperation _dataOperation = DataOperation.None;
		
		public Map Map;
		public Dictionary<string, Button> Buttons = new Dictionary<string, Button>();
		public Dictionary<string, SelectionMenu> SelectionMenus = new Dictionary<string, SelectionMenu>();
		public Dictionary<string, TextField> TextFields = new Dictionary<string, TextField>();
        public Dictionary<string, ChoiceField> ChoiceFields = new Dictionary<string, ChoiceField>();

        public Camera Camera { get; private set; }
		
		/// <summary>
		/// Current map editor state.
		/// </summary>
		public MapEditorModeState CurrentState
		{
			get
			{
				return _state;
			}
			set
			{
				if(_state != null)
					_state.OnLeave(value);

				if(value != null)
				{
					value.Game = Game;
					value.BaseState = this;
					value.OnEnter(_state);
				}
                
				_state = value;
			}
		}

        public override void OnEnter(GameState previousState)
		{
            Game.IsMouseVisible = true;
			Map = new Map(50, 50, new Map.Information("Untitled", Map.Environment.Forest, Map.Weather.Sunny));

			if(Map.TransitionCache != null)
				Map.TransitionCache.Clear();

			Camera = new Camera(this);

            Buttons.Add("edit-tiles-mode", new Button
            {
                Text = "",
                Icon = Assets.Get<Texture2D>("editor/tiles.png"),
                Position = new TweenedVector2(Game, new Vector2(-200, 114)),
                Font = Game.DefaultFonts.Normal,
                Clicked = (btn) =>
                {
	                _editMode = EditMode.Tiles;
	                CurrentState = new MapEditorTileEditState();
                },

                Active = false
            });

            Buttons.Add("edit-attributes-mode", new Button
            {
                Text = "",
                Icon = Assets.Get<Texture2D>("editor/attributes.png"),
	            Position = new TweenedVector2(Game, new Vector2(-200, 114 + 32)),
                Font = Game.DefaultFonts.Normal,
                Clicked = (btn) =>
                {
	                _editMode = EditMode.Attributes; 
                },

                Active = false
            });

            Buttons.Add("edit-info-mode", new Button
            {
                Text = "",
                Icon = Assets.Get<Texture2D>("editor/info.png"),
	            Position = new TweenedVector2(Game, new Vector2(-200, 114 + 32 * 2)),
                Font = Game.DefaultFonts.Normal,
                Clicked = (btn) =>
                {
	                _editMode = EditMode.Info; 
	                CurrentState = new MapEditorEditInfoState();
                },

                Active = false
            });
			
			Buttons.Add("keybinds-help-mode", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("editor/keybinds.png"),
				Position = new TweenedVector2(Game, new Vector2(-200, 114 + 32 * 3)),
				Font = Game.DefaultFonts.Normal,
				Clicked = (btn) =>
				{
					_editMode = EditMode.Keybinds; 
					CurrentState = new MapEditorShowKeybindsState();
				},

				Active = false
			});
			
			Buttons.Add("load", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("editor/load.png"),
				Position = new TweenedVector2(Game, new Vector2(-200, 118 + 32 * 4)),
				Font = Game.DefaultFonts.Normal,
				Clicked = (btn) =>
				{
					OpenFileDialog dialog = new OpenFileDialog();
					
					dialog.Filter = "Map files (*.map)|*.map|All files (*.*)|*.*";
                    if(dialog.ShowDialog() == DialogResult.OK)
                    {
                        _dataOperation = DataOperation.Loading;
                        Task.Run(() =>
                        {
                            Map = Map.Load(new FileStream(dialog.FileName, FileMode.Open));
                            _dataOperation = DataOperation.None;
                        });
                    }
					
					dialog.Dispose();
					Application.DoEvents();
				},

				Active = false
			});
			
			Buttons.Add("save", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("editor/save.png"),
				Position = new TweenedVector2(Game, new Vector2(-200, 118 + 32 * 5)),
				Font = Game.DefaultFonts.Normal,
				Clicked = (btn) =>
				{
					SaveFileDialog dialog = new SaveFileDialog();
					
					dialog.Filter = "Map files (*.map)|*.map|All files (*.*)|*.*";
                    if(dialog.ShowDialog() == DialogResult.OK)
                    {
                        _dataOperation = DataOperation.Saving;
                        Task.Run(() => {
                            Map.Save(new FileStream(dialog.FileName, FileMode.Create));
                            _dataOperation = DataOperation.None;
                        });
                    }
					 
					dialog.Dispose();
					Application.DoEvents();
				},

				Active = false
			});
			
			Buttons.Add("exit", new Button
			{
				Text = "",
				Icon = Assets.Get<Texture2D>("editor/exit.png"),
				Position = new TweenedVector2(Game, new Vector2(-200, 118 + 32 * 6)),
				Font = Game.DefaultFonts.Normal,
				Clicked = (btn) => { throw new Exception("aaa"); },

				Active = false
			});

            Buttons["edit-tiles-mode"].Position.TweenTo(new Vector2(10, 114), TweenEaseType.EaseOutQuad, 0.65f);
            Buttons["edit-attributes-mode"].Position.TweenTo(new Vector2(10, 114 + 32), TweenEaseType.EaseOutQuad, 0.65f);
            Buttons["edit-info-mode"].Position.TweenTo(new Vector2(10, 114 + 32 * 2), TweenEaseType.EaseOutQuad, 0.65f);
			Buttons["keybinds-help-mode"].Position.TweenTo(new Vector2(10, 114 + 32 * 3), TweenEaseType.EaseOutQuad, 0.65f);
			
			Buttons["load"].Position.TweenTo(new Vector2(10, 118 + 32 * 4), TweenEaseType.EaseOutQuad, 0.65f);
			Buttons["save"].Position.TweenTo(new Vector2(10, 118 + 32 * 5), TweenEaseType.EaseOutQuad, 0.65f);
			Buttons["exit"].Position.TweenTo(new Vector2(10, 118 + 32 * 6), TweenEaseType.EaseOutQuad, 0.65f);

            _topTextY = new TweenedDouble(Game, -300);
            _topTextY.TweenTo(10, TweenEaseType.EaseOutQuad, 0.65f);

            _fade = new TweenedDouble(Game, 1.0);
            _fade.TweenTo(0, TweenEaseType.Linear, 0.5f);

            _topTextFade = new TweenedDouble(Game, 2.0);
			
			CurrentState = new MapEditorTileEditState();
        }

        public override void OnLeave(GameState nextState)
		{
			CurrentState.OnLeave(null);
		}

		private string CurrentHelpText
		{
			get
			{
                if(CurrentState.CurrentHelpText != "")
                    return CurrentState.CurrentHelpText;

				if(!Buttons["edit-tiles-mode"].Active && Buttons["edit-tiles-mode"].Rectangle.Contains(Input.MousePosition))
					return "Edit tiles";
				if(!Buttons["edit-attributes-mode"].Active && Buttons["edit-attributes-mode"].Rectangle.Contains(Input.MousePosition))
					return "Edit tile attributes";
				if(!Buttons["edit-info-mode"].Active && Buttons["edit-info-mode"].Rectangle.Contains(Input.MousePosition))
					return "Edit map info";
				if(!Buttons["keybinds-help-mode"].Active && Buttons["keybinds-help-mode"].Rectangle.Contains(Input.MousePosition))
					return "Show key bindings";
				
				if(!Buttons["load"].Active && Buttons["load"].Rectangle.Contains(Input.MousePosition))
					return "Load map file";
				if(!Buttons["save"].Active && Buttons["save"].Rectangle.Contains(Input.MousePosition))
					return "Save map file";
				if(!Buttons["exit"].Active && Buttons["exit"].Rectangle.Contains(Input.MousePosition))
					return "Exit";

				return "No action selected";
			}
		}

		public override void Update(double deltaTime, double totalTime, long count)
		{
			if(!CurrentState.PauseEditor && _dataOperation == DataOperation.None)
			{
                int xadd = Input.Key(Keys.LeftAlt) || Input.Key(Keys.RightAlt) ? -1 : 1;
                if(Input.Key(Keys.LeftControl) || Input.Key(Keys.RightControl))
                    xadd *= 5;

                int yadd = xadd;

                while(Map.Width + xadd < 1)
                    xadd++;

                while(Map.Width + xadd > 4096)
                    xadd--;

                while(Map.Height + yadd < 1)
                    yadd++;

                while(Map.Height + yadd > 4096)
                    yadd--;

                if(Input.Key(Keys.LeftShift) || Input.Key(Keys.RightShift))
                {
                    if(Input.KeyPressed(Keys.Left))
                        Map.Resize(Map.Width + xadd, Map.Height, xadd, 0);

                    if(Input.KeyPressed(Keys.Right))
                        Map.Resize(Map.Width + xadd, Map.Height, 0, 0);

                    if(Input.KeyPressed(Keys.Up))
                        Map.Resize(Map.Width, Map.Height + yadd, 0, yadd);

                    if(Input.KeyPressed(Keys.Down))
                        Map.Resize(Map.Width, Map.Height + yadd, 0, 0);
                }
            }

            Camera.Update(deltaTime, totalTime);
			
            Buttons["edit-tiles-mode"].Active = _editMode == EditMode.Tiles;
            Buttons["edit-attributes-mode"].Active = _editMode == EditMode.Attributes;
            Buttons["edit-info-mode"].Active = _editMode == EditMode.Info;
			Buttons["keybinds-help-mode"].Active = _editMode == EditMode.Keybinds;

            if(_dataOperation == DataOperation.None)
            {
                foreach(Button button in Buttons.Values.ToArray())
                    button.Update(Game);

                foreach(SelectionMenu menu in SelectionMenus.Values.ToArray())
                    menu.Update(Game);

                foreach(TextField field in TextFields.Values.ToArray())
                    field.Update(Game);

                foreach(ChoiceField field in ChoiceFields.Values.ToArray())
                    field.Update(Game);
            }

            string str =
               "Map Editor\n" +
               "Name: \"" + Map.Info.Name + "\"\n" +
               "Size: " + Map.Width + "x" + Map.Height + "\n" +
			   "[PLACEHOLDER PLACEHOLDER PLACEHOLDER]";

            Vector2 measure = Game.DefaultFonts.Bold.Measure(15, str);
            Rectangle topTextRectangle = new Rectangle(10, (int)_topTextY, (int)(measure.X + 20), (int)(measure.Y + 20));

            if(topTextRectangle.Contains(Input.MousePosition) && !topTextRectangle.Contains(Input.PreviousMousePosition))
                _topTextFade.TweenTo(0.6, TweenEaseType.EaseOutQuad, 0.4f);
            else if(!topTextRectangle.Contains(Input.MousePosition) && topTextRectangle.Contains(Input.PreviousMousePosition))
                _topTextFade.TweenTo(2.0, TweenEaseType.EaseOutQuad, 0.4f);

            if(_dataOperation == DataOperation.None)
                CurrentState.Update(deltaTime, totalTime, count);
        }

		public bool IsPointObscured(Vector2 point)
		{
			foreach(Button btn in Buttons.Values)
			{
				if(btn.Rectangle.Contains(point))
					return true;
			}

            foreach(SelectionMenu menu in SelectionMenus.Values)
            {
                if(menu.Rectangle.Value.Contains(point))
                    return true;
            }
			
			foreach(TextField field in TextFields.Values)
			{
				if(field.Rectangle.Contains(point))
					return true;
			}

            foreach(ChoiceField field in ChoiceFields.Values)
            {
                if(field.Rectangle.Contains(point))
                    return true;
            }

            return false;
		}

        public override void FixedUpdate(long count)
		{
            if(_dataOperation == DataOperation.None)
                CurrentState.FixedUpdate(count);
		}

		public override void Draw(SpriteBatch batch, Vector2 screenSize)
		{
			batch.Transform = Camera.Transform;
			batch.SamplerState = SamplerState.PointWrap;

			Vector2 scissorPos = Vector2.Transform(Vector2.Zero, Camera.Transform);
			batch.Scissor = new Rectangle((int)scissorPos.X, (int)scissorPos.Y, (int)(Map.Width * 16 * Camera.Zoom.Value), (int)(Map.Height * 16 * Camera.Zoom.Value));

            foreach(Tile.MapLayer layer in Enum.GetValues(typeof(Tile.MapLayer)))
            {
                if(layer == Tile.MapLayer.Control && 
                    (CurrentState.GetType() != typeof(MapEditorTileEditState) || (CurrentState as MapEditorTileEditState).Layer != Tile.MapLayer.Control))
                    break;

                Rectangle area = new Rectangle(
                    (int)-(Camera.Translation.X / Camera.Zoom.Value),
                    (int)-(Camera.Translation.Y / Camera.Zoom.Value),
                    (int)(screenSize.X / Math.Floor(Camera.Zoom.Value)),
                    (int)(screenSize.Y / Math.Floor(Camera.Zoom.Value)));

                if(layer == Tile.MapLayer.Control)
                    batch.Rectangle(area, Color.Black * 0.6f);

                Map.Draw(layer, Game, area);
            }

			batch.Scissor = null;

            batch.Outline(new Rectangle(0, 0, Map.Width * 16, Map.Height * 16),
				Color.White, 1, false);
			
			//batch.Texture(new Vector2(64, 64), Assets.Get<Texture2D>("textures/buildings/mine_soviet.png"), Color.White);
			
			batch.Reset();

            if(_dataOperation == DataOperation.None)
            {
                CurrentState.Draw(batch, screenSize);

                string str =
                    "Map Editor\n" +
                    "Name: \"" + Map.Info.Name + "\"\n" +
                    "Size: " + Map.Width + "x" + Map.Height + "\n" +
                    "[PLACEHOLDER PLACEHOLDER PLACEHOLDER]";

                Vector2 measure = Game.DefaultFonts.Bold.Measure(15, str);
                batch.Rectangle(new Rectangle(10, (int)_topTextY, (int)(measure.X + 20), (int)(measure.Y + 20)),
                    Color.Black * (MathHelper.Clamp(_topTextFade, 0, 1) * 0.85f));

                str = str.Replace("[PLACEHOLDER PLACEHOLDER PLACEHOLDER]", CurrentHelpText)
                    .Replace("Map Editor\n", "");

                batch.Text(SpriteBatch.FontStyle.Bold, 15, "Map Editor", new Vector2(20, (int)_topTextY + 10),
                    Color.White * MathHelper.Clamp(_topTextFade, 0, 1));
                batch.Text(SpriteBatch.FontStyle.Bold, 15, str, new Vector2(20, (int)_topTextY + 10 + (15 * 1.25f)),
                    Color.White * (MathHelper.Clamp(_topTextFade, 0, 1) * 0.8f));

                foreach(Button button in Buttons.Values)
                    button.Draw(Game);

                foreach(SelectionMenu menu in SelectionMenus.Values)
                    menu.Draw(Game);

                foreach(TextField field in TextFields.Values)
                    field.Draw(Game);

                foreach(ChoiceField field in ChoiceFields.Values)
                    field.Draw(Game);
            }

            batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black * _fade);

            if(_dataOperation != DataOperation.None)
            {
                string text = "Saving map...";
                if(_dataOperation == DataOperation.Loading)
                    text = "Loading map...";

                Vector2 measure = Game.DefaultFonts.Bold.Measure(32, text);

                float alpha = 0.6f;
                alpha = MathHelper.Clamp(alpha + (float)Math.Sin(Game.Time * 10f) * 0.05f, 0, 1);

                batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black * alpha);
                batch.Text(FontStyle.Bold, 32, text, screenSize / 2 - measure / 2, Color.White);
            }
        }
	}
}