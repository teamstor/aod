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

        private IEnumerator<ICoroutineOperation> ChangeSelectedSlot(int selectedSlot)
        {
            _selectedSlot = -1;

            int y = 37 + selectedSlot * 15;
            int height = (_world.Player.Inventory.OccupiedSlots * 15);
            Rectangle itemsListBounds = new Rectangle(16, 37, 171, 210);

            float scrollStart = _scroll;
            float scrollTarget = -1;

            if(itemsListBounds.Bottom < y + 15 - (int)_scroll)
            {
                scrollTarget = _scroll;

                while(itemsListBounds.Bottom < y + 15 - (int)scrollTarget)
                    scrollTarget++;
            }
            if(itemsListBounds.Top >= y - (int)_scroll)
            {
                scrollTarget = _scroll;

                while(itemsListBounds.Top >= y - (int)scrollTarget)
                    scrollTarget--;
            }

            if(scrollTarget != -1)
                scrollTarget = MathHelper.Clamp(scrollTarget, 0, height - itemsListBounds.Height - 6);

            if(scrollTarget == -1)
                yield return Wait.Seconds(_world.Game, 0.06);
            else
            {
                double startTime = _world.Game.Time;

                while(_world.Game.Time <= startTime + 0.15)
                {
                    _scroll = MathHelper.Lerp(scrollStart, scrollTarget, (float)(_world.Game.Time - startTime) * (1f / 0.15f));
                    yield return null;
                }
            }
                
            _selectedSlot = selectedSlot;
        }

        private void UpdateHook(object sender, Game.UpdateEventArgs e)
        {
            if(InputMap.FindMapping(InputAction.Back).Held(_world.Input))
                _closed = true;

            if(_selectedSlot != -1)
            {
                if(InputMap.FindMapping(InputAction.Up).Pressed(_world.Input))
                {
                    if(_selectedSlot > 0)
                        _world.Coroutine.AddExisting(ChangeSelectedSlot(_selectedSlot - 1));
                }

                if(InputMap.FindMapping(InputAction.Down).Pressed(_world.Input))
                {
                    if(_selectedSlot < _world.Player.Inventory.OccupiedSlots - 1)
                        _world.Coroutine.AddExisting(ChangeSelectedSlot(_selectedSlot + 1));
                }
            }
        }

        private void DrawHook(object sender, WorldState.DrawEventArgs args)
        {
            WorldState world = sender as WorldState;
            SpriteBatch batch = args.Batch;
            Vector2 screenSize = args.ScreenSize;

            batch.Rectangle(new Rectangle(0, 0, args.Batch.Device.Viewport.Width, args.Batch.Device.Viewport.Height), Color.Black * (1.0f - _offsetY) * 0.3f);

            Texture2D bg = _world.Assets.Get<Texture2D>("ui/inventory.png");
            batch.Texture(new Vector2(0, 270 - bg.Height + (bg.Height + 20) * (float)_offsetY.Value), bg, Color.White);

            Font font = _world.Assets.Get<Font>("fonts/bitcell.ttf");
            batch.Text(font, 16, "Inventory", new Vector2(16, 8 + (bg.Height + 20) * (float)_offsetY.Value), Color.White);

            Rectangle itemsListRectangle = new Rectangle(16, 31, 171, 222);
            Vector2 transformLT = Vector2.Transform(itemsListRectangle.Location.ToVector2(), batch.Transform);
            Vector2 transformRB = Vector2.Transform(new Vector2(itemsListRectangle.Right, itemsListRectangle.Bottom), batch.Transform);

            batch.Scissor = new Rectangle(
                (int)transformLT.X,
                (int)transformLT.Y,
                (int)(transformRB.X - transformLT.X),
                (int)(transformRB.Y - transformLT.Y));

            itemsListRectangle.Y += 6;
            itemsListRectangle.Height -= 12;

            Vector3 scale, translation;
            Quaternion rot;
            batch.Transform.Decompose(out scale, out rot, out translation);
            batch.Transform = batch.Transform * Matrix.CreateTranslation(0, (int)-_scroll * scale.X, 0);

            if(_world.Player.Inventory.OccupiedSlots == 0)
            {
                Vector2 measure = font.Measure(16, "No items in inventory");
                batch.Text(font, 16, "No items in inventory", itemsListRectangle.Center.ToVector2() - measure / 2 + new Vector2(0, (bg.Height + 20) * (float)_offsetY.Value), Color.White * 0.4f);
            }
            else
            {
                for(int i = 0; i < _world.Player.Inventory.OccupiedSlots; i++)
                {
                    Inventory.ItemSlotReference reference = _world.Player.Inventory[i];
                    int y = itemsListRectangle.Y + i * 15 + (int)((bg.Height + 20) * _offsetY.Value);
                    bool selected = _selectedSlot == i;

                    batch.Texture(
                        new Vector2(itemsListRectangle.X + 6, y), 
                        _world.Assets.Get<Texture2D>(reference.ReferencedItem.SmallIcon), 
                        Color.White * (selected ? 0.8f : 0.6f));

                    batch.Text(font, 
                        16, 
                        reference.ReferencedItem.Name, 
                        new Vector2(itemsListRectangle.X + 18, y - 8),
                        Color.White * (selected ? 0.8f : 0.6f));
                }
            }

            batch.Transform = batch.Transform * Matrix.CreateTranslation(0, (int)_scroll * scale.X, 0);
            batch.Scissor = null;
        }

        public static IEnumerator<ICoroutineOperation> Show(WorldState world, LivingEntity entity, OnInventoryUICompleted completeEvent = null)
        {
            InventoryUI inventory = new InventoryUI(world, entity);
            inventory._completedEvent = completeEvent;
            return world.Coroutine.Start(inventory.ShowCoroutine);
        }
    }
}
