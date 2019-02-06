using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.AOD.Gameplay.Behavior;

namespace TeamStor.AOD.Gameplay
{
    /// <summary>
    /// Contains a list of NPCs.
    /// </summary>
    public class NPCs
    {
        /// <summary>
        /// A pig. Nöff nöff.
        /// </summary>
        public static NPCTemplate Pig = new NPCTemplate("Pig", "pig", new PigNPCBehavior());

        /// <summary>
        /// A green test pig.
        /// </summary>
        public static NPCTemplate GreenPig = new NPCTemplate("Green Pig", "green_pig", new PigNPCBehavior());
  
        /// <summary>
        /// A slime/gelatin cube.
        /// Can only be encountered in combat.
        /// </summary>
        public static NPCTemplate SlimeCube = new NPCTemplate("Slime Cube", "slime_cube", new NullNPCBehavior());

        /// <summary>
        /// A spider.
        /// Can only be encountered in combat.
        /// </summary>
        public static NPCTemplate Spider = new NPCTemplate("Spider", "spider", new NullNPCBehavior());

        /// <summary>
        /// An aggressive wolf.
        /// Can only be encountered in combat.
        /// </summary>
        public static NPCTemplate Wolf = new NPCTemplate("Wolf", "wolf", new NullNPCBehavior());
    }
}
