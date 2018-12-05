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
    /// Behavior for the (green) pig.
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
                TextBox.Show(player.World, new TextBoxContent
                {
                    Speaker = npc.Template.Name,
                    Text = "Hej. Jag är en gris."
                }, () =>
                {
                    TextBox.Show(player.World, new TextBoxContent
                    {
                        Speaker = "Player",
                        Text = "Grisar ska inte kunna prata?"
                    }, () => {
                        TextBox.Show(player.World, new TextBoxContent
                        {
                            Speaker = npc.Template.Name,
                            Text = "Dags att dö"
                        }, () => {
                            player.World.StartCombat(npc);
                        });
                    });
                });
            }
        }
    }
}
