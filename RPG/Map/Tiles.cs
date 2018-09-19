﻿using Microsoft.Xna.Framework;

namespace TeamStor.RPG
{
	/// <summary>
	/// Contains a list of tiles.
	/// </summary>
	public static class Tiles
	{
        #region Terrain
        public static class Terrain
		{
			public static Tile Water = new AnimatedTile(0, Tile.MapLayer.Terrain, "Water", new Point(0, 0), 4, 2, true);

            public static Tile Grass = new VariationsTile(10, Tile.MapLayer.Terrain, "Grass", 
                new Point[] {
                    // 5/11 chance
                    new Point(4, 0),
                    new Point(4, 0),
                    new Point(4, 0),
                    new Point(4, 0),
                    new Point(4, 0),

                    /// 5/11 chance
                    new Point(5, 0),
                    new Point(5, 0),
                    new Point(5, 0),
                    new Point(5, 0),
                    new Point(5, 0),

                    /// 1/11 chance
                    new Point(6, 0)
			});
            public static Tile Dirt = new VariationsTile(11, Tile.MapLayer.Terrain, "Dirt",
                new Point[] {
                    // 4/5 chance
                    new Point(7, 0),
                    new Point(7, 0),
                    new Point(7, 0),
                    new Point(7, 0),

                    // 1/5 chance
                    new Point(8, 0)
			});
			public static Tile Stone = new Tile(12, Tile.MapLayer.Terrain, "Stone", new Point(9, 0), true);
			
			public static Tile Wood = new Tile(20, Tile.MapLayer.Terrain, "Wood", new Point(1, 1), true, -1);
			
			public static Tile RoadCity = new Tile(50, Tile.MapLayer.Terrain, "Road (City)", new Point(0, 1), false, 500);
        }
        #endregion

        #region Decoration
        public static class Decoration
		{
			public static Tile Empty = new Tile(0, Tile.MapLayer.Decoration, "Empty", new Point(0, 0));
			
			public static Tile Tree = new DoubleTile(10, Tile.MapLayer.Decoration, "Tree", new Point(0, 1), true);
			
			public static Tile Wood = new TransitionIntoTerrainTile(20, Tile.MapLayer.Decoration, "Wood", new Point(2, 0), true);
            public static Tile Brick = new Tile(21, Tile.MapLayer.Decoration, "Brick", new Point(3, 0), true);
			public static Tile BrickStone = new Tile(22, Tile.MapLayer.Decoration, "Brick (Stone)", new Point(4, 0), true);

            public static Tile RoofOrange = new Tile(30, Tile.MapLayer.Decoration, "Roof (Orange)", new Point(3, 1), true);
            public static Tile RoofBlue = new Tile(31, Tile.MapLayer.Decoration, "Roof (Blue)", new Point(4, 1), true);
            public static Tile RoofPurple = new Tile(32, Tile.MapLayer.Decoration, "Roof (Purple)", new Point(5, 1), true);
            public static Tile RoofRed = new Tile(33, Tile.MapLayer.Decoration, "Roof (Red)", new Point(6, 1), true);

            public static Tile StairsWood = new TransitionIntoTerrainTile(40, Tile.MapLayer.Decoration, "Stairs (Wood)", new Point(2, 1));

            public static Tile DoorWood = new DoubleTile(50, Tile.MapLayer.Decoration, "Door (Wooden)", new Point(1, 1), true);
		}
        #endregion

        #region NPC
        public static class NPC
		{
			public static Tile Empty = new Tile(0, Tile.MapLayer.NPC, "Empty", new Point(0, 0));
		}
        #endregion

        #region Control
        public static class Control
		{
			public static Tile Empty = new Tile(0, Tile.MapLayer.Control, "Empty", new Point(0, 0));

            public static Tile TextBoxTrigger = new Tile(10, Tile.MapLayer.Control, "Textbox Trigger", new Point(0, 0));
            public static Tile TextBoxInteraction = new Tile(11, Tile.MapLayer.Control, "Textbox Interaction", new Point(1, 0));

            public static Tile EnemyTrigger = new Tile(20, Tile.MapLayer.Control, "Enemy Trigger", new Point(2, 0));

            public static Tile SpawnPoint = new Tile(30, Tile.MapLayer.Control, "Spawn Point", new Point(3, 0));

            public static Tile Barrier = new Tile(40, Tile.MapLayer.Control, "Barrier", new Point(4, 0), true);
            public static Tile InvertedBarrier = new Tile(41, Tile.MapLayer.Control, "Inverted Barrier", new Point(5, 0));
        }
        #endregion
    }
}