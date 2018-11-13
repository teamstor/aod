using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;

namespace TeamStor.RPG
{
    public class DoormatTile : Tile
    {
        public DoormatTile(string id, MapLayer layer, string name, string textureName, bool solid = false, int transitionPriority = 1000, bool createGlobally = true) : 
            base(id, layer, name, textureName, solid, transitionPriority, createGlobally)
        {
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
        {
            if(mapPos.Y - 1 > 0 && map[Layer, mapPos.X, mapPos.Y - 1] != this)
                map[Layer, mapPos.X, mapPos.Y - 1].Draw(game, mapPos, map, map.GetMetadata(Layer, mapPos.X, mapPos.Y - 1), environment);

            base.Draw(game, mapPos, map, metadata, environment, color);
        }
    }
}
