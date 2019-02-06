using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;

namespace TeamStor.AOD
{
    /// <summary>
    /// FoliageTiles.Bush class
    /// </summary>
    public class BushTile : Tile
    {
        private static int[] _randomValues = new int[100 * 100];
        private static bool _hasValues = false;

        private int _variation = 0;
        private bool _connectedLeft, _connectedRight, _connectedUp, _connectedDown;

        public BushTile(string id, MapLayer layer, string name, string textureNameTemplate, bool solid = false, int transitionPriority = 1000, bool createGlobally = true) : 
            base(id, layer, name, textureNameTemplate, solid, transitionPriority, createGlobally)
        {
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
        {
            if(!_hasValues)
            {
                Random random = new Random();
                for(int i = 0; i < _randomValues.Length; i++)
                    _randomValues[i] = random.Next();

                _hasValues = true;
            }

            int slot = _randomValues[((Math.Abs(mapPos.Y) * map.Width) + Math.Abs(mapPos.X)) % _randomValues.Length] % 4;
            _variation = slot == 3 ? 1 : 0;

            _connectedLeft = mapPos.X - 1 > 0 && map[MapLayer.Decoration, mapPos.X - 1, mapPos.Y] == this;
            _connectedRight = mapPos.X + 1 < map.Width && map[MapLayer.Decoration, mapPos.X + 1, mapPos.Y] == this;

            _connectedUp = mapPos.Y - 1 > 0 && map[MapLayer.Decoration, mapPos.X, mapPos.Y - 1] == this;
            _connectedDown = mapPos.Y + 1 < map.Height && map[MapLayer.Decoration, mapPos.X, mapPos.Y + 1] == this;

            base.Draw(game, mapPos, map, metadata, environment, color);
        }

        public override string TextureName(TileMetadata metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            string connection = "normal";
            string var = "var" + (_variation + 1);

            if(_connectedLeft || _connectedRight)
            {
                if(_connectedLeft && _connectedRight)
                    connection = "lr";
                else if(_connectedLeft)
                    connection = "left";
                else if(_connectedRight)
                    connection = "right";
            }
            else if(_connectedUp || _connectedDown)
            {
                if(_connectedUp && _connectedDown)
                    connection = "ud";
                else if(_connectedUp)
                    connection = "up";
                else if(_connectedDown)
                    connection = "down";
            }

            return base.TextureName(metadata, environment).
                Replace("{connection}", connection).
                Replace("{var}", var);
        }
    }
}
