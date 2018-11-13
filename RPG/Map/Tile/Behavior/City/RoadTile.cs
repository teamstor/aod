using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeamStor.RPG
{
    public class RoadTile : Tile
    {
        public RoadTile(string id, MapLayer layer, string name, string textureName, bool solid = false, int transitionPriority = 1000, bool createGlobally = true) : base(id, layer, name, textureName, solid, transitionPriority, createGlobally)
        {
        }

        public override void DrawAfterTransition(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
        {
            TileAtlas.Region region = Map.Atlas["tiles/city/road_corner.png"];
            Rectangle mapRect = new Rectangle(0, 0, map.Width, map.Height);

            if(mapRect.Contains(mapPos - new Point(1, 1)) && map[Layer, mapPos.X - 1, mapPos.Y - 1] != this)
            {
                game.Batch.Texture(
                    new Vector2(mapPos.X * 16 - 16, mapPos.Y * 16 - 16),
                    region.Texture,
                    color.HasValue ? color.Value : Color.White,
                    Vector2.One,
                    region.Rectangle, 0,
                    null,
                    SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically);
            }

            if(mapRect.Contains(mapPos + new Point(-1, 1)) && map[Layer, mapPos.X - 1, mapPos.Y + 1] != this)
            {
                game.Batch.Texture(
                    new Vector2(mapPos.X * 16 - 16, mapPos.Y * 16 + 16),
                    region.Texture,
                    color.HasValue ? color.Value : Color.White,
                    Vector2.One,
                    region.Rectangle, 0,
                    null,
                    SpriteEffects.FlipHorizontally);
            }

            if(mapRect.Contains(mapPos + new Point(1, -1)) && map[Layer, mapPos.X + 1, mapPos.Y - 1] != this)
            {
                game.Batch.Texture(
                    new Vector2(mapPos.X * 16 + 16, mapPos.Y * 16 - 16),
                    region.Texture,
                    color.HasValue ? color.Value : Color.White,
                    Vector2.One,
                    region.Rectangle, 0,
                    null,
                    SpriteEffects.FlipVertically);
            }

            if(mapRect.Contains(mapPos + new Point(1, 1)) && map[Layer, mapPos.X + 1, mapPos.Y + 1] != this)
            {
                game.Batch.Texture(
                    new Vector2(mapPos.X * 16 + 16, mapPos.Y * 16 + 16),
                    region.Texture,
                    color.HasValue ? color.Value : Color.White,
                    Vector2.One,
                    region.Rectangle);
            }
        }

        public override string TransitionTexture(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            return "tiles/transitions/road_color.png";
        }
    }
}
