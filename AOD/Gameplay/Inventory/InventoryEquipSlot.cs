using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD.Gameplay
{
    /// <summary>
    /// Slot an item can be equipped in. This can be more than just one value.
    /// </summary>
    [Flags]
    public enum InventoryEquipSlot : byte
    {
        None = 0,

        Head = 1,
        Chest = 2,
        Leggings = 4,
        Boots = 8,
        Ring = 16,

        Weapon = 32
    }
}
