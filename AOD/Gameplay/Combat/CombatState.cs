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
using TeamStor.AOD.Gameplay.World;
using TeamStor.AOD.Gameplay.World.UI;
using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Gameplay
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
        
        private Vector2 _shakeOffset = Vector2.Zero;

        /// <summary>
        /// The combat menu.
        /// </summary>
        public CombatMenu Menu
        {
            get; private set;
        }

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

        private InventoryUI _inventoryUI;

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
            Menu = new CombatMenu(this);
            
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

        private class CombatActionArgs
        {
            /// <summary>
            /// Stops combat and returns to the world.
            /// </summary>
            public bool Stop;

            /// <summary>
            /// Skips the enemys turn.
            /// </summary>
            public bool BackToPlayerTurn;
        }

        private IEnumerator<ICoroutineOperation> AttackAction(bool fromEnemy, CombatActionArgs args)
        {
            LivingEntity attackingEntity = fromEnemy ? Enemy : Combatant;
            LivingEntity attackedEntity = fromEnemy ? Combatant : Enemy;

            Random random = new Random();
            int armorSlots = attackedEntity.PhysicalArmor;
            if(armorSlots > 100)
                armorSlots = 100;
            
            bool didHit = random.Next() % 100 > armorSlots;
            Inventory.ItemSlotReference hitArmorPiece = attackedEntity.Inventory[Inventory.EMPTY_SLOT];

            if(!didHit)
            {
                foreach(InventoryEquipSlot slot in ((InventoryEquipSlot[])Enum.GetValues(typeof(InventoryEquipSlot))).OrderBy(x => random.Next()))
                {
                    if(!attackedEntity.Inventory[slot].IsEmptyReference)
                    {
                        hitArmorPiece = attackedEntity.Inventory[slot];
                        break;
                    }
                }
            }
                        
            int damage = attackingEntity.MeleeAttackDamageRange.Item1;
            if(attackingEntity.MeleeAttackDamageRange.Item2 != attackingEntity.MeleeAttackDamageRange.Item1)
            {
                damage = attackingEntity.MeleeAttackDamageRange.Item1 +
                         random.Next() % (attackingEntity.MeleeAttackDamageRange.Item2 - attackingEntity.MeleeAttackDamageRange.Item1);
            }

            if(!fromEnemy)
            {
                bool[] slots = new bool[200];

                int slotsFilled = armorSlots;
                while(slotsFilled > 0)
                {
                    int tries = 150;
                    int rand;
                    while(slots[(rand = random.Next()) % 100] && tries > 0) { tries--; }
                    slots[rand % 100] = true;
                    
                    slotsFilled--;
                }
                
                if(armorSlots == 100)
                    for(int i = 0; i < 100; i++)
                        slots[i] = true;
                
                // always land on 72
                slots[64] = !didHit;

                // duplicate
                for(int i = 0; i < 100; i++)
                    slots[i + 100] = slots[i];
                
                yield return Wait.Seconds(Game, 0.5);

                Menu.AttackSlots = slots;
                Menu.AttackSlotAnimation.TweenTo(0, TweenEaseType.Linear, 0);
                Menu.AttackSlotAnimation.TweenTo((8 * 164) - 480 / 2, TweenEaseType.EaseOutSine, 2);

                while(!Menu.AttackSlotAnimation.IsComplete)
                {
                    if(InputMap.FindMapping(InputAction.Action).Pressed(Input))
                    {
                        Menu.AttackSlotAnimation.TweenTo((8 * 164) - 480 / 2, TweenEaseType.Linear, 0);
                        break;
                    }
                    
                    yield return null;
                }

                yield return Wait.Seconds(Game, 1.5);
                Menu.AttackSlots = null;
            }

            if(didHit)
            {
                for(int i = 0; i < 6; i++)
                {
                    int factor = 3;
                    if(damage > attackedEntity.MaxHealth / 2)
                        factor = 5;
                    if(damage < attackedEntity.MaxHealth / 10)
                        factor = 2;
                    _shakeOffset = new Vector2((random.Next() % factor) * (random.Next() % 3 - 1), (random.Next() % factor) * (random.Next() % 3 - 1));
                    yield return Wait.Seconds(Game, 0.03);
                }
                
                _shakeOffset = Vector2.Zero;
            }

            string attackMsg = "";

            if(didHit)
            {
                if(damage > attackedEntity.MaxHealth / 2)
                    attackMsg = "{attacker} hit {attacked} for an astonishing {dmg} damage!!!";
                else if(damage < attackedEntity.MaxHealth / 10)
                    attackMsg = "{attacker} hit {attacked} for {dmg} damage like a wet noodle.";
                else
                    attackMsg = "{attacker} hit {attacked} for {dmg} damage.";
            }
            else
                attackMsg = "{attacker} got blocked by {attacked}'s {apiece}.";

            attackMsg = attackMsg.
                Replace("{attacker}", attackingEntity.Name).
                Replace("{attacked}", attackedEntity.Name).
                Replace("{dmg}", damage.ToString()).
                Replace("{apiece}", hitArmorPiece.Item?.Name);

            yield return Wait.Seconds(Game, 0.5);
            Menu.ShowMessage(attackMsg, true, Game.Time + 3.5);
            yield return Wait.Seconds(Game, 3.5);

            if(didHit)
                attackedEntity.Health -= damage;

            if(attackedEntity.Health <= 0)
            {
                yield return Wait.Seconds(Game, 0.5);
                Menu.ShowMessage(fromEnemy ? "You died." : attackedEntity.Name + " was defeated. " + attackedEntity.KillXP + " XP gained.", true, Game.Time + 3.5);
                yield return Wait.Seconds(Game, 3.5);

                args.Stop = true;
            }
            
            yield return Wait.Seconds(Game, 0.5);
        }

        private IEnumerator<ICoroutineOperation> RunAwayAction(CombatActionArgs args)
        {
            yield return Wait.Seconds(Game, 0.5);

            bool didRunAway = (new Random((int)Game.TotalUpdates)).NextDouble() > 0.5f;

            Menu.ShowMessage(didRunAway ?
                "You safely ran away from " + Enemy.Name + "." :
                "You failed to escape.", true, Game.Time + (didRunAway ? 3.5 : 3.0));
            yield return Wait.Seconds(Game, 3.5);

            if(didRunAway)
                args.Stop = true;
        }

        private IEnumerator<ICoroutineOperation> OpenInventoryAction(CombatActionArgs args)
        {
            _offset.TweenTo(0, TweenEaseType.EaseInOutCubic, 0.3f);
            while(!_offset.IsComplete)
                yield return null;

            _inventoryUI = new InventoryUI(Combatant, this);
            _inventoryUI.CloseOnAction = true;

            while(!_inventoryUI.IsShowingCompleted)
                yield return null;

            _inventoryUI = null;

            _offset.TweenTo(40, TweenEaseType.EaseInOutCubic, 0.3f);
            while(!_offset.IsComplete)
                yield return null;

            args.BackToPlayerTurn = _inventoryUI.WasActionPerformedOnClose;
            Menu.Page = CombatMenuPage.ActionSelection;
            Menu.SelectedButton = Menu.Buttons[CombatMenuPage.ActionSelection].IndexOf("Inventory");
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

            CombatActionArgs args = new CombatActionArgs();
            while(true)
            {
                args.Stop = args.BackToPlayerTurn = false;
                Menu.NewTurn();

                while(Menu.PendingAction == CombatPendingPlayerAction.None)
                    yield return null;

                IEnumerator<ICoroutineOperation> subAction;

                switch(Menu.PendingAction)
                {
                    case CombatPendingPlayerAction.AttackMelee:
                        subAction = AttackAction(false, args);
                        while(subAction.MoveNext()) yield return subAction.Current;
                        break;

                    case CombatPendingPlayerAction.AttemptRunAway:
                        subAction = RunAwayAction(args);
                        while(subAction.MoveNext()) yield return subAction.Current;
                        break;

                    case CombatPendingPlayerAction.OpenInventory:
                        subAction = OpenInventoryAction(args);
                        while(subAction.MoveNext()) yield return subAction.Current;
                        break;
                }

                if(args.Stop)
                    break;

                if(!args.BackToPlayerTurn)
                {
                    subAction = AttackAction(true, args);
                    while(subAction.MoveNext()) yield return subAction.Current;
                }

                args.BackToPlayerTurn = false;
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

            if(_inventoryUI != null)
                _inventoryUI.ManualUpdate();
        }

        public override void FixedUpdate(long count)
        {
        }

        private void DrawToScreenOrRenderTarget(SpriteBatch batch, Vector2 screenSize, RenderTarget2D target)
        {
            if(target != null)
            {
                batch.RenderTarget = target;
                batch.SamplerState = SamplerState.PointClamp;
            }
            else
                screenSize = Program.ScaleBatch(batch);
            
            batch.Transform = Matrix.CreateTranslation(_shakeOffset.X, _shakeOffset.Y, 0) * batch.Transform;

            string bg = "combat/plains.png";

            batch.Texture(new Vector2(0, -(float)_offset.Value), Assets.Get<Texture2D>(bg), Color.White);
            batch.Texture(new Vector2(0, 270 - (float)_offset.Value * 2), Assets.Get<Texture2D>("combat/menu.png"), Color.White);

            Font font = Assets.Get<Font>("fonts/bitcell.ttf");

            Matrix oldTransform = batch.Transform;
            batch.Transform = Matrix.CreateScale(2) * Matrix.CreateTranslation(80, 160 - (float)_offset.Value, 0) * oldTransform;
            Combatant.Draw(batch, false);
            Combatant.Draw(batch, true);

            batch.Transform = Matrix.CreateScale(2) * Matrix.CreateTranslation(screenSize.X - 80 - 32, 160 - (float)_offset.Value, 0) * oldTransform;
            if(Enemy is NPC)
                (Enemy as NPC).Draw(batch);
        
            batch.Transform = oldTransform;

            Menu.Draw(batch, new Rectangle(0, (int)(270 - (float)_offset.Value * 2), 480, Assets.Get<Texture2D>("combat/menu.png").Height), this);

            if(_showWarning)
            {
                Vector2 measure = font.Measure(32, "OH SHIT!!! " + Enemy.Name + " ENCOUNTERED");

                batch.Text(font, 32, "OH SHIT!!! " + Enemy.Name + " ENCOUNTERED", screenSize / 2 - measure / 2, Color.Red);
            }

            if(_inventoryUI != null)
            {
                batch.Rectangle(new Rectangle(0, 0, 480, 270), Color.Black * ((1.0f - _inventoryUI.MenuOffset) * 0.6f));
                _inventoryUI.ManualDraw();
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
