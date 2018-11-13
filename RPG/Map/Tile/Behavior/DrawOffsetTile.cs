using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine;

namespace TeamStor.RPG
{
    /// <summary>
    /// Tile that's drawn offset from it's real position by a number of tiles.
    /// </summary>
    public class DrawOffsetTile : Tile
    {
        /// <summary>
        /// Offset of this tile in tiles units.
        /// </summary>
        public Point Offset { get; private set; }

        public DrawOffsetTile(string id, MapLayer layer, string name, string textureName, Point offset, bool solid = false, int transitionPriority = 1000, bool createGlobally = true) : 
            base(id, layer, name, textureName, solid, transitionPriority, createGlobally)
        {
            Offset = offset;
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
        {
            base.Draw(game, mapPos + Offset, map, metadata, environment, color);
        }

        public override void DrawAfterTransition(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
        {
            base.DrawAfterTransition(game, mapPos + Offset, map, metadata, environment, color);
        }
    }
}
