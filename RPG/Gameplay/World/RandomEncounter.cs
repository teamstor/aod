using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG.Gameplay.World
{
    /// <summary>
    /// Contains lists of all NPCs that can be random encountered in an environment.
    /// </summary>
    public static class RandomEncounter
    {
        private static Dictionary<Map.Environment, List<NPCTemplate>> _encounters = new Dictionary<Map.Environment, List<NPCTemplate>>();

        static RandomEncounter()
        {
            _encounters.Add(Map.Environment.Forest, new List<NPCTemplate>());

            _encounters[Map.Environment.Forest].Add(NPCs.SlimeCube);
            _encounters[Map.Environment.Forest].Add(NPCs.Wolf);
        }

        /// <summary>
        /// Attempts a random encounter. Call this after every step while in encounter areas.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public static bool TryRandomEncounter(WorldState world)
        {
            List<NPCTemplate> encounters;
            if(!_encounters.TryGetValue(world.Map.Info.Environment, out encounters))
                return false;

            bool didEncounter = (new Random((int)world.Game.TotalUpdates).NextDouble() < 0.3);
            NPCTemplate encounteredNPC = encounters[new Random((int)world.Game.TotalUpdates + 1337).Next() % encounters.Count];

            if(didEncounter)
            {
                // spawn a new NPC but don't actually add it to the world
                NPC entity = new NPC(world, new Point(0, 0), encounteredNPC);
                world.StartCombat(entity);
            }

            return didEncounter;
        }
    }
}
