using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.RPG.Gameplay.World;

namespace TeamStor.RPG.Gameplay
{
    /// <summary>
    /// Entity inventory.
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// Special slot number that is always guaranteed to be empty.
        /// </summary>
        public const int EMPTY_SLOT = -1;

        /// <summary>
        /// Reference for an item slot in an inventory.
        /// NOTE: if an item is removed at an earlier slot than this, this slot reference won't reference the same item anymore.
        /// </summary>
        public struct ItemSlotReference
        {
            /// <summary>
            /// Inventory this item is in.
            /// </summary>
            public Inventory Inventory
            {
                get; private set;
            }

            /// <summary>
            /// The slot in the inventory.
            /// If this is Inventory.EMPTY_SLOT then this always returns null.
            /// </summary>
            public int Slot
            {
                get; private set;
            }

            /// <summary>
            /// Item this item reference is referencing.
            /// </summary>
            public Item ReferencedItem
            {
                get
                {
                    if(Inventory.OccupiedSlots <= Slot)
                        return null;

                    return Inventory._slots[Slot];
                }
            }

            /// <summary>
            /// If this is a reference to an empty slot.
            /// </summary>
            public bool IsEmptyReference
            {
                get
                {
                    return ReferencedItem == null;
                }
            }

            public ItemSlotReference(Inventory inventory, int slot)
            {
                Inventory = inventory;
                Slot = slot;
            }

            public static implicit operator Item(ItemSlotReference r)
            {
                return r.ReferencedItem;
            }
        }

        private List<Item> _slots = new List<Item>();

        /// <summary>
        /// The entity this inventory belongs to.
        /// </summary>
        public LivingEntity Owner
        {
            get; private set;
        }

        private Dictionary<InventoryEquipSlot, int> _equips = new Dictionary<InventoryEquipSlot, int>();

        public Inventory(LivingEntity entity)
        {
            Owner = entity;

            foreach(InventoryEquipSlot slot in Enum.GetValues(typeof(InventoryEquipSlot)))
                _equips.Add(slot, EMPTY_SLOT);
        }

        public ItemSlotReference this[int slot]
        {
            get
            {
                if(slot == EMPTY_SLOT)
                    return new ItemSlotReference(this, EMPTY_SLOT);

                return new ItemSlotReference(this, slot);
            }
        }

        public ItemSlotReference this[InventoryEquipSlot slot]
        {
            get
            {
                if(this[_equips[slot]].IsEmptyReference || !(this[_equips[slot]].ReferencedItem.EquippableIn.HasFlag(slot)))
                    return this[EMPTY_SLOT];

                return this[_equips[slot]];
            }

            set
            {
                if(value.IsEmptyReference || !value.ReferencedItem.EquippableIn.HasFlag(slot))
                    _equips[slot] = EMPTY_SLOT;
                else
                    _equips[slot] = value.Slot;
            }
        }

        /// <summary>
        /// Pushes/adds a new item to the inventory.
        /// If the inventory is full this will return an empty reference.
        /// You can check if the item was added successfully by checking ItemSlotReference.IsEmptyReference
        /// </summary>
        /// <param name="item">The item to add to the inventory.</param>
        /// <returns>The reference to the slot the item was pushed to, or an empty reference.</returns>
        public ItemSlotReference Push(Item item)
        {
            if(IsFull)
                return this[EMPTY_SLOT];

            _slots.Add(item);
            return this[_slots.Count - 1];
        }

        /// <summary>
        /// If the inventory is full.
        /// </summary>
        public bool IsFull
        {
            get
            {
                return _slots.Count == MAX_SLOTS;
            }
        }

        /// <summary>
        /// Amount of inventory slots that have been taken up already.
        /// </summary>
        public int OccupiedSlots
        {
            get
            {
                return _slots.Count;
            }
        }

        /// <summary>
        /// Removes the item at a specified slot.
        /// </summary>
        /// <returns>If an item was actually removed.</returns>
        public bool PopAt(int slot)
        {
            if(slot == EMPTY_SLOT)
                return true;
            if(slot >= MAX_SLOTS)
                return false;
            if(slot <= OccupiedSlots)
                return false;

            _slots.RemoveAt(slot);
            return true;
        }

        /// <summary>
        /// Removes the last item in the inventory.
        /// </summary>
        /// <returns>If an item was removed.</returns>
        public bool Pop()
        {
            if(OccupiedSlots == 0)
                return false;

            _slots.RemoveAt(_slots.Count - 1);
            return true;
        }
    }
}
