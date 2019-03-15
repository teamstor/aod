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
    /// An animated tile that cycles through several textures.
    /// {frame} in the texture name will be replaced by the current frame.
    /// </summary>
    public class AnimatedTile : Tile
    {
        private int _currentSlot = 0;

        /// <summary>
        /// Amount of texture slots this animation uses.
        /// </summary>
        public int TextureCount
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
        
        /// <summary>
        /// If every other tile should be one frame ahead.
        /// </summary>
        public bool UseVariation { get; private set; }

        public AnimatedTile(string id, MapLayer layer, string name, string textureNameTemplate, int textureCount, int fps, bool useVariation, bool solid = false, int transitionPriority = 1000) : 
            base(id, layer, name, textureNameTemplate, solid, transitionPriority)
        {
            TextureCount = textureCount;
            UseVariation = useVariation;
            FPS = fps;
        }

        public override void Draw(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
        {
            UpdateCurrentFrameWithGame(game, mapPos);
            base.Draw(game, mapPos, map, metadata, environment, color);
        }

        public override void DrawAfterTransition(Engine.Game game, Point mapPos, Map map, TileMetadata metadata, Map.Environment environment, Color? color = null)
        {
            UpdateCurrentFrameWithGame(game, mapPos);
            base.DrawAfterTransition(game, mapPos, map, metadata, environment, color);
        }

        public void UpdateCurrentFrameWithGame(Engine.Game game, Point mapPos)
        {
            if(UseVariation)
                _currentSlot = ((int)(game.Time * FPS) + ((mapPos.X + mapPos.Y) % 2)) % TextureCount;
            else
                _currentSlot = (int)(game.Time * FPS) % TextureCount;
        }

        public override string TextureName(TileMetadata metadata = null, Map.Environment environment = Map.Environment.Forest)
        {
            return base.TextureName(metadata, environment).Replace("{frame}", _currentSlot.ToString());
        }
    }
}
