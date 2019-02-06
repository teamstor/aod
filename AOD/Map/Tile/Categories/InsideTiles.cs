using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD
{
    public static class InsideTiles
    {
        public static Tile Floor = new Tile("inside/floor", Tile.MapLayer.Terrain, "Floor", "tiles/inside/floor.png", false, -1);
        public static Tile Panel = new Tile("inside/panel", Tile.MapLayer.Decoration, "Panel", "tiles/inside/panel.png", true);
        public static Tile Wall = new Tile("inside/wall", Tile.MapLayer.Decoration, "Wall", "tiles/inside/wall.png", true);

        public static Tile Door = new DrawOffsetTile("inside/door", Tile.MapLayer.Decoration, "Door", "tiles/inside/door.png", new Point(0, -1), true);
        public static Tile Doormat = new DoormatTile("inside/doormat", Tile.MapLayer.Terrain, "Doormat", "tiles/inside/doormat.png", false, -1);
    }
}
