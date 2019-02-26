using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine.Graphics;

using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Gameplay.World
{
    /// <summary>
    /// Non playable character in the world that can interact with the player.
    /// </summary>
    public class NPC : LivingEntity
    {
        public const string UP_TEXTURE = "npc/{name}/back{i}.png";
        public const string DOWN_TEXTURE = "npc/{name}/front{i}.png";
        public const string LEFT_TEXTURE = "npc/{name}/left{i}.png";
        public const string RIGHT_TEXTURE = "npc/{name}/right{i}.png";

        /// <summary>
        /// The origin tile of this NPC on the map. 
        /// Since only one NPC can exist on a given position on the NPC layer,
        /// this acts as a unique identifier of what NPC this is.
        /// </summary>
        public Point OriginTile
        {
            get; private set;
        }

        /// <summary>
        /// Template this NPC was created from.
        /// </summary>
        public NPCTemplate Template
        {
            get; private set;
        }

        public NPC(WorldState world, Point originTile, NPCTemplate template) : base(world, template.Name)
        {
            OriginTile = originTile;
            Template = template;

            Template.InitializeSelf(this);
        }

        public void Update(double deltaTime, double totalTime, long count)
        {
            Template.Behavior.OnTick(this, deltaTime, totalTime, count);
        }

        public void Draw(SpriteBatch batch)
        {
            int frame = 0;
            if(IsWalking)
                frame = ((int)World.Game.TotalFixedUpdates / 10);

            Texture2D texture = Template.TextureForDirection(World.Game, Heading, frame);

            batch.Texture(WorldPosition, texture, Color.White);
        }
    }
}
