using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TeamStor.RPG
{
    public class GrassTile : VariationsTile
    {
        public GrassTile(byte id, MapLayer layer, string name, Point[] textureSlots, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, textureSlots, solid, transitionPriority)
        {
        }

        public override string Name(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            if(environment == Map.Environment.SnowMountain)
                return "Snow";
             return base.Name(metadata, environment);
        }

        public override Point TextureSlot(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            if(environment == Map.Environment.SnowMountain)
            {
                /*if(base.TextureSlot(metadata, environment) == new Point(4, 0) ||
                    base.TextureSlot(metadata, environment) == new Point(5, 0))
                    return new Point(4, 1);
                else
                    return new Point(5, 1);*/

                return new Point(4, 1);
            }
            return base.TextureSlot(metadata, environment);
        }

        public override string TransitionTexture(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            return "tiles/transitions/nature.png";
        }
    }
}
