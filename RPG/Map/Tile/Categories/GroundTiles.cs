using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG
{
    public static class GroundTiles
    {
        public static Tile Grass = new VariationsTile("grass", Tile.MapLayer.Terrain, "Grass", new string[]
        {
            "tiles/ground/grass/0.png", "tiles/ground/grass/0.png", "tiles/ground/grass/0.png",
            "tiles/ground/grass/1.png", "tiles/ground/grass/1.png", "tiles/ground/grass/1.png",
            "tiles/ground/grass/2.png"
        }, false, 1010).
            AttributeName((d, m, e) => { return e == Map.Environment.SnowMountain ? "Snow" : d; }).
            AttributeTextureName((d, m, e) => { return e == Map.Environment.SnowMountain ? "tiles/ground/snow/0.png" : d; });

        public static Tile Dirt = new VariationsTile("dirt", Tile.MapLayer.Terrain, "Dirt", new string[]
        {
            "tiles/ground/dirt/0.png", "tiles/ground/dirt/0.png", "tiles/ground/dirt/0.png",
            "tiles/ground/dirt/0.png", "tiles/ground/dirt/0.png", "tiles/ground/dirt/0.png",
            "tiles/ground/dirt/1.png"
        }, false, Grass.TransitionPriority() - 1);

        public static Tile Stone = new Tile("stone", Tile.MapLayer.Terrain, "Stone", "tiles/ground/stone.png", false, Grass.TransitionPriority() - 1);
    }
}
