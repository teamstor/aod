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
    /// Shows an editable inventory for an entity.
    /// </summary>
    public class InventoryUI
    {
        private bool _closed = false;
        private bool _transitioning = false;
        
        private bool _rightPaneSelected = false;

        private class PaneAction
        {
            public delegate void OnActionDelegate(PaneAction a, LivingEntity entity, Inventory.ItemSlotReference item, InventoryUI ui);

            public string Name;
            public OnActionDelegate OnAction { get; private set; }

            public PaneAction(string name, OnActionDelegate onAction)
            {
                Name = name;
                OnAction = onAction;
            }
        }

        private List<PaneAction> _actions = new List<PaneAction>();
        private int _selectedAction = 0;

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
        /// Offset of the menu.
        /// </summary>
        public float MenuOffset
        {
            get
            {
                return _offsetY;
            }
        }

        /// <summary>
        /// Creates a standalone inventory UI that needs to be updated manually.
        /// </summary>
        /// <param name="entity">The entity to use the inventory with.</param>
        public InventoryUI(LivingEntity entity, GameState state, bool noOffset = false)
        {
            _entity = entity;
            _offsetY = new TweenedDouble(state.Game, noOffset ? 0.0 : 1.0);
            _state = state;

            state.Coroutine.Start(ShowCoroutine);
            if(_entity.Inventory.OccupiedSlots == 0)
                _selectedSlot = -1;
            state.Coroutine.AddExisting(ChangeSelectedSlot(_selectedSlot));
        }

        private InventoryUI(WorldState world, LivingEntity entity, bool noOffset = false)
        {
            _state = world;
            _entity = entity;

            _offsetY = new TweenedDouble(world.Game, noOffset ? 0.0 : 1.0);
            if(_entity.Inventory.OccupiedSlots == 0)
                _selectedSlot = -1;
            _state.Coroutine.AddExisting(ChangeSelectedSlot(_selectedSlot));
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
                yield return null;

            if(_transitioning)
                yield return Wait.Seconds(_state.Game, 0.1);
            else
            {
                _offsetY.TweenTo(1.0, TweenEaseType.EaseInQuad, 0.1);
                while(!_offsetY.IsComplete)
                    yield return null;
            }

            if(_state is WorldState)
            {
                if(!_transitioning)
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
            if(selectedSlot != _selectedSlot)
            {
                _selectedSlot = -1;

                int y = 37 + Math.Max(0, selectedSlot * 15);
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
            }

            _selectedSlot = selectedSlot;
            _selectedAction = 0;
            
            _actions.Clear();

            if(_selectedSlot != -1)
            {
                _actions.Add(new PaneAction("Drop", (a, ent, item, ui) =>
                {
                    foreach(InventoryEquipSlot islot in Enum.GetValues(typeof(InventoryEquipSlot)))
                    {
                        if(islot != InventoryEquipSlot.None && ent.Inventory[islot].Slot == item.Slot)
                            ent.Inventory[islot] = ent.Inventory[Inventory.EMPTY_SLOT];
                    }

                    ent.Inventory.PopAt(item.Slot);
                    
                    int slot = ui._selectedSlot;
                    ui._selectedSlot = -1;
                    
                    if(slot >= ent.Inventory.OccupiedSlots)
                        ent.World.Coroutine.AddExisting(ChangeSelectedSlot(slot - 1));
                    else
                        ent.World.Coroutine.AddExisting(ChangeSelectedSlot(slot));
                }));
                
                Inventory.ItemSlotReference i = _entity.Inventory[_selectedSlot];
                foreach(InventoryEquipSlot slot in Enum.GetValues(typeof(InventoryEquipSlot)))
                {
                    if(slot != InventoryEquipSlot.None && i.ReferencedItem.EquippableIn.HasFlag(slot))
                    {
                        string text = "Equip ";
                        switch(slot)
                        {
                            case InventoryEquipSlot.Head:
                                text += "on head";
                                break;
                            case InventoryEquipSlot.Chest:
                                text += "chest";
                                break;
                            case InventoryEquipSlot.Leggings:
                                text += "leggings";
                                break;
                            case InventoryEquipSlot.Boots:
                                text += "boots";
                                break;
                            
                            case InventoryEquipSlot.Weapon:
                                text += "weapon";
                                break;
                            case InventoryEquipSlot.Ring:
                                text += "ring";
                                break;
                        }

                        if(i.Inventory[slot].Slot == i.Slot)
                            text = text.
                                Replace("Equip", "Unequip").
                                Replace("on ", "from ");
                        
                        _actions.Add(new PaneAction(text, (a, ent, item, ui) =>
                        {
                            if(ent.Inventory[slot].Slot == item.Slot)
                                ent.Inventory[slot] = ent.Inventory[Inventory.EMPTY_SLOT];
                            else
                            {
                                ent.Inventory[slot] = ent.Inventory[item.Slot];

                                foreach(InventoryEquipSlot slot_2 in Enum.GetValues(typeof(InventoryEquipSlot)))
                                {
                                    if(slot_2 != InventoryEquipSlot.None && slot_2 != slot && ent.Inventory[slot_2].Slot == item.Slot)
                                        ent.Inventory[slot_2] = ent.Inventory[Inventory.EMPTY_SLOT];
                                }
                            }
                        }));
                    }
                }
            }
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
            if(_offsetY == 0)
            {
                if(!_closed && _state is WorldState && InputMap.FindMapping(InputAction.Player).Pressed(_state.Input))
                {
                    PlayerUI.Show(_state as WorldState, null, true);
                    _transitioning = true;
                }

                if(InputMap.FindMapping(InputAction.Back).Pressed(_state.Input) ||
                   (_state is WorldState && InputMap.FindMapping(InputAction.Player).Pressed(_state.Input)))
                    _closed = true;

                if(_rightPaneSelected)
                {
                    if(InputMap.FindMapping(InputAction.Left).Pressed(_state.Input))
                    {
                        if(_selectedAction > 0)
                            _selectedAction--;
                        else
                            _rightPaneSelected = false;
                    }

                    if(InputMap.FindMapping(InputAction.Right).Pressed(_state.Input))
                    {
                        if(_selectedAction < _actions.Count - 1)
                            _selectedAction++;
                        else
                            _rightPaneSelected = false;
                    }

                    if(InputMap.FindMapping(InputAction.Action).Pressed(_state.Input) && _actions.Count > 0)
                    {
                        int oldAction = _selectedAction;
                        _actions[_selectedAction].OnAction(_actions[_selectedAction], _entity, _entity.Inventory[_selectedSlot], this);

                        // update actions manually
                        IEnumerator<ICoroutineOperation> func = ChangeSelectedSlot(_selectedSlot);
                        while(func.MoveNext())
                        {
                        }

                        _selectedAction = oldAction;
                    }
                }
                else if(_selectedSlot != -1 && !_rightPaneSelected)
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

                    if(InputMap.FindMapping(InputAction.Left).Pressed(_state.Input))
                    {
                        _rightPaneSelected = true;
                        _selectedAction = _actions.Count - 1;
                    }

                    if(InputMap.FindMapping(InputAction.Right).Pressed(_state.Input))
                    {
                        _rightPaneSelected = true;
                        _selectedAction = 0;
                    }
                }
            }
        }

        private void DrawHook(object sender, WorldState.DrawEventArgs args)
        {
            WorldState world = sender as WorldState;
            SpriteBatch batch = args.Batch;
            Vector2 screenSize = args.ScreenSize;

            Texture2D bg = _state.Assets.Get<Texture2D>("ui/inventory.png");
            batch.Texture(new Vector2(0, 270 - bg.Height + (bg.Height + 20) * (float)_offsetY.Value), bg, Color.White);

            Font font = _state.Assets.Get<Font>("fonts/bitcell.ttf");

            if(_state is CombatState)
                batch.Text(font, 
                    16, 
                    "Inventory", 
                    new Vector2(16, 8 + (bg.Height + 20) * (float)_offsetY.Value), 
                    Color.White);
            else
            {
                batch.Text(font, 
                    16, 
                    "[" + InputMap.FindMapping(InputAction.Inventory).Key + "] Inventory", 
                    new Vector2(16, 8 + (bg.Height + 20) * (float)_offsetY.Value), 
                    Color.White);
                
                // only the player uses the inventory for now
                batch.Text(font, 
                    16, 
                    "[" + InputMap.FindMapping(InputAction.Player).Key + "] " + _entity.Name, 
                    new Vector2(16 + (int)font.Measure(16, "[" + InputMap.FindMapping(InputAction.Inventory).Key + "] Inventory").X + 6, 8 + (bg.Height + 20) * (float)_offsetY.Value), 
                    Color.White * 0.6f);
            }

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
                    bool selected = _selectedSlot == i && !_rightPaneSelected;

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

            infoRectangle.Y += 40;

            if(_entity.Inventory.OccupiedSlots > 0 && _selectedSlot != -1)
            {
                Inventory.ItemSlotReference reference = _entity.Inventory[_selectedSlot];

                batch.Texture(
                    new Vector2(infoRectangle.X + infoRectangle.Width / 2 - 16, infoRectangle.Y),
                    _state.Assets.Get<Texture2D>(reference.ReferencedItem.Icon),
                    Color.White);

                Vector2 measure = font.Measure(16, reference.ReferencedItem.Name);
                batch.Text(font, 16, reference.ReferencedItem.Name, new Vector2(infoRectangle.X + infoRectangle.Width / 2 - measure.X / 2, infoRectangle.Y + 30), Color.White);

                int y = 44;
                foreach(string s in reference.ReferencedItem.Description.Split('\n'))
                {
                    measure = font.Measure(16, s);
                    batch.Text(font, 16, s, new Vector2(infoRectangle.X + infoRectangle.Width / 2 - measure.X / 2, infoRectangle.Y + y), Color.White * 0.7f);

                    y += 12;
                }

                if(reference.ReferencedItem is ArmorItem)
                {
                    Rectangle blockRectangle = new Rectangle(infoRectangle.X + infoRectangle.Width / 2 - 140 / 2, infoRectangle.Y + 90, 140, 16);

                    batch.Rectangle(blockRectangle, Color.White * 0.1f);
                    blockRectangle.Width = (int)(140 * ((float)(reference.ReferencedItem as ArmorItem).ArmorValue / 100));
                    batch.Rectangle(blockRectangle, Color.White * 0.3f);
                    blockRectangle.Width = 140;

                    batch.Text(font, 16, 
                        ((reference.ReferencedItem as ArmorItem).Protection == ArmorItem.ProtectionType.Magical ? "Magical" : "Physical") +
                        " Armor: +" + 
                        (reference.ReferencedItem as ArmorItem).ArmorValue, 
                        new Vector2(blockRectangle.X + 4, blockRectangle.Y - 4), Color.White);
                }

                int x = 0;
                batch.Text(font, 16, "Actions", new Vector2(infoRectangle.X + x + 10, infoRectangle.Y + infoRectangle.Height - 72 - 8), Color.White);
                
                foreach(PaneAction action in _actions)
                {
                    bool selected = _rightPaneSelected && _selectedAction == _actions.IndexOf(action);
                    //float alpha = selected ? 0.8f + (float)Math.Sin(_state.Game.Time * 10) * 0.2f : 0.6f;
                    float alpha = selected ? 0.8f : 0.6f;

                    batch.Text(font, 16, action.Name, new Vector2(infoRectangle.X + x + 10, infoRectangle.Y + infoRectangle.Height - 60 - 8), Color.White * alpha);
                    if(selected)
                        batch.Texture(new Vector2(infoRectangle.X + x, infoRectangle.Y + infoRectangle.Height - 60), _state.Assets.Get<Texture2D>("ui/arrow.png"), Color.White);

                    x += (int)font.Measure(16, action.Name).X + 16;
                }
            }
        }

        public static IEnumerator<ICoroutineOperation> Show(WorldState world, LivingEntity entity, OnInventoryUICompleted completeEvent = null, bool noOffset = false)
        {
            InventoryUI inventory = new InventoryUI(world, entity, noOffset);
            inventory._completedEvent = completeEvent;
            return world.Coroutine.Start(inventory.ShowCoroutine);
        }
    }
}