using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG.Map
{
    public class Tile
    {
        /// <summary>
        /// Layer that a tile is on. 
        /// Layers are stacked on top of each other with "terrain" being the lowest and "control" being the highest.
        /// </summary>
        public enum Layer
        {
            /// <summary>
            /// The actual ground layer of the map. Tiles on this layer can be walked upon and no tile can be empty on this layer.
            /// Terrain tiles blend together with each other.
            /// </summary>
            Terrain = 0,
            /// <summary>
            /// Decoration tiles. These can be either solid or walk-through.
            /// </summary>
            Decoration = 1,
            /// <summary>
            /// Other characters in the world.
            /// </summary>
            NPC = 2,
            /// <summary>
            /// Control tiles are invisible tiles that control things such as enemy encounters or text boxes.
            /// </summary>
            Control = 3
        }
    }
}
