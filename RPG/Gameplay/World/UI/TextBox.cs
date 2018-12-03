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
using TeamStor.Engine.Tween;
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
        private long _completionTime = 0;

        private TweenedDouble _offsetY;

        private OnTextBoxCompleted _completedEvent;
        public delegate void OnTextBoxCompleted();

        private TextBox(WorldState world, TextBoxContent content)
        {
            _world = world;
            _content = content;
            _offsetY = new TweenedDouble(world.Game, 1.0);
        }

        private IEnumerator<ICoroutineOperation> ShowCoroutine()
        {
            _world.Paused = true;
            _world.DrawHook += DrawHook;

            bool wasHeldAtStart = InputMap.FindMapping(InputAction.Action).Held(_world.Input);

            _offsetY.TweenTo(0.0, TweenEaseType.EaseOutQuad, 0.1);
            while(!_offsetY.IsComplete)
                yield return null;

            yield return Wait.Seconds(_world.Game, 0.1);

            while(true)
            {
                if(InputMap.FindMapping(InputAction.Action).Pressed(_world.Input) && _textWrittenYet.Length == _content.Text.Length)
                    break;

                if(InputMap.FindMapping(InputAction.Action).Held(_world.Input) && !wasHeldAtStart)
                {
                    if(_textWrittenYet.Length < _content.Text.Length)
                    {
                        _textWrittenYet = _content.Text;
                        _completionTime = _world.Game.TotalFixedUpdates;
                    }
                }

                if(wasHeldAtStart && !InputMap.FindMapping(InputAction.Action).Held(_world.Input))
                    wasHeldAtStart = false;

                if(_textWrittenYet.Length < _content.Text.Length)
                {
                    _textWrittenYet += _content.Text[_textWrittenYet.Length];
                    if(_textWrittenYet.Length == _content.Text.Length)
                        _completionTime = _world.Game.TotalFixedUpdates;
                    yield return Wait.Seconds(_world.Game, 0.03);
                }
                else
                    yield return null;
            }
            
            _offsetY.TweenTo(1.0, TweenEaseType.EaseInQuad, 0.1);
            while(!_offsetY.IsComplete)
                yield return null;

            _world.DrawHook -= DrawHook;
            _world.Paused = false;

            if(_completedEvent != null)
                _completedEvent();
        }

        private void DrawHook(object sender, WorldState.DrawEventArgs args)
        {
            WorldState world = sender as WorldState;
            SpriteBatch batch = args.Batch;
            Vector2 screenSize = args.ScreenSize;

            Matrix oldTransform = batch.Transform;
            batch.Rectangle(new Rectangle(0, 0, args.Batch.Device.Viewport.Width, args.Batch.Device.Viewport.Height), Color.Black * (1.0f - _offsetY) * 0.3f);
            batch.Transform = oldTransform;

            Texture2D bg = _world.Assets.Get<Texture2D>("ui/textbox.png");
            batch.Texture(new Vector2(4, 270 - bg.Height - 4 + (bg.Height + 20) * (float)_offsetY.Value), bg, Color.White);

            Font font = _world.Assets.Get<Font>("fonts/bitcell.ttf");

            batch.Text(font, 16, _content.Speaker, new Vector2(16, 270 - bg.Height + (bg.Height + 20) * (float)_offsetY.Value), Color.White, 0.8f);
            batch.Text(font, 16, _textWrittenYet, new Vector2(16, 270 - bg.Height + 12 + (bg.Height + 20) * (float)_offsetY.Value), Color.White * 0.8f, 0.8f);

            if(_textWrittenYet.Length == _content.Text.Length && 
               ((world.Game.TotalFixedUpdates - _completionTime) % 40) > 20)
                batch.Texture(new Vector2(480 - 20, 270 - 24 + (bg.Height + 20) * (float)_offsetY.Value), _world.Assets.Get<Texture2D>("ui/arrow.png"), Color.White * 0.8f);
        }

        /// <summary>
        /// Shows a text box and pauses the world.
        /// </summary>
        /// <param name="world">The world to pause and show the text box in.</param>
        /// <param name="textBoxContent">Content of the text box.</param>
        public static IEnumerator<ICoroutineOperation> Show(WorldState world, TextBoxContent textBoxContent, OnTextBoxCompleted completeEvent = null)
        {
            TextBox box = new TextBox(world, textBoxContent);
            box._completedEvent = completeEvent;
            return world.Coroutine.Start(box.ShowCoroutine);
        }
    }
}
