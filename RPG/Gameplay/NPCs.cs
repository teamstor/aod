using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.RPG.Gameplay.Behavior;

namespace TeamStor.RPG.Gameplay
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
    }
}
