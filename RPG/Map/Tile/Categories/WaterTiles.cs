using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG
{
    public static class WaterTiles
    {
        public class WaterTile : AnimatedTile
        {
            public WaterTile(string id, MapLayer layer, string name, string textureNameTemplate, int textureCount, int fps, bool solid = false, int transitionPriority = 1000) : 
                base(id, layer, name, textureNameTemplate, textureCount, fps, solid, transitionPriority)
            {
            }

            public override string Name(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
            {
                if(ID == "" && environment == Map.Environment.Inside)
                    return "Void";
                return base.Name(metadata, environment);
            }

            public override void Draw(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
            {
                if(environment == Map.Environment.Inside && ID == "")
                    game.Batch.Rectangle(new Rectangle(mapPos.X * 16, mapPos.Y * 16, 16, 16), Color.Black);
                else
                    base.Draw(game, mapPos, map, metadata, environment, color);
            }

            public override string TransitionTexture(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
            {
                return ID == ShallowWater.ID ?
                    "tiles/transitions/water_light_color.png" :
                    "tiles/transitions/water_color.png";
            }
        }

        public static WaterTile DeepWaterOrVoid = new WaterTile("", Tile.MapLayer.Terrain, "Deep Water", "tiles/water/deep/{frame}.png", 4, 2, true, 1005);
        public static WaterTile ShallowWater = new WaterTile("shallow-water", Tile.MapLayer.Terrain, "Shallow Water", "tiles/water/shallow/{frame}.png", 4, 2, true, 995);
    }
}
