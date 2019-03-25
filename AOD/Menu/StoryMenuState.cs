using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.AOD.Gameplay.World;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Menu
{
    public class StoryMenuState : GameState
    {
        private string _story = "";
        private string _writtenStory = "";

        private long _startCount = -1;
        private float _scroll = 0;

        public override void OnEnter(GameState previousState)
        {
            _story = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/data/menu/story.txt").Replace("\r", "");
        }

        public override void OnLeave(GameState nextState)
        {
        }

        public override void Update(double deltaTime, double totalTime, long count)
        {
            if(InputMap.FindMapping(InputAction.Action).Pressed(Input))
            {
                if(_writtenStory.Length != _story.Length)
                    _writtenStory = _story;
                else
                    Game.CurrentState = new WorldState(Map.Load("data/maps/mapt.json"), true);
            }

            float targetScroll = Math.Min(-_writtenStory.Split('\n').Length * 18 + (18 * 6), 0);
            _scroll = MathHelper.Lerp(_scroll, targetScroll, (float)deltaTime * 0.5f);
        }

        public override void FixedUpdate(long count)
        {
            if(_startCount == -1)
                _startCount = count;

            if((count - _startCount) % 3 == 0 && _writtenStory.Length < _story.Length)
                _writtenStory += _story[_writtenStory.Length];
        }

        public override void Draw(SpriteBatch batch, Vector2 screenSize)
        {
            screenSize = Program.ScaleBatch(batch);

            batch.Rectangle(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.Black);

            Font alkhemikal = Assets.Get<Font>("fonts/Alkhemikal.ttf", true);

            int y = (int)(110 + _scroll);
            string[] split = _story.Split('\n');
            string[] writtenSplit = _writtenStory.Split('\n');

            for(int i = 0; i < writtenSplit.Length; i++)
            {
                Vector2 m = alkhemikal.Measure(16, split[i]);
                batch.Text(alkhemikal, 16, writtenSplit[i], new Vector2(screenSize.X / 2 - m.X / 2, y), Color.White);
                y += 18;
            }

            batch.Texture(new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Assets.Get<Texture2D>("menu/story_mask.png"), Color.White);
            batch.Texture(new Vector2(screenSize.X / 2 - 160 / 2, 20), Assets.Get<Texture2D>("ui/logo.png", true), Color.White);

            Font bitcell = Assets.Get<Font>("fonts/bitcell.ttf", true);
            string s = "Press [" + InputMap.FindMapping(InputAction.Action).Key + "] to " + (_writtenStory.Length == _story.Length ? "start game" : "skip");
            Vector2 measure = bitcell.Measure(16, s);
            float alpha = 0.8f + (float)Math.Sin(Game.Time * 10) * 0.1f;
            batch.Text(bitcell, 16, s, new Vector2(screenSize.X / 2 - measure.X / 2, screenSize.Y - 8 - 20), Color.White * alpha);

            Program.BlackBorders(batch);
        }
    }
}
