using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD
{
    public static class WaterTiles
    {
        public static Tile DeepWaterOrVoid = new AnimatedTile("", Tile.MapLayer.Terrain, "Deep Water", "tiles/water/deep/{frame}.png", 4, 2, false, true, 1005).
            AttributeName((d, m, e) => { return e == Map.Environment.Inside ? "Void" : d; }).
            AttributeTextureName((d, m, e) => { return e == Map.Environment.Inside ? "tiles/black.png" : d; }).
            AttributeTransitionTexture((d, m, e) => { return "tiles/transitions/water_color.png"; });

        public static Tile ShallowWater = new AnimatedTile("shallow-water", Tile.MapLayer.Terrain, "Shallow Water", "tiles/water/shallow/{frame}.png", 4, 2, true, true, 995).
            AttributeName((d, m, e) => { return e == Map.Environment.Swamp ? "Swamp Water" : d; }).
            AttributeTransitionTexture((d, m, e) => { return "tiles/transitions/water_color.png"; }).
            AttributeTextureName((d, m, e) => { return e == Map.Environment.Swamp ? d.Replace("shallow", "swamp") : d; });
    }
}
