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
    public class RoofBeamTile : Tile
    {
        public RoofBeamTile(byte id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, textureSlot, solid, transitionPriority)
        {
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
        {
            bool faceLeft = mapPos.X - 1 > 0 &&
                map[Layer, mapPos.X - 1, mapPos.Y] >= Tiles.Decoration.RoofOrange.ID &&
                map[Layer, mapPos.X - 1, mapPos.Y] <= Tiles.Decoration.RoofRed.ID;

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

        public override void DrawAfterTransition(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
        {
            bool faceLeft = mapPos.X - 1 > 0 &&
                map[Layer, mapPos.X - 1, mapPos.Y] >= Tiles.Decoration.RoofOrange.ID &&
                map[Layer, mapPos.X - 1, mapPos.Y] <= Tiles.Decoration.RoofRed.ID;

            if(mapPos.Y + 1 < map.Height)
            {
                if(map[Layer, mapPos.X, mapPos.Y + 1] != ID)
                {
                    game.Batch.Texture(
                        new Vector2(mapPos.X * 16, mapPos.Y * 16 + 16),
                        LayerToTexture(game, Layer),
                        color.HasValue ? color.Value : Color.White,
                        Vector2.One,
                        new Rectangle((TextureSlot(metadata, environment).X + 1) * 16, TextureSlot(metadata, environment).Y * 16, 16, 16), 
                        0,
                        null,
                        faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                }
            }
        }
    }
}
