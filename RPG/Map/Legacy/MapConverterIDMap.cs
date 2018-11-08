using System.Collections.Generic;

namespace TeamStor.RPG.Legacy
{
    public static class MapConverterIDMap
    {
        public static Dictionary<int, Tile> TerrainMap = new Dictionary<int, Tile>()
        {
            { 0, Tiles.Terrain.Water },
            { 1, Tiles.Terrain.ShallowWater },
            { 10, Tiles.Terrain.Grass },
            { 11, Tiles.Terrain.Dirt },
            { 12, Tiles.Terrain.Stone },
            { 20, Tiles.Terrain.Wood },
            { 50, Tiles.Terrain.RoadCity },
            { 51, Tiles.Terrain.StairsWood },
            { 52, Tiles.Terrain.Doormat }
        };

        public static Dictionary<int, Tile> DecorationMap = new Dictionary<int, Tile>()
        {
            { 0, Tiles.Decoration.Empty },
            { 10, Tiles.Decoration.Tree },
            { 11, Tiles.Decoration.Bush },
            { 20, Tiles.Decoration.Wood },
            { 21, Tiles.Decoration.Brick },
            { 30, Tiles.Decoration.RoofOrange },
            { 31, Tiles.Decoration.RoofBlue },
            { 32, Tiles.Decoration.RoofPurple },
            { 33, Tiles.Decoration.RoofRed },
            { 34, Tiles.Decoration.RoofBeam },
            { 35, Tiles.Decoration.Beam },
            { 36, Tiles.Decoration.WindowOrange },
            { 37, Tiles.Decoration.WindowBlue },
            { 38, Tiles.Decoration.WindowPurple },
            { 39, Tiles.Decoration.WindowRed },
            { 40, Tiles.Decoration.WindowStone },

            { 42, Tiles.Decoration.DoorWood },
            { 43, Tiles.Decoration.DoorStone },
            { 44, Tiles.Decoration.Sign },

            { 50, Tiles.Decoration.WoodenPanel },
            { 51, Tiles.Decoration.HouseWall },

            { 52, Tiles.Decoration.DoorWoodInside },
            { 60, Tiles.Decoration.Waterwheel }
        };

        public static Dictionary<int, Tile> ControlMap = new Dictionary<int, Tile>()
        {
            { 0, Tiles.Control.Empty },
            { 10, Tiles.Control.TextBoxTrigger },
            { 20, Tiles.Control.EnemyTrigger },
            { 30, Tiles.Control.SpawnPoint },
            { 40, Tiles.Control.Barrier },
            { 41, Tiles.Control.InvertedBarrier },
            { 60, Tiles.Control.MapPortal }
        };
    }
}
