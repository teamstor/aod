using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.AOD.Gameplay.World;

namespace TeamStor.AOD.Gameplay.Behavior
{
    public class NullNPCBehavior : INPCBehavior
    {
        public void OnSpawned(NPC npc)
        {
        }

        public void OnDespawned(NPC npc, bool isExitingWorld)
        {
        }

        public void OnTick(NPC npc, double deltaTime, double totalTime, long count)
        {
        }

        public void OnInteract(NPC npc, Player player, bool npcIsFacingPlayer)
        {
        }
    }
}
