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
    /// An animated tile that cycles through several textures.
    /// </summary>
    public class AnimatedTile : Tile
    {
        private Point _slotOverride = new Point(-1, -1);

        /// <summary>
        /// Amount of texture slots this animation uses.
        /// </summary>
        public int SlotCount
        {
            get; private set;
        }

        /// <summary>
        /// The number of frames per second.
        /// </summary>
        public int FPS
        {
            get; private set;
        }

        public AnimatedTile(byte id, MapLayer layer, string name, Point firstSlot, int slotCount, int fps, bool solid = false, int transitionPriority = 1000) : 
            base(id, layer, name, firstSlot, solid, transitionPriority)
        {
            SlotCount = slotCount;
            FPS = fps;
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, SortedDictionary<string, string> metadata, Map.Environment environment, Color? color = null)
        {
            _slotOverride = TextureSlot(metadata, environment) + new Point((int)(game.Time * FPS) % SlotCount, 0);
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
