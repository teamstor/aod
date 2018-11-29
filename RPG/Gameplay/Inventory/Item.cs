using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG.Gameplay
{
    /// <summary>
    /// Item that can be stored in an inventory or on the ground.
    /// </summary>
    public class Item
    {
        private string _id;
        private string _name;

        /// <summary>
        /// ID of the item.
        /// </summary>
        public virtual string ID
        {
            get { return _id;  }
        }

        /// <summary>
        /// Name of the item visible in the UI.
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Slots this item is equippable in.
        /// </summary>
        public virtual InventoryEquipSlot EquippableIn
        {
            get;
        } = InventoryEquipSlot.None;

        /// <summary>
        /// Item list.
        /// </summary>
        public static Dictionary<string, Item> Items
        {
            get; private set;
        } = new Dictionary<string, Item>();

        public Item(string id, string name, bool createGlobally = true)
        {
            _id = id;
            _name = name;

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
