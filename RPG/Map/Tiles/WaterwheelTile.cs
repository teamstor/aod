using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;

namespace TeamStor.RPG
{
    public class WaterwheelTile : Tile
    {
        private Point _slotOverride = new Point(-1, -1);

        public WaterwheelTile(byte id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, textureSlot, solid, transitionPriority)
        {
        }

        private bool ScanArea(Rectangle rect, Map map)
        {
            for(int x = rect.X; x < rect.Right; x++)
            {
                for(int y = rect.Y; y < rect.Bottom; y++)
                {
                    if(rect.X < 0 || rect.Y < 0 || rect.X >= map.Width || rect.Y >= map.Height)
                        return false;
                    if(map[Tile.MapLayer.Decoration, x, y] != ID)
                        return false;
                }
            }

            return true;
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
        {
            for(int x = 0; x < 3; x++)
            {
                for(int y = 0; y < 4; y++)
                {
                    if(ScanArea(new Rectangle(mapPos.X - x, mapPos.Y - y, 3, 4), map))
                        _slotOverride = new Point(x, 5 + y);
                }
            }

            base.Draw(game, mapPos, map, metadata, environment, color);

            _slotOverride = new Point(-1, -1);
        }

        public override Point TextureSlot(SortedDictionary<string, string> metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            if(_slotOverride != new Point(-1, -1))
                return _slotOverride;
            return base.TextureSlot(metadata, environment);
        }
    }
}
