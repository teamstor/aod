using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG
{
    public static class CityTiles
    {
        public static Tile Wall = new Tile("city/wall", Tile.MapLayer.Decoration, "Wall", "tiles/city/wall_wood.png", true);
        public static Tile Bricks = new Tile("city/bricks", Tile.MapLayer.Decoration, "Bricks", "tiles/city/wall_bricks.png", true);
        public static Tile StoneBricks = new Tile("city/stone-bricks", Tile.MapLayer.Decoration, "Stone Bricks", "tiles/city/wall_stone.png", true);

        public static Tile HouseStairs = new Tile("city/house-stairs", Tile.MapLayer.Terrain, "House Stairs", "tiles/city/house_stairs.png", false, -1);

        public static Tile Door = new DrawOffsetTile("city/door", Tile.MapLayer.Decoration, "Door", "tiles/city/door_wood.png", new Point(0, -1), true);
        public static Tile DoorStone = new DrawOffsetTile("city/door-stone", Tile.MapLayer.Decoration, "Door", "tiles/city/door_stone.png", new Point(0, -1), true);

        public static Tile Road = new RoadTile("city/road", Tile.MapLayer.Terrain, "Road", "tiles/city/road.png", false, 2000);

        public static Tile Sign = new Tile("city/sign", Tile.MapLayer.Decoration, "Sign", "tiles/decoration/sign.png", true);

        public static Tile RoofBlue = new Tile("city/roof-blue", Tile.MapLayer.Decoration, "Blue Roof", "tiles/city/roof/blue.png", true);
        public static Tile RoofOrange = new Tile("city/roof-orange", Tile.MapLayer.Decoration, "Orange Roof", "tiles/city/roof/orange.png", true);
        public static Tile RoofPurple = new Tile("city/roof-purple", Tile.MapLayer.Decoration, "Purple Roof", "tiles/city/roof/purple.png", true);
        public static Tile RoofRed = new Tile("city/roof-red", Tile.MapLayer.Decoration, "Red Roof", "tiles/city/roof/red.png", true);

        public static Tile WindowBlue = new Tile("city/window-blue", Tile.MapLayer.Decoration, "Blue Window", "tiles/city/window/blue.png", true);
        public static Tile WindowOrange = new Tile("city/window-orange", Tile.MapLayer.Decoration, "Orange Window", "tiles/city/window/orange.png", true);
        public static Tile WindowPurple = new Tile("city/window-purple", Tile.MapLayer.Decoration, "Purple Window", "tiles/city/window/purple.png", true);
        public static Tile WindowRed = new Tile("city/window-red", Tile.MapLayer.Decoration, "Red Window", "tiles/city/window/red.png", true);

        public static Tile WindowStone = new Tile("city/window-stone", Tile.MapLayer.Decoration, "Stone Window", "tiles/city/window/stone.png", true);

        public static Tile RoofBeam = new RoofBeamTile("city/beam-roof", Tile.MapLayer.Decoration, "Roof Beam", "tiles/city/beam/side.png", true);
        public static Tile WallBeam = new BeamTile("city/beam-wall", Tile.MapLayer.Decoration, "Wall Beam", "tiles/city/beam/narrow.png", true);

        public static Tile Waterwheel = new WaterwheelTile("city/waterwheel", Tile.MapLayer.Decoration, "Waterwheel", "tiles/city/waterwheel.png", true);
    }
}
