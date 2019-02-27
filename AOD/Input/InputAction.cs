using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD
{
    /// <summary>
    /// Input action types.
    /// </summary>
    public enum InputAction
    {
        Up,
        Down,
        Left,
        Right,

        /// <summary>
        /// Used for interacting with NPCs, signs, doors/map portals, etc.
        /// </summary>
        Action,

        /// <summary>
        /// Used for exiting out of UI.
        /// </summary>
        Back,

        /// <summary>
        /// Used for opening the inventory UI.
        /// </summary>
        Inventory,

        /// <summary>
        /// Used for opening the spells UI.
        /// </summary>
        Spells,

        /// <summary>
        /// Used for opening the player UI.
        /// </summary>
        Player,

        /// <summary>
        /// Used to switch tabs in UIs.
        /// </summary>
        SwitchTabLeft,

        /// <summary>
        /// Used to switch tabs in UIs.
        /// </summary>
        SwitchTabRight,

        /// <summary>
        /// Used for running.
        /// </summary>
        Run
    }
}
