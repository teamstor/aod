using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using TeamStor.RPG.Gameplay.World;

using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Gameplay
{
    /// <summary>
    /// Game state while the player is in combat with an enemy.
    /// </summary>
    public class CombatState : GameState
    {
        private Point _savedCombatantPosition, _savedEnemyPosition;
        private Direction _savedCombatantHeading, _savedEnemyHeading;

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

            _savedCombatantPosition = Combatant.Position;
            _savedEnemyPosition = Enemy.Position;
            _savedCombatantHeading = Combatant.Heading;
            _savedEnemyHeading = Enemy.Heading;

            Combatant.MoveInstantly(Point.Zero);
            Enemy.MoveInstantly(Point.Zero);
            Combatant.Heading = Direction.Right;
            Enemy.Heading = Direction.Left;
        }

        public override void OnEnter(GameState previousState)
        {
        }

        public override void OnLeave(GameState nextState)
        {
            Combatant.MoveInstantly(_savedCombatantPosition);
            Enemy.MoveInstantly(_savedEnemyPosition);

            Combatant.Heading = _savedCombatantHeading;
            Enemy.Heading = _savedEnemyHeading;
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

            switch(Combatant.World.Map.Info.Environment)
            {
                case Map.Environment.Forest:
                    batch.Texture(Vector2.Zero, Assets.Get<Texture2D>("combat/plains.png"), Color.White);
                    break;
            }

            Font font = Assets.Get<Font>("fonts/bitcell.ttf");

            Matrix oldTransform = batch.Transform;
            batch.Transform = Matrix.CreateScale(2) * Matrix.CreateTranslation(80, 160, 0) * oldTransform;
            Combatant.Draw(batch);

            batch.Transform = Matrix.CreateScale(2) * Matrix.CreateTranslation(screenSize.X - 80 - 32, 160, 0) * oldTransform;
            if(Enemy is NPC)
                (Enemy as NPC).Draw(batch);
        }
    }
}
