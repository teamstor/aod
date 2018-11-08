using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace TeamStor.RPG
{
    public class BeamTile : Tile
    {
        public BeamTile(string id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : 
            base(id, layer, name, textureSlot, solid, transitionPriority)
        {
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
        {
            bool faceLeft = mapPos.X - 1 > 0 &&
                (map[Layer, mapPos.X - 1, mapPos.Y] == Tiles.Decoration.Wood ||
                map[Tile.MapLayer.Terrain, mapPos.X - 1, mapPos.Y] == Tiles.Terrain.StairsWood ||
                map[Layer, mapPos.X - 1, mapPos.Y] == Tiles.Decoration.BrickStone);

            game.Batch.Texture(
                new Vector2(mapPos.X * 16, mapPos.Y * 16),
                LayerToTexture(game, Layer),
                color.HasValue ? color.Value : Color.White,
                Vector2.One,
                new Rectangle(TextureSlot(metadata, environment).X * 16, TextureSlot(metadata, environment).Y * 16, 16, 16),
                0,
                null,
                faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
        }
    }
}
