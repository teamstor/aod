using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using TeamStor.RPG.Gameplay.World;

using Game = TeamStor.Engine.Game;

namespace TeamStor.RPG.Gameplay
{
    /// <summary>
    /// Game state while the player is in combat with an enemy.
    /// </summary>
    public class CombatState : GameState
    {
        /// <summary>
        /// The player fighting.
        /// </summary>
        public Player Combatant
        {
            get; private set;
        }

        /// <summary>
        /// The enemy of the player.
        /// </summary>
        public LivingEntity Enemy
        {
            get; private set;
        }

        public CombatState(Player player, LivingEntity enemy)
        {
            Combatant = player;
            Enemy = enemy;
        }

        public override void OnEnter(GameState previousState)
        {
        }

        public override void OnLeave(GameState nextState)
        {
        }

        public override void Update(double deltaTime, double totalTime, long count)
        {
            if(InputMap.FindMapping(InputAction.Back).Pressed(Input))
            {
                typeof(Game).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Game, Combatant.World);
                OnLeave(Combatant.World);
            }
        }

        public override void FixedUpdate(long count)
        {
        }

        public override void Draw(SpriteBatch batch, Vector2 screenSize)
        {
            screenSize = Program.ScaleBatch(batch);

            Font font = Assets.Get<Font>("fonts/bitcell.ttf");
            batch.Text(font, 16, Combatant.Name + " vs " + Enemy.Name, new Vector2(40, 40), Color.White);
        }
    }
}
