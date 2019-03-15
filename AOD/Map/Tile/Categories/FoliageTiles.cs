using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD
{
    public static class FoliageTiles
    {
        public static Tile Tree = new DrawOffsetTile("foliage/tree", Tile.MapLayer.Decoration, "Tree", "tiles/foliage/tree.png", new Point(0, -1), true).
            AttributeTextureName((d, m, e) => { return e == Map.Environment.SnowMountain ? "tiles/foliage/tree_snow.png" : d; });
        public static Tile Birch = new DrawOffsetTile("foliage/birch", Tile.MapLayer.Decoration, "Birch", "tiles/foliage/birch.png", new Point(0, -1), true);

        public static Tile Bush = new BushTile("foliage/bush", Tile.MapLayer.Decoration, "Bush", "tiles/foliage/bush/{connection}_{var}.png", true);
        public static TallGrassTile TallGrass = new TallGrassTile("foliage/tall-grass", Tile.MapLayer.Decoration, "Tall Grass", "tiles/foliage/enemy_encounter_block.png", false);
        public static Tile Rock = new RockTile("foliage/rock", Tile.MapLayer.Decoration, "Rock", "tiles/foliage/rock.png", true);
    }
}
