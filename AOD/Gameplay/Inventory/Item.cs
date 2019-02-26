using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD.Gameplay
{
    /// <summary>
    /// Item that can be stored in an inventory or on the ground.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Item list.
        /// </summary>
        public static Dictionary<string, Item> Items
        {
            get; private set;
        } = new Dictionary<string, Item>();

        public static ArmorItem TestItem = new ArmorItem("test", "Test Item #1", "This is the legendary test item 1.\nWow. Asså wow.", "items/icons/test/8x8.png", "items/icons/test/32x32.png", ArmorItem.ProtectionType.Physical, 4, InventoryEquipSlot.Head);
        public static Item TestItem2 = new Item("test2", "Test Item #2", "This is the infamous test item 2.\nIt's not as good as test item 1.", "items/icons/test/8x8.png", "items/icons/test/32x32.png", InventoryEquipSlot.None);

        /// <summary>
        /// ID of the item.
        /// </summary>
        public virtual string ID
        {
            get; private set;
        }

        /// <summary>
        /// Name of the item visible in the UI.
        /// </summary>
        public virtual string Name
        {
            get; private set;
        }

        /// <summary>
        /// Description of the item visible in the UI.
        /// </summary>
        public virtual string Description
        {
            get; private set;
        }

        /// <summary>
        /// 8x8 icon next to the name in the item list.
        /// </summary>
        public virtual string SmallIcon
        {
            get; private set;
        }

        /// <summary>
        /// 32x32 icon that shows up when the item is selected.
        /// </summary>
        public virtual string Icon
        {
            get; private set;
        }

        /// <summary>
        /// Slots this item is equippable in.
        /// </summary>
        public virtual InventoryEquipSlot EquippableIn
        {
            get;
        } = InventoryEquipSlot.None;

        public Item(string id, string name, string description, string smallIcon, string icon, InventoryEquipSlot equippableIn, bool createGlobally = true)
        {
            ID = id;
            Name = name;
            Description = description;
            SmallIcon = smallIcon;
            Icon = icon;
            EquippableIn = equippableIn;

            if(createGlobally)
                Items.Add(id, this);
        }

        /// <summary>
        /// Finds an item by name.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <returns>The </returns>
        public static Item FindByName(string name)
        {
            foreach(Item item in Items.Values)
            {
                if(item.Name == name)
                    return item;
            }

            throw new ArgumentException("Item with name " + name + " not found.");
        }
    }
}
