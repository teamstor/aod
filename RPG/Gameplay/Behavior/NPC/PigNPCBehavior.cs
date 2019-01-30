using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.RPG.Gameplay.World;
using TeamStor.RPG.Gameplay.World.UI;

namespace TeamStor.RPG.Gameplay.Behavior
{
    /// <summary>
    /// Behavior for pigs.
    /// </summary>
    public class PigNPCBehavior : SimpleWalkBehavior
    {
        public PigNPCBehavior() : base(2, 1, 0.3f)
        {
        }

        public override void OnInteract(NPC npc, Player player, bool isFacingPlayer)
        {
            if(isFacingPlayer)
            {
                TextBox.Show(player.World, new TextBoxContent {
                    Speaker = npc.Template.Name,
                    Text = "*nöff nöff*"
                }, () => {
                    player.World.StartCombat(npc);
                });
            }
        }
    }
}
