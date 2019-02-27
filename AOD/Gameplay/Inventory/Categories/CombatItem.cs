using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD.Gameplay
{
    /// <summary>
    /// An item usable in combat.
    /// </summary>
    public class CombatItem : Item
    {
        public CombatItem(string id, string name, string description, string smallIcon, string icon, InventoryEquipSlot equippableIn, bool createGlobally = true) : 
            base(id, name, description, smallIcon, icon, equippableIn, createGlobally)
        {
        }
    }
}
