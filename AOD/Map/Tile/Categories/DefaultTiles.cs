using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD
{
    public static class DefaultTiles
    {
        public static Tile EmptyDecoration = new Tile("", Tile.MapLayer.Decoration, "Empty", "");
        public static Tile EmptyNPC = new Tile("", Tile.MapLayer.NPC, "Empty", "");
        public static Tile EmptyControl = new Tile("", Tile.MapLayer.Control, "Empty", "");
    }
}
