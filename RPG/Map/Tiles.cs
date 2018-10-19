using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamStor.Engine;
using TeamStor.RPG.Gameplay;

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
            public class WaterTile : AnimatedTile
            {
                public WaterTile(byte id, MapLayer layer, string name, Point firstSlot, int slotCount, int fps, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, firstSlot, slotCount, fps, solid, transitionPriority)
                {
                }

                public override string Name(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
                {
                    if(environment == Map.Environment.Inside)
                        return "Void";
                    return base.Name(metadata, environment);
                }

                public override void Draw(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
                {
                    if(environment == Map.Environment.Inside)
                        game.Batch.Rectangle(new Rectangle(mapPos.X * 16, mapPos.Y * 16, 16, 16), Color.Black);
                    else
                        base.Draw(game, mapPos, map, metadata, environment, color);
                }
            }

            public static Tile Water = new WaterTile(0, Tile.MapLayer.Terrain, "Water", new Point(0, 0), 4, 2, true);

            public static Tile Grass = new GrassTile(10, Tile.MapLayer.Terrain, "Grass", 
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
			}, false, 1001);

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
			
			public static Tile Wood = new InsideTile(20, Tile.MapLayer.Terrain, "Wood", new Point(1, 1), false, -1);

            public static Tile RoadCity = new RoadCityTile(50, Tile.MapLayer.Terrain, "City Road", new Point(0, 1), false, 1002);
            public static Tile StairsWood = new Tile(51, Tile.MapLayer.Terrain, "Wooden Stairs", new Point(0, 2), false, -1);
            public static Tile Doormat = new InsideTile(52, Tile.MapLayer.Terrain, "Doormat", new Point(1, 2), false, -1);
        }
        #endregion

        #region Decoration
        public static class Decoration
		{
			public static Tile Empty = new Tile(0, Tile.MapLayer.Decoration, "Empty", new Point(0, 0));
			
            public class TreeTile : DoubleTile
            {
                public TreeTile(byte id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, textureSlot, solid, transitionPriority)
                {
                }

                public override Point TextureSlot(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
                {
                    if(environment == Map.Environment.SnowMountain)
                        return base.TextureSlot(metadata, environment) + new Point(0, 2);
                    return base.TextureSlot(metadata, environment);
                }
            }

			public static Tile Tree = new TreeTile(10, Tile.MapLayer.Decoration, "Tree", new Point(0, 1), true);
            public static Tile Bush = new BushTile(11, Tile.MapLayer.Decoration, "Bush", new Point(0, 4), true);

            public static Tile Wood = new Tile(20, Tile.MapLayer.Decoration, "Wood", new Point(2, 0), true);
            public static Tile Brick = new Tile(21, Tile.MapLayer.Decoration, "Bricks", new Point(3, 0), true);
			public static Tile BrickStone = new Tile(22, Tile.MapLayer.Decoration, "Stone Bricks", new Point(4, 0), true);

            public static Tile RoofOrange = new RoofTile(30, Tile.MapLayer.Decoration, "Orange Roof", new Point(3, 1), true);
            public static Tile RoofBlue = new RoofTile(31, Tile.MapLayer.Decoration, "Blue Roof", new Point(4, 1), true);
            public static Tile RoofPurple = new RoofTile(32, Tile.MapLayer.Decoration, "Purple Roof", new Point(5, 1), true);
            public static Tile RoofRed = new RoofTile(33, Tile.MapLayer.Decoration, "Red Roof", new Point(6, 1), true);
            public static Tile RoofBeam = new RoofBeamTile(34, Tile.MapLayer.Decoration, "Roof Beam", new Point(5, 0));
            public static Tile Beam = new BeamTile(35, Tile.MapLayer.Decoration, "Beam", new Point(7, 0));
            public static Tile WindowOrange = new Tile(36, Tile.MapLayer.Decoration, "Orange Window", new Point(4, 2), true);
            public static Tile WindowBlue = new Tile(37, Tile.MapLayer.Decoration, "Blue Window", new Point(5, 2), true);
            public static Tile WindowPurple = new Tile(38, Tile.MapLayer.Decoration, "Purple Window", new Point(6, 2), true);
            public static Tile WindowRed = new Tile(39, Tile.MapLayer.Decoration, "Red Window", new Point(7, 2), true);
            public static Tile WindowStone = new Tile(40, Tile.MapLayer.Decoration, "Stone Window", new Point(2, 1), true);

            public static Tile DoorWood = new DoubleTile(42, Tile.MapLayer.Decoration, "Wooden Door/Wood BG", new Point(1, 1), true);
            public static Tile DoorStone = new DoubleTile(43, Tile.MapLayer.Decoration, "Wooden Door/Stone BG", new Point(1, 3), true);
            public static Tile Sign = new Tile(44, Tile.MapLayer.Decoration, "Sign", new Point(8, 1), true);

            public static Tile WoodenPanel = new InsideTile(50, Tile.MapLayer.Decoration, "Wooden Panel", new Point(9, 0), true, -1);
            public static Tile HouseWall = new InsideTile(51, Tile.MapLayer.Decoration, "Wall", new Point(8, 0), true, -1);

            public class DoorInsideTile : DoubleTile
            {
                public DoorInsideTile(byte id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, textureSlot, solid, transitionPriority)
                {
                }

                public override bool Filter(Map.Environment environment)
                {
                    return environment == Map.Environment.Inside;
                }
            }

            public static Tile DoorWoodInside = new DoorInsideTile(53, Tile.MapLayer.Decoration, "Wooden Door", new Point(10, 1), true);

            public static Tile Waterwheel = new WaterwheelTile(60, Tile.MapLayer.Decoration, "Waterwheel", new Point(3, 5), true);
        }
        #endregion

        #region NPC
        public static class NPC
		{
			public static Tile Empty = new Tile(0, Tile.MapLayer.NPC, "Empty", new Point(0, 0));

            public static Tile GreenPig = NPCs.GreenPig.AsTile(10);
		}
        #endregion

        #region Control
        public static class Control
		{
			public static Tile Empty = new Tile(0, Tile.MapLayer.Control, "Empty", new Point(0, 0));

            public static Tile TextBoxTrigger = new TextBoxTile(10, "Textbox", new Point(0, 0));

            public static Tile EnemyTrigger = new Tile(20, Tile.MapLayer.Control, "Enemy Trigger", new Point(2, 0));

            public static Tile SpawnPoint = new Tile(30, Tile.MapLayer.Control, "Spawn Point", new Point(3, 0));

            public static Tile Barrier = new Tile(40, Tile.MapLayer.Control, "Barrier", new Point(4, 0), true);
            public static Tile InvertedBarrier = new Tile(41, Tile.MapLayer.Control, "Inverted Barrier", new Point(5, 0));

            public static Tile NPCPathStart = new NPCPathTile(50, Tile.MapLayer.Control, "NPC Path Start", new Point(6, 0), false);
            public static Tile NPCPath = new Tile(51, Tile.MapLayer.Control, "NPC Path", new Point(7, 0), false);

            public static Tile MapPortal = new PortalTile(60, Tile.MapLayer.Control, "Map Portal", new Point(8, 0), false);
        }
        #endregion
    }
}