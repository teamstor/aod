using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Game = TeamStor.Engine.Game;

namespace TeamStor.RPG
{
	/// <summary>
	/// 16x32 tile.
	/// </summary>
	public class DoubleTile : Tile
	{
		public DoubleTile(byte id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : 
			base(id, layer, name, textureSlot, solid, transitionPriority)
		{
		}
		
		public override void Draw(Game game, Point mapPos, Map map, Dictionary<string, string> metadata, Map.Environment environment, Color? color = null)
		{
			game.Batch.Texture(
				new Vector2(mapPos.X * 16, mapPos.Y * 16 - 16),
				LayerToTexture(game, Layer),
				color.HasValue ? color.Value : Color.White,
				Vector2.One,
				new Rectangle(TextureSlot(metadata, environment).X * 16, TextureSlot(metadata, environment).Y * 16 - 16, 16, 32));		
		}
	}
}