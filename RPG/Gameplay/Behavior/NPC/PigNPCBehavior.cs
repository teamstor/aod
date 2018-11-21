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
                    Text = "Hallo. Ich bin ein Schwein. Nöff nöff.\nIch liebe zu rulla runt i lera."
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
                            Text = "Nöff nöff nöff nöff nöff nöff"
                        });
                    });
                });
            }
        }
    }
}
