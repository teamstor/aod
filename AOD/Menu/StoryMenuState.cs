using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Menu
{
    public class StoryMenuState : GameState
    {
        private string _story = "";
        private string _writtenStory = "";

        public override void OnEnter(GameState previousState)
        {
            _story = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/data/menu/story.txt").Replace("\r", "");
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

            batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black);

            Font font = Assets.Get<Font>("fonts/Alkhemikal.ttf", true);

            int y = 110;
            foreach(string s in _story.Split('\n'))
            {
                Vector2 measure = font.Measure(16, s);
                batch.Text(font, 16, s, new Vector2(screenSize.X / 2 - measure.X / 2, y), Color.White);
                y += (int)measure.Y;
            }

            batch.Texture(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Assets.Get<Texture2D>("menu/story_mask.png"), Color.White);
            batch.Texture(new Vector2(screenSize.X / 2 - 160 / 2, 20), Assets.Get<Texture2D>("ui/logo.png", true), Color.White);

            Program.BlackBorders(batch);
        }
    }
}
