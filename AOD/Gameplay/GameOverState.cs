using Microsoft.Xna.Framework;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;

namespace TeamStor.AOD.Gameplay
{
	public class GameOverState : GameState
	{
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
			screenSize = Program.ScaleBatch(batch);
			
			batch.Rectangle(new Rectangle(0, 0, 480, 270), Color.Black);
			
			Font font = Assets.Get<Font>("fonts/bitcell.ttf");
			Vector2 measure = font.Measure(16, "You died, bitch.");
			batch.Text(font, 16, "You died, bitch.", screenSize / 2 - measure / 2, Color.Red);

			Program.BlackBorders(batch);
		}
	}
}