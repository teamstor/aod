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
    public class BeamTile : Tile
    {
        private static HashSet<Tile> _connectTo = new HashSet<Tile>
        {
            CityTiles.Wall,
            CityTiles.Bricks,
            CityTiles.StoneBricks,

            CityTiles.Door,
            CityTiles.DoorStone,
            CityTiles.HouseStairs
        };

        public BeamTile(string id, MapLayer layer, string name, string textureName, bool solid = false, int transitionPriority = 1000, bool createGlobally = true) : base(id, layer, name, textureName, solid, transitionPriority, createGlobally)
        {
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
        {
            bool faceLeft = mapPos.X - 1 > 0 && _connectTo.Contains(map[Layer, mapPos.X - 1, mapPos.Y]);

            TileAtlas.Region region = Map.Atlas[TextureName(metadata, environment)];

            game.Batch.Texture(
                new Vector2(mapPos.X * 16, mapPos.Y * 16),
                region.Texture,
                color.HasValue ? color.Value : Color.White,
                Vector2.One,
                region.Rectangle, 0, null,
                faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
        }
    }
}
