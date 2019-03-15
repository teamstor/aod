using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD.Gameplay
{
    /// <summary>
    /// A melee weapon scaling with either strength or agility.
    /// </summary>
    public class WeaponItem : CombatItem
    {
        /// <summary>
        /// The minimum and maximum base damage this weapon does.
        /// </summary>
        public Tuple<int, int> DamageRange { get; private set; }

        /// <summary>
        /// If this item scales based on agility rather than strength.
        /// </summary>
        public bool UsesAgility { get; private set; }

        public WeaponItem(string id, string name, string description, string smallIcon, string icon, InventoryEquipSlot equippableIn, Tuple<int, int> damageRange, bool isAgility, bool createGlobally = true) : 
            base(id, name, description, smallIcon, icon, equippableIn, createGlobally)
        {
            DamageRange = damageRange;
            UsesAgility = isAgility;
        }
    }
}
