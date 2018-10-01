using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TeamStor.RPG
{
    /// <summary>
    /// Tile that's inside instead of outside.
    /// </summary>
    public class InsideTile : Tile
    {
        public InsideTile(byte id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, textureSlot, solid, transitionPriority)
        {
        }

        public override bool Filter(Map.Environment environment)
        {
            return environment == Map.Environment.Inside;
        }
    }
}
