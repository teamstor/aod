using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;
using TeamStor.Engine.Coroutine;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;
using TeamStor.RPG.Gameplay.World;

using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Gameplay
{
    public enum CombatTurn
    {
        Player,
        Enemy
    }

    /// <summary>
    /// Game state while the player is in combat with an enemy.
    /// </summary>
    public class CombatState : GameState
    {
        private Point _savedCombatantPosition, _savedEnemyPosition;
        private Direction _savedCombatantHeading, _savedEnemyHeading;

        private TweenedDouble _offset;
        private bool _showWarning = false;

        /// <summary>
        /// The combat menu.
        /// </summary>
        public CombatMenu Menu
        {
            get; private set;
        } = new CombatMenu();

        /// <summary>
        /// Entity that currently has the turn in combat.
        /// </summary>
        public CombatTurn Turn
        {
            get; private set;
        } = CombatTurn.Player;

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
            _offset = new TweenedDouble(Game, 0);

            Coroutine.Start(RunCombat);
        }

        public override void OnLeave(GameState nextState)
        {
            Combatant.MoveInstantly(_savedCombatantPosition);
            Enemy.MoveInstantly(_savedEnemyPosition);

            Combatant.Heading = _savedCombatantHeading;
            Enemy.Heading = _savedEnemyHeading;
        }

        private IEnumerator<ICoroutineOperation> RunCombat()
        {
            for(int i = 0; i < 12; i++)
            {
                _showWarning = !_showWarning;
                yield return Wait.Seconds(Game, 0.15);
            }

            yield return Wait.Seconds(Game, 0.7);

            _offset.TweenTo(40, TweenEaseType.EaseInOutCubic);

            while(!_offset.IsComplete)
                yield return null;

            while(true)
            {
                bool stop = false;
                Menu.NewTurn();

                while(Menu.PendingAction == CombatPendingPlayerAction.None)
                    yield return null;

                switch(Menu.PendingAction)
                {
                    case CombatPendingPlayerAction.AttemptRunAway:
                        yield return Wait.Seconds(Game, 0.5);

                        bool didRunAway = (new Random((int)Game.TotalUpdates)).NextDouble() > 0.5f;

                        Menu.ShowMessage(didRunAway ? 
                            "You safely ran away from " + Enemy.Name + "." : 
                            "You failed to escape.", true, Game.Time + (didRunAway ? 3.5 : 3.0));
                        yield return Wait.Seconds(Game, 3.5);

                        if(didRunAway)
                            stop = true;
                        break;

                    default:
                        // wtf
                        break;
                }

                stop = stop || InputMap.FindMapping(InputAction.Back).Pressed(Input);
                if(stop)
                    break;

                // enemy turn
                yield return null;
            }

            _offset.TweenTo(0, TweenEaseType.EaseInOutCubic);

            while(!_offset.IsComplete)
                yield return null;

            typeof(Game).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Game, Combatant.World);
            OnLeave(Combatant.World);
        }

        public override void Update(double deltaTime, double totalTime, long count)
        {
            if(Turn == CombatTurn.Player)
                Menu.Update(this);
        }

        public override void FixedUpdate(long count)
        {
        }

        public override void Draw(SpriteBatch batch, Vector2 screenSize)
        {
            screenSize = Program.ScaleBatch(batch);

            string bg = "combat/plains.png";

            batch.Texture(new Vector2(0, -(float)_offset.Value), Assets.Get<Texture2D>(bg), Color.White);
            batch.Texture(new Vector2(0, 270 - (float)_offset.Value * 2), Assets.Get<Texture2D>("combat/menu.png"), Color.White);

            if(Turn == CombatTurn.Player)
                Menu.Draw(batch, new Rectangle(0, (int)(270 - (float)_offset.Value * 2), 270, Assets.Get<Texture2D>("combat/menu.png").Height), this);

            Font font = Assets.Get<Font>("fonts/bitcell.ttf");

            Matrix oldTransform = batch.Transform;
            batch.Transform = Matrix.CreateScale(2) * Matrix.CreateTranslation(80, 160 - (float)_offset.Value, 0) * oldTransform;
            Combatant.Draw(batch, false);
            Combatant.Draw(batch, true);

            batch.Transform = Matrix.CreateScale(2) * Matrix.CreateTranslation(screenSize.X - 80 - 32, 160 - (float)_offset.Value, 0) * oldTransform;
            if(Enemy is NPC)
                (Enemy as NPC).Draw(batch);
        
            batch.Transform = oldTransform;

            if(_showWarning)
            {
                Vector2 measure = font.Measure(32, "OH SHIT!!! " + Enemy.Name + " ENCOUNTERED");

                batch.Text(font, 32, "OH SHIT!!! " + Enemy.Name + " ENCOUNTERED", screenSize / 2 - measure / 2, Color.Red);
            }

            Program.BlackBorders(batch);
        }
    }
}
