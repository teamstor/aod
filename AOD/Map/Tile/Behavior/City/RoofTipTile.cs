using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;

namespace TeamStor.AOD
{
    public class RoofTipTile : Tile
    {
        public RoofTipTile(string id, MapLayer layer, string name, string textureName, bool solid = false, int transitionPriority = 1000, bool createGlobally = true) : 
            base(id, layer, name, textureName, solid, transitionPriority, createGlobally)
        {
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
        {
            TileAtlas.Region region = Map.Atlas[TextureName(metadata, environment)];

            if(mapPos.X - 1 > 0 && map[Layer, mapPos.X - 1, mapPos.Y].ID == ID)
                region = Map.Atlas[TextureName(metadata, environment).Replace("tip.png", "tip_right.png")];

            if(mapPos.X + 1 <= map.Width && map[Layer, mapPos.X + 1, mapPos.Y].ID == ID)
                region = Map.Atlas[TextureName(metadata, environment).Replace("tip.png", "tip_left.png")];

            if(mapPos.X - 1 > 0 && map[Layer, mapPos.X - 1, mapPos.Y].ID == ID &&
                mapPos.X + 1 <= map.Width && map[Layer, mapPos.X + 1, mapPos.Y].ID == ID)
                region = Map.Atlas[TextureName(metadata, environment).Replace("_tip.png", ".png")];

            game.Batch.Texture(
                new Vector2(mapPos.X * 16, mapPos.Y * 16),
                region.Texture,
                color.HasValue ? color.Value : Color.White,
                Vector2.One,
                region.Rectangle);
        }
    }
}
