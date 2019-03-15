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
        public static NPCTemplate Pig = new NPCTemplate("Pig", "pig", new PigNPCBehavior()).
            AttributeInitializeSelf((npc) =>
            {
                npc.Level = 4;
                npc.Vitality = 10;
                npc.Health = npc.MaxHealth;
                npc.KillXP = 10000;

                npc.Inventory[InventoryEquipSlot.Head] = npc.Inventory.Push(Item.TestItem);
                npc.Inventory[InventoryEquipSlot.Weapon] = npc.Inventory.Push(Item.Thunderfury);
            });

        /// <summary>
        /// A green test pig.
        /// </summary>
        public static NPCTemplate GreenPig = new NPCTemplate("Green Pig", "green_pig", new PigNPCBehavior()).
            AttributeInitializeSelf((npc) => { npc.KillXP = 10; });
  
        /// <summary>
        /// A slime/gelatin cube.
        /// Can only be encountered in combat.
        /// </summary>
        public static NPCTemplate SlimeCube = new NPCTemplate("Slime Cube", "slime_cube", new NullNPCBehavior()).
            AttributeInitializeSelf((npc) => { npc.KillXP = 2; });

        /// <summary>
        /// A spider.
        /// Can only be encountered in combat.
        /// </summary>
        public static NPCTemplate Spider = new NPCTemplate("Spider", "spider", new NullNPCBehavior()).
            AttributeInitializeSelf((npc) => { npc.KillXP = 5; });

        /// <summary>
        /// An aggressive wolf.
        /// Can only be encountered in combat.
        /// </summary>
        public static NPCTemplate Wolf = new NPCTemplate("Wolf", "wolf", new NullNPCBehavior()).
            AttributeInitializeSelf((npc) => { npc.KillXP = 7; });
    }
}
