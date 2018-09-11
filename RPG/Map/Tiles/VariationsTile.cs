using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TeamStor.RPG
{
    /// <summary>
    /// Tile that has several textures chosen at random.
    /// </summary>
    public class VariationsTile : Tile
    {
        private static int[] _randomValues = new int[100 * 100];
        private bool _hasValues = false;

        private Point _slotOverride = new Point(-1, -1);

        /// <summary>
        /// All texture slots this tile uses.
        /// </summary>
        public Point[] TextureSlots
        {
            get; private set;
        }

        public VariationsTile(byte id, MapLayer layer, string name, Point[] textureSlots, bool solid = false) :
            base(id, layer, name, textureSlots[0], solid)
        {
            TextureSlots = textureSlots;
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, string metadata, Map.Environment environment)
        {
            if(!_hasValues)
            {
                Random random = new Random();
                for(int i = 0; i < _randomValues.Length; i++)
                    _randomValues[i] = random.Next();

                _hasValues = true;
            }

            int slot = _randomValues[((mapPos.Y * map.Width) + mapPos.X) % _randomValues.Length] % TextureSlots.Length;

            _slotOverride = TextureSlots[slot];
            base.Draw(game, mapPos, map, metadata, environment);
            _slotOverride = new Point(-1, -1);
        }

        public override Point TextureSlot(string metadata = "", Map.Environment environment = Map.Environment.Forest)
        {
            if(_slotOverride != new Point(-1, -1))
                return _slotOverride;
            return base.TextureSlot(metadata, environment);
        }
    }
}
