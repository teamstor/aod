using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TeamStor.Engine.Coroutine;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;

using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Gameplay.World.UI
{
    /// <summary>
    /// Shows the players stats and equips.
    /// </summary>
    public class SpellsUI
    {
        private bool _closed = false;
        private bool _transitioning = false;

        private OnSpellsUICompleted _completedEvent;
        public delegate void OnSpellsUICompleted(Player entity);

        private TweenedDouble _offsetY;

        private WorldState _world;
        private Player _entity;

        /// <summary>
        /// If this spells UI has been closed and has finished showing.
        /// </summary>
        public bool IsShowingCompleted
        {
            get; private set;
        } = false;

        private SpellsUI(WorldState world, Player entity, bool noOffset)
        {
            _world = world;
            _entity = entity;

            _offsetY = new TweenedDouble(world.Game, noOffset ? 0.0 : 1.0);
        }

        private IEnumerator<ICoroutineOperation> ShowCoroutine()
        {
            _world.Paused = true;
            _world.UpdateHook += UpdateHook;
            _world.DrawHook += DrawHook;

            _offsetY.TweenTo(0.0, TweenEaseType.EaseOutQuad, 0.1);
            while(!_offsetY.IsComplete)
                yield return null;

            yield return Wait.Seconds(_world.Game, 0.1);

            while(!_closed)
                yield return null;

            if(_transitioning)
                yield return Wait.Seconds(_world.Game, 0.1);
            else
            {
                _offsetY.TweenTo(1.0, TweenEaseType.EaseInQuad, 0.1);
                while(!_offsetY.IsComplete)
                    yield return null;
            }

            if(!_transitioning)
                _world.Paused = false;
            _world.UpdateHook -= UpdateHook;
            _world.DrawHook -= DrawHook;

            if(_completedEvent != null)
                _completedEvent(_entity);

            IsShowingCompleted = true;
        }

        private void UpdateHook(object sender, Game.UpdateEventArgs e)
        {
            if(_offsetY == 0)
            {
                if(!_closed && InputMap.FindMapping(InputAction.Inventory).Pressed(_world.Input))
                {
                    InventoryUI.Show(_world, _entity, null, true);
                    _transitioning = true;
                }

                if(!_closed && InputMap.FindMapping(InputAction.Player).Pressed(_world.Input))
                {
                    PlayerUI.Show(_world, null, true);
                    _transitioning = true;
                }

                if(InputMap.FindMapping(InputAction.Back).Pressed(_world.Input) ||
                   InputMap.FindMapping(InputAction.Inventory).Pressed(_world.Input) ||
                   InputMap.FindMapping(InputAction.Player).Pressed(_world.Input))
                    _closed = true;
            }
        }

        private void DrawHook(object sender, WorldState.DrawEventArgs args)
        {
            WorldState world = sender as WorldState;
            SpriteBatch batch = args.Batch;
            Vector2 screenSize = args.ScreenSize;

            Texture2D bg = world.Assets.Get<Texture2D>("ui/player.png");
            batch.Texture(new Vector2(0, 270 - bg.Height + (bg.Height + 20) * (float)_offsetY.Value), bg, Color.White);

            Font font = world.Assets.Get<Font>("fonts/bitcell.ttf");

            int x = 16;

            batch.Text(font,
                16,
                "[" + InputMap.FindMapping(InputAction.Inventory).Key + "] Inventory",
                new Vector2(x, 8 + (bg.Height + 20) * (float)_offsetY.Value),
                Color.White * 0.6f);

            x += (int)font.Measure(16, "[" + InputMap.FindMapping(InputAction.Inventory).Key + "] Inventory").X + 6;

            batch.Text(font,
                16,
                "[" + InputMap.FindMapping(InputAction.Spells).Key + "] Spells",
                new Vector2(x, 8 + (bg.Height + 20) * (float)_offsetY.Value),
                Color.White);

            x += (int)font.Measure(16, "[" + InputMap.FindMapping(InputAction.Spells).Key + "] Spells").X + 6;

            batch.Text(font,
                16,
                "[" + InputMap.FindMapping(InputAction.Player).Key + "] " + _entity.Name,
                new Vector2(x, 8 + (bg.Height + 20) * (float)_offsetY.Value),
                Color.White * 0.6f);
        }

        public static IEnumerator<ICoroutineOperation> Show(WorldState world, OnSpellsUICompleted completeEvent = null, bool noOffset = false)
        {
            SpellsUI sui = new SpellsUI(world, world.Player, noOffset);
            sui._completedEvent = completeEvent;
            return world.Coroutine.Start(sui.ShowCoroutine);
        }
    }
}