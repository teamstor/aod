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

        private int _selectedSlot;

        private OnInventoryUICompleted _completedEvent;
        public delegate void OnInventoryUICompleted(LivingEntity entity);

        private TweenedDouble _offsetY;
        private float _scroll;

        private GameState _state;
        private LivingEntity _entity;

        /// <summary>
        /// If this inventory UI has been closed and has finished showing.
        /// </summary>
        public bool IsShowingCompleted
        {
            get; private set;
        } = false;

        /// <summary>
        /// Creates a standalone inventory UI that needs to be updated manually.
        /// </summary>
        /// <param name="entity">The entity to use the inventory with.</param>
        public InventoryUI(LivingEntity entity, GameState state)
        {
            _entity = entity;
            _offsetY = new TweenedDouble(state.Game, 1.0);
            _state = state;

            state.Coroutine.Start(ShowCoroutine);
        }

        private InventoryUI(WorldState world, LivingEntity entity)
        {
            _state = world;
            _entity = entity;

            _offsetY = new TweenedDouble(world.Game, 1.0);
        }

        private IEnumerator<ICoroutineOperation> ShowCoroutine()
        {
            if(_state is WorldState)
            {
                (_state as WorldState).Paused = true;
                (_state as WorldState).UpdateHook += UpdateHook;
                (_state as WorldState).DrawHook += DrawHook;
            }

            _offsetY.TweenTo(0.0, TweenEaseType.EaseOutQuad, 0.1);
            while(!_offsetY.IsComplete)
                yield return null;

            yield return Wait.Seconds(_state.Game, 0.1);

            while(!_closed)
            {
                yield return null;
            }

            _offsetY.TweenTo(1.0, TweenEaseType.EaseInQuad, 0.1);
            while(!_offsetY.IsComplete)
                yield return null;

            if(_state is WorldState)
            {
                (_state as WorldState).Paused = false;
                (_state as WorldState).UpdateHook -= UpdateHook;
                (_state as WorldState).DrawHook -= DrawHook;
            }

            if(_completedEvent != null)
                _completedEvent(_entity);

            IsShowingCompleted = true;
        }

        private IEnumerator<ICoroutineOperation> ChangeSelectedSlot(int selectedSlot)
        {
            _selectedSlot = -1;

            int y = 37 + selectedSlot * 15;
            int height = (_entity.Inventory.OccupiedSlots * 15);
            Rectangle itemsListBounds = new Rectangle(16, 37, 171, 210);

            float scrollStart = _scroll;
            float scrollTarget = -1;

            if(itemsListBounds.Bottom <= y + 15 - (int)_scroll)
            {
                scrollTarget = _scroll;

                while(itemsListBounds.Bottom <= y + 15 - (int)scrollTarget)
                    scrollTarget++;
            }
            if(itemsListBounds.Top > y - (int)_scroll)
            {
                scrollTarget = _scroll;

                while(itemsListBounds.Top > y - (int)scrollTarget)
                    scrollTarget--;
            }

            if(scrollTarget != -1)
                scrollTarget = MathHelper.Clamp(scrollTarget, 0, height - itemsListBounds.Height - 6);

            if(scrollTarget == -1 || Math.Abs(scrollTarget - scrollStart) == 1)
                yield return Wait.Seconds(_state.Game, 0.06);
            else
            {
                double startTime = _state.Game.Time;

                while(_state.Game.Time < startTime + (Math.Abs(scrollTarget - scrollStart) > 100 ? 0.5 : 0.1))
                {
                    _scroll = MathHelper.Lerp(scrollStart, scrollTarget, (float)(_state.Game.Time - startTime) * (1f / (Math.Abs(scrollTarget - scrollStart) > 100 ? 0.5f : 0.1f)));
                    yield return null;
                }

                _scroll = scrollTarget;
            }

            _selectedSlot = selectedSlot;
        }

        public void ManualUpdate()
        {
            UpdateHook(this, new Game.UpdateEventArgs(_state.Game.DeltaTime, _state.Game.Time, _state.Game.TotalUpdates));
        }

        public void ManualDraw()
        {
            DrawHook(this, new WorldState.DrawEventArgs()
            {
                Batch = _state.Game.Batch,
                ScreenSize = new Vector2(480, 270)
            });
        }

        private void UpdateHook(object sender, Game.UpdateEventArgs e)
        {
            if(InputMap.FindMapping(InputAction.Back).Held(_state.Input))
                _closed = true;

            if(_selectedSlot != -1)
            {
                if(InputMap.FindMapping(InputAction.Up).Pressed(_state.Input))
                {
                    if(_selectedSlot > 0)
                        _state.Coroutine.AddExisting(ChangeSelectedSlot(_selectedSlot - 1));
                    else
                        _state.Coroutine.AddExisting(ChangeSelectedSlot(_entity.Inventory.OccupiedSlots - 1));
                }

                if(InputMap.FindMapping(InputAction.Down).Pressed(_state.Input))
                {
                    if(_selectedSlot < _entity.Inventory.OccupiedSlots - 1)
                        _state.Coroutine.AddExisting(ChangeSelectedSlot(_selectedSlot + 1));
                    else
                        _state.Coroutine.AddExisting(ChangeSelectedSlot(0));
                }
            }
        }

        private void DrawHook(object sender, WorldState.DrawEventArgs args)
        {
            WorldState world = sender as WorldState;
            SpriteBatch batch = args.Batch;
            Vector2 screenSize = args.ScreenSize;

            batch.Rectangle(new Rectangle(0, 0, args.Batch.Device.Viewport.Width, args.Batch.Device.Viewport.Height), Color.Black * (1.0f - _offsetY) * 0.3f);

            Texture2D bg = _state.Assets.Get<Texture2D>("ui/inventory.png");
            batch.Texture(new Vector2(0, 270 - bg.Height + (bg.Height + 20) * (float)_offsetY.Value), bg, Color.White);

            Font font = _state.Assets.Get<Font>("fonts/bitcell.ttf");
            batch.Text(font, 16, "Inventory", new Vector2(16, 8 + (bg.Height + 20) * (float)_offsetY.Value), Color.White);

            Rectangle itemsListRectangle = new Rectangle(16, 31, 171, 222);
            Vector2 transformLT = Vector2.Transform(itemsListRectangle.Location.ToVector2(), batch.Transform);
            Vector2 transformRB = Vector2.Transform(new Vector2(itemsListRectangle.Right, itemsListRectangle.Bottom), batch.Transform);

            Vector3 scale, translation;
            Quaternion rot;
            batch.Transform.Decompose(out scale, out rot, out translation);
            batch.Transform = batch.Transform * Matrix.CreateTranslation(0, (int)-_scroll * scale.X, 0);

            batch.Scissor = new Rectangle(
                (int)transformLT.X,
                (int)(transformLT.Y + (bg.Height + 20) * (float)_offsetY.Value * scale.X),
                (int)(transformRB.X - transformLT.X),
                (int)(transformRB.Y - transformLT.Y));

            itemsListRectangle.Y += 6;
            itemsListRectangle.Height -= 12;

            if(_entity.Inventory.OccupiedSlots == 0)
            {
                Vector2 measure = font.Measure(16, "No items in inventory");
                batch.Text(font, 16, "No items in inventory", itemsListRectangle.Center.ToVector2() - measure / 2 + new Vector2(0, (bg.Height + 20) * (float)_offsetY.Value), Color.White * 0.4f);
            }
            else
            {
                for(int i = 0; i < _entity.Inventory.OccupiedSlots; i++)
                {
                    Inventory.ItemSlotReference reference = _entity.Inventory[i];
                    int y = itemsListRectangle.Y + i * 15 + (int)((bg.Height + 20) * _offsetY.Value);
                    bool selected = _selectedSlot == i;

                    batch.Texture(
                        new Vector2(itemsListRectangle.X + 6, y),
                        _state.Assets.Get<Texture2D>(reference.ReferencedItem.SmallIcon),
                        Color.White * (selected ? 0.8f : 0.4f));

                    batch.Text(font,
                        16,
                        reference.ReferencedItem.Name,
                        new Vector2(itemsListRectangle.X + 18, y - 8),
                        Color.White * (selected ? 0.8f : 0.4f));
                }
            }

            batch.Transform = batch.Transform * Matrix.CreateTranslation(0, (int)_scroll * scale.X, 0);
            batch.Scissor = null;

            Rectangle infoRectangle = new Rectangle(itemsListRectangle.Right + 12, itemsListRectangle.Top - 6 + (int)((bg.Height + 20) * (float)_offsetY.Value), 261, 223);

            if(_entity.Inventory.OccupiedSlots > 0 && _selectedSlot != -1)
            {
                Inventory.ItemSlotReference reference = _entity.Inventory[_selectedSlot];

                batch.Texture(
                    new Vector2(infoRectangle.X + infoRectangle.Width / 2 - 16, infoRectangle.Y),
                    _state.Assets.Get<Texture2D>(reference.ReferencedItem.Icon),
                    Color.White);
            }
        }

        public static IEnumerator<ICoroutineOperation> Show(WorldState world, LivingEntity entity, OnInventoryUICompleted completeEvent = null)
        {
            InventoryUI inventory = new InventoryUI(world, entity);
            inventory._completedEvent = completeEvent;
            return world.Coroutine.Start(inventory.ShowCoroutine);
        }
    }
}