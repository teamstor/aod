using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.AOD.Gameplay.World;

namespace TeamStor.AOD.Gameplay
{
    /// <summary>
    /// Controls the behavior of a spawned NPC.
    /// This includes walking, interaction events, etc.
    /// </summary>
    public interface INPCBehavior
    {
        /// <summary>
        /// Called when the NPC is first spawned.
        /// </summary>
        void OnSpawned(NPC npc);

        /// <summary>
        /// Called when the NPC is despawned (or when exiting the world).
        /// </summary>
        void OnDespawned(NPC npc, bool isExitingWorld);

        /// <summary>
        /// Called each frame the NPC exists in the world.
        /// </summary>
        void OnTick(NPC npc, double deltaTime, double totalTime, long count);

        /// <summary>
        /// Called when the NPC interacts with the player.
        /// </summary>
        void OnInteract(NPC npc, Player player, bool isFacingPlayer);

        // TODO: tile attributes
    }
}
