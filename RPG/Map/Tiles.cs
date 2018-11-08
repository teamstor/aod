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
                public WaterTile(string id, MapLayer layer, string name, Point firstSlot, int slotCount, int fps, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, firstSlot, slotCount, fps, solid, transitionPriority)
                {
                }

                public override string Name(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
                {
                    if(environment == Map.Environment.Inside && ID == "")
                        return "Void";
                    return base.Name(metadata, environment);
                }

                public override void Draw(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
                {
                    if(environment == Map.Environment.Inside && ID == "")
                        game.Batch.Rectangle(new Rectangle(mapPos.X * 16, mapPos.Y * 16, 16, 16), Color.Black);
                    else
                        base.Draw(game, mapPos, map, metadata, environment, color);
                }

                public override string TransitionTexture(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
                {
                    return ID == ShallowWater.ID ?
                        "tiles/transitions/water_light_color.png" :
                        "tiles/transitions/water_color.png";
                }
            }

            public static Tile Water = new WaterTile("", Tile.MapLayer.Terrain, "Water", new Point(0, 0), 4, 2, true);
            public static Tile ShallowWater = new WaterTile("shallow-water", Tile.MapLayer.Terrain, "Shallow Water", new Point(0, 3), 4, 3, true, 999);

            public static Tile Grass = new GrassTile("grass", Tile.MapLayer.Terrain, "Grass", 
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

            public static Tile Dirt = new VariationsTile("dirt", Tile.MapLayer.Terrain, "Dirt",
                new Point[] {
                    // 4/5 chance
                    new Point(7, 0),
                    new Point(7, 0),
                    new Point(7, 0),
                    new Point(7, 0),

                    // 1/5 chance
                    new Point(8, 0)
			});

			public static Tile Stone = new Tile("stone", Tile.MapLayer.Terrain, "Stone", new Point(9, 0), false);
			
			public static Tile Wood = new InsideTile("wood", Tile.MapLayer.Terrain, "Wood", new Point(1, 1), false, -1);

            public static Tile RoadCity = new RoadCityTile("road-city", Tile.MapLayer.Terrain, "City Road", new Point(0, 1), false, 1002);
            public static Tile StairsWood = new Tile("stairs-wood", Tile.MapLayer.Terrain, "Wooden Stairs", new Point(0, 2), false, -1);
            public static Tile Doormat = new InsideTile("doormat", Tile.MapLayer.Terrain, "Doormat", new Point(1, 2), false, -1);
        }
        #endregion

        #region Decoration
        public static class Decoration
		{
			public static Tile Empty = new Tile("", Tile.MapLayer.Decoration, "Empty", new Point(0, 0));
			
            public class TreeTile : DoubleTile
            {
                public TreeTile(string id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, textureSlot, solid, transitionPriority)
                {
                }

                public override Point TextureSlot(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
                {
                    if(environment == Map.Environment.SnowMountain)
                        return base.TextureSlot(metadata, environment) + new Point(0, 2);
                    return base.TextureSlot(metadata, environment);
                }
            }

			public static Tile Tree = new TreeTile("tree", Tile.MapLayer.Decoration, "Tree", new Point(0, 1), true);
            public static Tile Bush = new BushTile("bush", Tile.MapLayer.Decoration, "Bush", new Point(0, 4), true);

            public static Tile Wood = new Tile("wood", Tile.MapLayer.Decoration, "Wood", new Point(2, 0), true);
            public static Tile Brick = new Tile("brick", Tile.MapLayer.Decoration, "Bricks", new Point(3, 0), true);
			public static Tile BrickStone = new Tile("brick-stone", Tile.MapLayer.Decoration, "Stone Bricks", new Point(4, 0), true);

            public static Tile RoofOrange = new RoofTile("roof-orange", Tile.MapLayer.Decoration, "Orange Roof", new Point(3, 1), true);
            public static Tile RoofBlue = new RoofTile("roof-blue", Tile.MapLayer.Decoration, "Blue Roof", new Point(4, 1), true);
            public static Tile RoofPurple = new RoofTile("roof-purple", Tile.MapLayer.Decoration, "Purple Roof", new Point(5, 1), true);
            public static Tile RoofRed = new RoofTile("roof-red", Tile.MapLayer.Decoration, "Red Roof", new Point(6, 1), true);
            public static Tile RoofBeam = new RoofBeamTile("roof-beam", Tile.MapLayer.Decoration, "Roof Beam", new Point(5, 0));
            public static Tile Beam = new BeamTile("beam", Tile.MapLayer.Decoration, "Beam", new Point(7, 0));
            public static Tile WindowOrange = new Tile("window-orange", Tile.MapLayer.Decoration, "Orange Window", new Point(4, 2), true);
            public static Tile WindowBlue = new Tile("window-blue", Tile.MapLayer.Decoration, "Blue Window", new Point(5, 2), true);
            public static Tile WindowPurple = new Tile("window-purple", Tile.MapLayer.Decoration, "Purple Window", new Point(6, 2), true);
            public static Tile WindowRed = new Tile("window-red", Tile.MapLayer.Decoration, "Red Window", new Point(7, 2), true);
            public static Tile WindowStone = new Tile("window-stone", Tile.MapLayer.Decoration, "Stone Window", new Point(2, 1), true);

            public static Tile DoorWood = new DoubleTile("door-wood", Tile.MapLayer.Decoration, "Wooden Door/Wood BG", new Point(1, 1), true);
            public static Tile DoorStone = new DoubleTile("door-stone", Tile.MapLayer.Decoration, "Wooden Door/Stone BG", new Point(1, 3), true);
            public static Tile Sign = new Tile("stone", Tile.MapLayer.Decoration, "Sign", new Point(8, 1), true);

            public static Tile WoodenPanel = new InsideTile("wooden-panel", Tile.MapLayer.Decoration, "Wooden Panel", new Point(9, 0), true, -1);
            public static Tile HouseWall = new InsideTile("house-wall", Tile.MapLayer.Decoration, "Wall", new Point(8, 0), true, -1);

            public class DoorInsideTile : DoubleTile
            {
                public DoorInsideTile(string id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, textureSlot, solid, transitionPriority)
                {
                }

                public override bool Filter(Map.Environment environment)
                {
                    return environment == Map.Environment.Inside;
                }
            }

            public static Tile DoorWoodInside = new DoorInsideTile("door-wood-inside", Tile.MapLayer.Decoration, "Wooden Door", new Point(10, 1), true);

            public static Tile Waterwheel = new WaterwheelTile("waterwheel", Tile.MapLayer.Decoration, "Waterwheel", new Point(3, 5), true);
        }
        #endregion

        #region NPC
        public static class NPC
		{
			public static Tile Empty = new Tile("", Tile.MapLayer.NPC, "Empty", new Point(0, 0));

            public static Tile GreenPig = NPCs.GreenPig.AsTile();
		}
        #endregion

        #region Control
        public static class Control
		{
			public static Tile Empty = new Tile("", Tile.MapLayer.Control, "Empty", new Point(0, 0));

            public static Tile TextBoxTrigger = new TextBoxTile("textbox", "Textbox", new Point(0, 0));

            public static Tile EnemyTrigger = new Tile("enemy-trigger", Tile.MapLayer.Control, "Enemy Trigger", new Point(2, 0));

            public static Tile SpawnPoint = new Tile("spawnpoint", Tile.MapLayer.Control, "Spawn Point", new Point(3, 0));

            public static Tile Barrier = new Tile("barrier", Tile.MapLayer.Control, "Barrier", new Point(4, 0), true);
            public static Tile InvertedBarrier = new Tile("-barrier", Tile.MapLayer.Control, "Inverted Barrier", new Point(5, 0));

            public static Tile NPCPathStart = new NPCPathTile("npc-path-start", Tile.MapLayer.Control, "NPC Path Start", new Point(6, 0), false);
            public static Tile NPCPath = new Tile("npc-path", Tile.MapLayer.Control, "NPC Path", new Point(7, 0), false);

            public static Tile MapPortal = new PortalTile("map-portal", Tile.MapLayer.Control, "Map Portal", new Point(8, 0), false);
        }
        #endregion
    }
}