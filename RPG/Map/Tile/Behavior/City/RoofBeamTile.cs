using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;

namespace TeamStor.RPG
{
    public class RoofBeamTile : Tile
    {
        public RoofBeamTile(string id, MapLayer layer, string name, string textureName, bool solid = false, int transitionPriority = 1000, bool createGlobally = true) : base(id, layer, name, textureName, solid, transitionPriority, createGlobally)
        {
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
        {
            bool faceLeft = mapPos.X - 1 > 0 && map[Layer, mapPos.X - 1, mapPos.Y].ID.StartsWith("city/roof");

            TileAtlas.Region region = Map.Atlas[TextureName(metadata, environment)];

            game.Batch.Texture(
                new Vector2(mapPos.X * 16, mapPos.Y * 16),
                region.Texture,
                color.HasValue ? color.Value : Color.White,
                Vector2.One,
                region.Rectangle, 0, null,
                faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
        }

        public override void DrawAfterTransition(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
        {
            bool faceLeft = mapPos.X - 1 > 0 && map[Layer, mapPos.X - 1, mapPos.Y].ID.StartsWith("city/roof");

            if(mapPos.Y + 1 < map.Height && map[Layer, mapPos.X, mapPos.Y + 1] != this)
            {
                TileAtlas.Region region = Map.Atlas["tiles/city/beam/top.png"];

                game.Batch.Texture(
                    new Vector2(mapPos.X * 16, mapPos.Y * 16 + 16),
                    region.Texture,
                    color.HasValue ? color.Value : Color.White,
                    Vector2.One,
                    region.Rectangle, 0, null,
                    faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            }
        }
    }
}
