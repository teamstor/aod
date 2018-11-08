using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;

namespace TeamStor.RPG
{
    public class RoofTile : Tile
    {
        public RoofTile(string id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, textureSlot, solid, transitionPriority)
        {
        }

        public override void DrawAfterTransition(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
        {
            if(mapPos.Y + 1 < map.Height && map[Layer, mapPos.X, mapPos.Y + 1] == Tiles.Decoration.Wood)
            {
                game.Batch.Texture(
                    new Vector2(mapPos.X * 16, mapPos.Y * 16 + 16),
                    LayerToTexture(game, Layer),
                    Color.White,
                    Vector2.One,
                    new Rectangle(3 * 16, 2 * 16, 16, 16));
            }
        }
    }
}
