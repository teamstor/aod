using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG
{
    public static class GroundTiles
    {
        public static Tile Grass = new VariationsTile("ground/grass", Tile.MapLayer.Terrain, "Grass", new string[]
        {
            "tiles/ground/grass/0.png", "tiles/ground/grass/0.png", "tiles/ground/grass/0.png",
            "tiles/ground/grass/1.png", "tiles/ground/grass/1.png", "tiles/ground/grass/1.png",
            "tiles/ground/grass/2.png"
        }, false, 1010).
            AttributeChooseVariation((d, m, p, s) => { return m.Info.Environment == Map.Environment.SnowMountain ? "tiles/ground/snow/0.png" : d; }).
            AttributeName((d, m, e) => { return e == Map.Environment.SnowMountain ? "Snow" : d; }).
            AttributeTransitionTexture((d, m, e) => { return "tiles/transitions/nature.png"; });

        public static Tile Dirt = new VariationsTile("ground/dirt", Tile.MapLayer.Terrain, "Dirt", new string[]
        {
            "tiles/ground/dirt/0.png", "tiles/ground/dirt/0.png", "tiles/ground/dirt/0.png",
            "tiles/ground/dirt/0.png", "tiles/ground/dirt/0.png", "tiles/ground/dirt/0.png",
            "tiles/ground/dirt/1.png"
        }, false, Grass.TransitionPriority() - 1);

        public static Tile Stone = new Tile("ground/stone", Tile.MapLayer.Terrain, "Stone", "tiles/ground/stone.png", false, Grass.TransitionPriority() - 1);
        public static Tile StoneDarker = new Tile("ground/stone-darker", Tile.MapLayer.Terrain, "Stone (Darker)", "tiles/ground/stone_darker.png", false, Grass.TransitionPriority() - 1);
        public static Tile Gravel = new Tile("ground/gravel", Tile.MapLayer.Terrain, "Gravel", "tiles/ground/gravel.png", false, Grass.TransitionPriority() - 2);
    }
}
