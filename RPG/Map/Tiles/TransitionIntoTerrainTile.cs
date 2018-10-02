using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TeamStor.RPG
{
    /// <summary>
    /// Tile that transitions down into the terrain layer on the decoration layer.
    /// </summary>
    public class TransitionIntoTerrainTile : Tile
    {
        public TransitionIntoTerrainTile(byte id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : 
            base(id, layer, name, textureSlot, solid, transitionPriority)
        {
            if(layer != MapLayer.Decoration)
                throw new InvalidOperationException("TransitionIntoTerrainTile only works on the decoration layer");
        }

        public override bool UseTransition(Point from, Point to, Map map, Tile other, Dictionary<string, string> metadata = null, Dictionary<string, string> otherMetadata = null)
        {
            if((to - from).Y == 1)
            {
                if(map[MapLayer.Decoration, to.X, to.Y] == Tiles.Decoration.Empty.ID &&
                    map[MapLayer.Terrain, to.X, to.Y] != Tiles.Terrain.Water.ID)
                    return true;
            }
            return false;
        }
    }
}
