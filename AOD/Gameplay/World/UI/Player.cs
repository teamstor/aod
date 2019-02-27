using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine;
using TeamStor.Engine.Coroutine;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;
using static TeamStor.Engine.Graphics.SpriteBatch;

using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Gameplay.World.UI
{
    /// <summary>
    /// Shows the players stats and equips.
    /// </summary>
    public class PlayerUI
    {
        private bool _closed = false;
        private bool _transitioning = false;

        private OnPlayerUICompleted _completedEvent;
        public delegate void OnPlayerUICompleted(Player entity);

        private TweenedDouble _offsetY;

        private WorldState _world;
        private Player _entity;

        /// <summary>
        /// If this player UI has been closed and has finished showing.
        /// </summary>
        public bool IsShowingCompleted
        {
            get; private set;
        } = false;

        private PlayerUI(WorldState world, Player entity, bool noOffset)
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

                if(InputMap.FindMapping(InputAction.Back).Pressed(_world.Input) ||
                   InputMap.FindMapping(InputAction.Inventory).Pressed(_world.Input))
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

            batch.Text(font, 
                16, 
                "[" + InputMap.FindMapping(InputAction.Inventory).Key + "] Inventory", 
                new Vector2(16, 8 + (bg.Height + 20) * (float)_offsetY.Value), 
                Color.White * 0.6f);
            
            batch.Text(font, 
                16, 
                "[" + InputMap.FindMapping(InputAction.Player).Key + "] " + _entity.Name, 
                new Vector2(16 + (int)font.Measure(16, "[" + InputMap.FindMapping(InputAction.Inventory).Key + "] Inventory").X + 6, 8 + (bg.Height + 20) * (float)_offsetY.Value), 
                Color.White);
        }

        public static IEnumerator<ICoroutineOperation> Show(WorldState world, OnPlayerUICompleted completeEvent = null, bool noOffset = false)
        {
            PlayerUI pui = new PlayerUI(world, world.Player, noOffset);
            pui._completedEvent = completeEvent;
            return world.Coroutine.Start(pui.ShowCoroutine);
        }
    }
}