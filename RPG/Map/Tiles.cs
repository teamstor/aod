using System.Drawing;

namespace TeamStor.RPG
{
	/// <summary>
	/// Contains a list of tiles.
	/// </summary>
	public static class Tiles
	{
		public static class Terrain
		{
			public static Tile Water = new Tile(0, Tile.MapLayer.Terrain, "Water", new Point(0, 0));
		}
		
		public static class Decoration
		{
			public static Tile Empty = new Tile(0, Tile.MapLayer.Decoration, "Empty", new Point(0, 0));
		}
		
		public static class NPC
		{
			public static Tile Empty = new Tile(0, Tile.MapLayer.NPC, "Empty", new Point(0, 0));
		}
		
		public static class Control
		{
			public static Tile Empty = new Tile(0, Tile.MapLayer.Control, "Empty", new Point(0, 0));
		}
	}
}