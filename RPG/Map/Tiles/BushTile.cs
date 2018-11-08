using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;

namespace TeamStor.RPG
{
    public class BushTile : Tile
    {
        private static int[] _randomValues = new int[100 * 100];
        private static bool _hasValues = false;

        private Point _slotOverride = new Point(-1, -1);

        public BushTile(string id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, textureSlot, solid, transitionPriority)
        {
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
        {
            if(!_hasValues)
            {
                Random random = new Random();
                for(int i = 0; i < _randomValues.Length; i++)
                    _randomValues[i] = random.Next();

                _hasValues = true;
            }

            int slot = _randomValues[((Math.Abs(mapPos.Y) * map.Width) + Math.Abs(mapPos.X)) % _randomValues.Length] % 4;

            _slotOverride = TextureSlot(metadata, environment);
            _slotOverride.X += slot == 3 ? 1 : 0;

            if(mapPos.Y - 1 > 0 && map[MapLayer.Decoration, mapPos.X, mapPos.Y - 1] == this &&
                mapPos.Y + 1 < map.Height && map[MapLayer.Decoration, mapPos.X, mapPos.Y + 1] == this)
                _slotOverride.X += 12;
            else if(mapPos.Y - 1 > 0 && map[MapLayer.Decoration, mapPos.X, mapPos.Y - 1] == this)
                _slotOverride.X += 10;
            else if(mapPos.Y + 1 < map.Height && map[MapLayer.Decoration, mapPos.X, mapPos.Y + 1] == this)
                _slotOverride.X += 8;
            else if(mapPos.X - 1 > 0 && map[MapLayer.Decoration, mapPos.X - 1, mapPos.Y] == this &&
                mapPos.X + 1 < map.Width && map[MapLayer.Decoration, mapPos.X + 1, mapPos.Y] == this)
                _slotOverride.X += 6;
            else if(mapPos.X + 1 < map.Width && map[MapLayer.Decoration, mapPos.X + 1, mapPos.Y] == this)
                _slotOverride.X += 4;
            else if(mapPos.X - 1 > 0 && map[MapLayer.Decoration, mapPos.X - 1, mapPos.Y] == this)
                _slotOverride.X += 2;

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
