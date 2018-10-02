using Microsoft.Xna.Framework;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;

namespace TeamStor.RPG.Editor.States
{
	public class MapEditorShowKeybindsState : MapEditorModeState
	{
		public const string KEY_BINDINGS = 
			"Left-click: Select or edit\n" + 
			"Right-click: Move camera\n\n" +
            "E: Select the empty tile\n" +
			"T: Switch between editing tiles and tile attributes\n\n" +
            "Up/down: Switch tiles\n" +
			"CTRL + Up/down: Switch layers\n" +
            "SHIFT + Arrow keys: Grow map (Hold CTRL to grow by 5, Hold ALT to shrink)";
		
		public override bool PauseEditor
		{
			get { return true; }
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
			batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black * 0.6f);

			Vector2 measure = Game.DefaultFonts.Bold.Measure(16, KEY_BINDINGS);
			batch.Text(SpriteBatch.FontStyle.Bold, 16, KEY_BINDINGS, new Vector2(screenSize.X / 2 - measure.X / 2, screenSize.Y / 2 - measure.Y / 2), Color.White);
		}
	}
}