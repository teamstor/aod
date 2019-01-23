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

        private TweenedDouble _transition;

        private enum TransitionType
        {
            CombatIn,
            CombatOut
        }

        private TransitionType _transitionType = TransitionType.CombatIn;
        private static RenderTarget2D _transitionRenderTarget;

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
            if(_transitionRenderTarget == null)
                _transitionRenderTarget = new RenderTarget2D(Game.GraphicsDevice, 480, 270, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            _offset = new TweenedDouble(Game, 0);
            _transition = new TweenedDouble(Game, 1);

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
            DrawToScreenOrRenderTarget(Game.Batch, new Vector2(480, 270), _transitionRenderTarget);

            _transition.TweenTo(0, TweenEaseType.EaseOutCubic, 0.5);
            yield return Wait.Seconds(Game, 0.8);

            for(int i = 0; i < 12; i++)
            {
                _showWarning = !_showWarning;
                yield return Wait.Seconds(Game, 0.15);
            }

            yield return Wait.Seconds(Game, 0.7);

            _offset.TweenTo(40, TweenEaseType.EaseInOutCubic, 0.6f);

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

                if(stop)
                    break;

                // enemy turn
                yield return null;
            }

            _offset.TweenTo(0, TweenEaseType.EaseInOutCubic, 0.6f);

            while(!_offset.IsComplete)
                yield return null;

            yield return Wait.Seconds(Game, 0.3);

            DrawToScreenOrRenderTarget(Game.Batch, new Vector2(480, 270), _transitionRenderTarget);
            _transitionType = TransitionType.CombatOut;
            _transition.TweenTo(1, TweenEaseType.EaseInCubic, 0.5);

            yield return Wait.Seconds(Game, 0.6);

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

        private void DrawToScreenOrRenderTarget(SpriteBatch batch, Vector2 screenSize, RenderTarget2D target)
        {
            if(target != null)
                batch.RenderTarget = target;
            else
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

            if(target != null)
                batch.RenderTarget = null;
            else
                Program.BlackBorders(batch);
        }

        public override void Draw(SpriteBatch batch, Vector2 screenSize)
        {
            if(_transition != 0)
            {
                screenSize = Program.ScaleBatch(batch);

                batch.Rectangle(new Rectangle(0, 0, 480, 270), Color.Black);

                Rectangle rectangle = new Rectangle(0, 0, 480, 270);
                float extraScale = _transition * 0.2f;
                if(_transitionType == TransitionType.CombatIn)
                    extraScale *= -1;

                rectangle.Width += (int)(480 * extraScale);
                rectangle.Height += (int)(270 * extraScale);

                rectangle.X = 480 / 2 - rectangle.Width / 2;
                rectangle.Y = 270 / 2 - rectangle.Height / 2;

                batch.Texture(rectangle, _transitionRenderTarget, Color.White * (1.0f - _transition));

                Program.BlackBorders(batch);
            }
            else
                DrawToScreenOrRenderTarget(batch, screenSize, null);
        }
    }
}
