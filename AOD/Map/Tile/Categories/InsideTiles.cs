using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.AOD.Attributes;
using TeamStor.AOD.Editor;

namespace TeamStor.AOD
{
    public static class InsideTiles
    {
        public static Tile Floor = new Tile("inside/floor", Tile.MapLayer.Terrain, "Floor", "tiles/inside/floor.png", false, -1);
        public static Tile Panel = new Tile("inside/panel", Tile.MapLayer.Decoration, "Panel", "tiles/inside/panel.png", true);
        public static Tile Wall = new Tile("inside/wall", Tile.MapLayer.Decoration, "Wall", "tiles/inside/wall.png", true);

        public static Tile Door = new DrawOffsetTile("inside/door", Tile.MapLayer.Decoration, "Door", "tiles/inside/door.png", new Point(0, -1), true);
        public static Tile Doormat = new DoormatTile("inside/doormat", Tile.MapLayer.Terrain, "Doormat", "tiles/inside/doormat.png", false, -1);

        public static Tile Bookshelf = new DrawOffsetTile("inside/bookshelf", Tile.MapLayer.Decoration, "Bookshelf", "tiles/inside/bookshelf.png", new Point(0, -1), true);
        public static Tile Chair = new Tile("inside/chair", Tile.MapLayer.Decoration, "Chair", "tiles/inside/chair.png", true).
            AttributeEditableAttributes((MapEditorState s, ref int y) =>
            {
                return new TileAttributeEditor[] { new BoolAttributeEditor("flip-h", s, false, ref y) };
            });

        public static Tile Table = new Tile("inside/table", Tile.MapLayer.Decoration, "Table", "tiles/inside/table.png", true);
    }
}
