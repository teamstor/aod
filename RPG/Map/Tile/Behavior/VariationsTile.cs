using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;

namespace TeamStor.RPG
{
    /// <summary>
    /// Tile that has several textures chosen at random.
    /// {var} in the texture name will be replaced by the current variation.
    /// </summary>
    public class VariationsTile : Tile
    {
        public delegate string VariationOverrideDelegate(string defaultValue, Map map, Point mapPos, int randomValue);

        private VariationOverrideDelegate _vFunc;
        public VariationsTile AttributeChooseVariation(VariationOverrideDelegate d) { _vFunc = d; return this; }

        private static int[] _randomValues = new int[100 * 100];
        private static bool _hasValues = false;

        private string _currentVariation;

        /// <summary>
        /// Variations of this tile.
        /// </summary>
        public string[] Variations
        {
            get; private set;
        }

        public VariationsTile(string id, MapLayer layer, string name, string textureNameTemplate, int textureCount, bool solid = false, int transitionPriority = 1000) :
            base(id, layer, name, textureNameTemplate.Replace("{var}", 0.ToString()), solid, transitionPriority)
        {
            Variations = new string[textureCount];
            for(int i = 0; i < textureCount; i++)
                Variations[i] = textureNameTemplate.Replace("{var}", i.ToString());

            _currentVariation = Variations[0];
        }

        public VariationsTile(string id, MapLayer layer, string name, string[] variations, bool solid = false, int transitionPriority = 1000) :
            base(id, layer, name, variations[0], solid, transitionPriority)
        {
            Variations = variations;
            _currentVariation = Variations[0];
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

            int slot = _randomValues[((Math.Abs(mapPos.Y) * map.Width) + Math.Abs(mapPos.X)) % _randomValues.Length];
            _currentVariation = Variations[slot % Variations.Length];

            if(_vFunc != null)
                _currentVariation = _vFunc(_currentVariation, map, mapPos, slot);

            base.Draw(game, mapPos, map, metadata, environment, color);
        }

        public override void DrawAfterTransition(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
        {
            if(!_hasValues)
            {
                Random random = new Random();
                for(int i = 0; i < _randomValues.Length; i++)
                    _randomValues[i] = random.Next();

                _hasValues = true;
            }

            int slot = _randomValues[((Math.Abs(mapPos.Y) * map.Width) + Math.Abs(mapPos.X)) % _randomValues.Length];
            _currentVariation = Variations[slot % Variations.Length];

            if(_vFunc != null)
                _currentVariation = _vFunc(_currentVariation, map, mapPos, slot);

            base.DrawAfterTransition(game, mapPos, map, metadata, environment, color);
        }

        public override string TextureName(TileMetadata metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            return _currentVariation;
        }
    }
}
