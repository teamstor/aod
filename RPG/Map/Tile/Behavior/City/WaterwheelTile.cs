using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine;

namespace TeamStor.RPG
{
    public class WaterwheelTile : Tile
    {
        public const int ANIMATION_FRAMES = 22;

        public WaterwheelTile(string id, MapLayer layer, string name, string textureName, bool solid = false, int transitionPriority = 1000, bool createGlobally = true) : base(id, layer, name, textureName, solid, transitionPriority, createGlobally)
        {
        }

        private bool ScanArea(Rectangle rect, Map map)
        {
            for(int x = rect.X; x < rect.Right; x++)
            {
                for(int y = rect.Y; y < rect.Bottom; y++)
                {
                    if(rect.X < 0 || rect.Y < 0 || rect.X >= map.Width || rect.Y >= map.Height)
                        return false;
                    if(map[Tile.MapLayer.Decoration, x, y] != this)
                        return false;
                }
            }

            return true;
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
        {
            TileAtlas.Region region = Map.Atlas.MissingRegion;

            for(int i = 1; i <= ANIMATION_FRAMES; i++)
                // preload all frames
                region = Map.Atlas["tiles/city/waterwheel/" + i + ".png"];

            region = Map.Atlas.MissingRegion;

            for(int x = 0; x < 3; x++)
            {
                for(int y = 0; y < 4; y++)
                {
                    if(ScanArea(new Rectangle(mapPos.X - x, mapPos.Y - y, 3, 4), map))
                    {
                        region = Map.Atlas["tiles/city/waterwheel/" + (((int)(game.Time * 10) % ANIMATION_FRAMES) + 1) + ".png"];
                        region.Rectangle = new Rectangle(region.Rectangle.X + x * 16, region.Rectangle.Y + y * 16, 16, 16);
                    }
                }
            }

            game.Batch.Texture(
                new Vector2(mapPos.X * 16, mapPos.Y * 16),
                region.Texture,
                color.HasValue ? color.Value : Color.White,
                Vector2.One,
                region.Rectangle);
        }
    }
}
