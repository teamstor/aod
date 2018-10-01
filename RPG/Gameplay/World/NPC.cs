using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine.Graphics;

using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Gameplay.World
{
    /// <summary>
    /// Non playable character in the world that can interact with the player.
    /// </summary>
    public class NPC : PositionedEntity
    {
        public const string UP_TEXTURE = "npc/{name}/back{i}.png";
        public const string DOWN_TEXTURE = "npc/{name}/front{i}.png";
        public const string LEFT_TEXTURE = "npc/{name}/left{i}.png";
        public const string RIGHT_TEXTURE = "npc/{name}/right{i}.png";

        /// <summary>
        /// Template this NPC was created from.
        /// </summary>
        public NPCTemplate Template
        {
            get; private set;
        }

        public NPC(WorldState world, NPCTemplate template) : base(world)
        {
            Template = template;
        }

        public void Update(double deltaTime, double totalTime, long count)
        {
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
