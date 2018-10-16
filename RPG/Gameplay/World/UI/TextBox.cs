using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine.Coroutine;
using TeamStor.Engine.Graphics;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Gameplay.World.UI
{
    /// <summary>
    /// Content of a text box.
    /// </summary>
    public struct TextBoxContent
    {
        /// <summary>
        /// Person/thing that's speaking.
        /// </summary>
        public string Speaker;

        /// <summary>
        /// Text that's shown in the text box.
        /// </summary>
        public string Text;

        /// <summary>
        /// Portrait of the character speaking.
        /// </summary>
        public Texture2D Portait;
    }

    /// <summary>
    /// A customizable text box.
    /// </summary>
    public class TextBox
    {
        private WorldState _world;
        private TextBoxContent _content;
        private string _textWrittenYet = "";

        private TextBox(WorldState world, TextBoxContent content)
        {
            _world = world;
            _content = content;
        }

        private IEnumerator<ICoroutineOperation> ShowCoroutine()
        {
            _world.Paused = true;
            _world.DrawHook += DrawHook;

            while(!_world.Game.Input.Key(Keys.Enter))
            {
                if(_textWrittenYet.Length < _content.Text.Length)
                {
                    _textWrittenYet += _content.Text[_textWrittenYet.Length];
                    yield return Wait.Seconds(_world.Game, 0.03);
                }
                else
                    yield return null;
            }

            _world.DrawHook -= DrawHook;
            _world.Paused = false;
        }

        private void DrawHook(object sender, WorldState.DrawEventArgs args)
        {
            WorldState world = sender as WorldState;
            SpriteBatch batch = args.Batch;
            Vector2 screenSize = args.ScreenSize;

            Texture2D bg = _world.Assets.Get<Texture2D>("ui/textbox.png");
            batch.Texture(new Vector2(4, 270 - bg.Height - 4), bg, Color.White);

            Font font = _world.Assets.Get<Font>("fonts/bitcell.ttf");
            batch.Text(font, 16, _content.Speaker, new Vector2(16, 270 - bg.Height), Color.White, 0.8f);

            batch.Text(font, 16, _textWrittenYet, new Vector2(16, 270 - bg.Height + 12), Color.White * 0.8f, 0.8f);
        }

        /// <summary>
        /// Shows a text box and pauses the world.
        /// </summary>
        /// <param name="world">The world to pause and show the text box in.</param>
        /// <param name="textBoxContent">Content of the text box.</param>
        public static IEnumerator<ICoroutineOperation> Show(WorldState world, TextBoxContent textBoxContent)
        {
            TextBox box = new TextBox(world, textBoxContent);
            return world.Coroutine.Start(box.ShowCoroutine);
        }
    }
}
