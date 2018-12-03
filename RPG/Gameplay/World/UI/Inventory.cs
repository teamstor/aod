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

namespace TeamStor.RPG.Gameplay.World.UI
{
    /// <summary>
    /// Shows an editable inventory for an entity.
    /// </summary>
    public class InventoryUI
    {
        private bool _closed = false;

        private OnInventoryUICompleted _completedEvent;
        public delegate void OnInventoryUICompleted(LivingEntity entity);

        private TweenedDouble _offsetY;

        private WorldState _world;
        private LivingEntity _entity;

        private InventoryUI(WorldState world, LivingEntity entity)
        {
            _world = world;
            _entity = entity;

            _offsetY = new TweenedDouble(world.Game, 1.0);
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
            {
                yield return null;
            }

            _offsetY.TweenTo(1.0, TweenEaseType.EaseInQuad, 0.1);
            while(!_offsetY.IsComplete)
                yield return null;

            _world.Paused = false;
            _world.UpdateHook -= UpdateHook;
            _world.DrawHook -= DrawHook;

            if(_completedEvent != null)
                _completedEvent(_entity);
        }

        private void UpdateHook(object sender, Game.UpdateEventArgs e)
        {
            if(InputMap.FindMapping(InputAction.Back).Held(_world.Input))
                _closed = true;
        }

        private void DrawHook(object sender, WorldState.DrawEventArgs args)
        {
            WorldState world = sender as WorldState;
            SpriteBatch batch = args.Batch;
            Vector2 screenSize = args.ScreenSize;

            Matrix oldTransform = batch.Transform;
            batch.Rectangle(new Rectangle(0, 0, args.Batch.Device.Viewport.Width, args.Batch.Device.Viewport.Height), Color.Black * (1.0f - _offsetY) * 0.3f);
            batch.Transform = oldTransform;

            Texture2D bg = _world.Assets.Get<Texture2D>("ui/inventory.png");
            batch.Texture(new Vector2(0, 270 - bg.Height + (bg.Height + 20) * (float)_offsetY.Value), bg, Color.White);
        }

        public static IEnumerator<ICoroutineOperation> Show(WorldState world, LivingEntity entity, OnInventoryUICompleted completeEvent = null)
        {
            InventoryUI inventory = new InventoryUI(world, entity);
            inventory._completedEvent = completeEvent;
            return world.Coroutine.Start(inventory.ShowCoroutine);
        }
    }
}
