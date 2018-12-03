using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG
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
        /// Used for running.
        /// </summary>
        Run
    }
}
