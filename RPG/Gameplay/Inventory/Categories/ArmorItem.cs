using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG.Gameplay
{
    /// <summary>
    /// An item adding to the entities armor value.
    /// </summary>
    public class ArmorItem : Item
    {
        /// <summary>
        /// The type of damage this armor piece blocks.
        /// </summary>
        public enum ProtectionType
        {
            Physical,
            Magical
        }

        /// <summary>
        /// Type of damage this armor piece blocks.
        /// </summary>
        public virtual ProtectionType Protection
        {
            get; private set;
        }

        /// <summary>
        /// Armor value.
        /// </summary>
        public virtual int ArmorValue
        {
            get; private set;
        }

        public ArmorItem(string id, string name, string smallIcon, string icon, ProtectionType protectionType, int armorValue, bool createGlobally = true) : base(id, name, smallIcon, icon, createGlobally)
        {
            Protection = protectionType;
            ArmorValue = armorValue;
        }
    }
}
