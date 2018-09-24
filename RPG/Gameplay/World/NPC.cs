using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public NPC(WorldState world) : base(world)
        {
        }
    }
}
