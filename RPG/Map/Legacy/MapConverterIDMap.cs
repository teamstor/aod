using System.Collections.Generic;
using TeamStor.RPG.Gameplay;

namespace TeamStor.RPG.Legacy
{
    public static class MapConverterIDMap
    {
        public static Dictionary<int, Tile> TerrainMap = new Dictionary<int, Tile>()
        {
            { 0, WaterTiles.DeepWaterOrVoid },
            { 1, WaterTiles.ShallowWater },
            { 10, GroundTiles.Grass },
            { 11, GroundTiles.Dirt },
            { 12, GroundTiles.Stone },
            { 20, InsideTiles.Floor },
            { 50, CityTiles.Road },
            { 51, CityTiles.HouseStairs },
            { 52, InsideTiles.Doormat }
        };

        public static Dictionary<int, Tile> DecorationMap = new Dictionary<int, Tile>()
        {
            { 0, DefaultTiles.EmptyDecoration },
            { 10, FoliageTiles.Tree },
            { 11, FoliageTiles.Bush },
            { 20, CityTiles.Wall },
            { 21, CityTiles.Bricks },
            { 30, CityTiles.RoofOrange },
            { 31, CityTiles.RoofBlue },
            { 32, CityTiles.RoofPurple },
            { 33, CityTiles.RoofRed },
            { 34, CityTiles.RoofBeam },
            { 35, CityTiles.WallBeam },
            { 36, CityTiles.WindowOrange },
            { 37, CityTiles.WindowBlue },
            { 38, CityTiles.WindowPurple },
            { 39, CityTiles.WindowRed },
            { 40, CityTiles.WindowStone },

            { 42, CityTiles.Door },
            { 43, CityTiles.DoorStone },
            { 44, CityTiles.Sign },

            { 50, InsideTiles.Panel },
            { 51, InsideTiles.Wall },
            { 52, InsideTiles.Door },

            { 60, CityTiles.Waterwheel }
        };

        public static Dictionary<int, Tile> NPCMap = new Dictionary<int, Tile>()
        {
            { 0, DefaultTiles.EmptyNPC },
            { 1, NPCs.GreenPig.TileTemplate }
        };

        public static Dictionary<int, Tile> ControlMap = new Dictionary<int, Tile>()
        {
            { 0, DefaultTiles.EmptyControl },
            { 10, ControlTiles.TextBox },
            { 30, ControlTiles.Spawnpoint },
            { 40, ControlTiles.Barrier },
            { 41, ControlTiles.InvertedBarrier },
            { 60, ControlTiles.MapPortal }
        }; 
    }
}
