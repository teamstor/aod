using System.Drawing;

namespace TeamStor.RPG.Map
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
			
		}
		
		public static class NPC
		{
			
		}
		
		public static class Control
		{
			
		}
	}
}