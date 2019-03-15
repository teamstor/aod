using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TeamStor.Engine.Coroutine;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;

using Game = TeamStor.Engine.Game;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Gameplay.World.UI
{
    /// <summary>
    /// Shows the players stats and equips.
    /// </summary>
    public class PlayerUI
    {
        private bool _closed = false;
        private bool _transitioning = false;

        private OnPlayerUICompleted _completedEvent;
        public delegate void OnPlayerUICompleted(Player entity);

        private TweenedDouble _offsetY;

        private WorldState _world;
        private Player _entity;

        /// <summary>
        /// If this player UI has been closed and has finished showing.
        /// </summary>
        public bool IsShowingCompleted
        {
            get; private set;
        } = false;

        private PlayerUI(WorldState world, Player entity, bool noOffset)
        {
            _world = world;
            _entity = entity;

            _offsetY = new TweenedDouble(world.Game, noOffset ? 0.0 : 1.0);
        }

        private IEnumerator<ICoroutineOperation> ShowCoroutine()
        {
            _world.Paused = true;
            _world.UpdateHook += UpdateHook;
            _world.DrawHook += DrawHook;

            _offsetY.TweenTo(0.0, TweenEaseType.EaseOutQuad, 0.1);
            while(!_offsetY.IsComplete)
                yield return null;

            yield return Wait.Seconds(_world.Game, 0.1);

            while(!_closed)
                yield return null;

            if(_transitioning)
                yield return Wait.Seconds(_world.Game, 0.1);
            else
            {
                _offsetY.TweenTo(1.0, TweenEaseType.EaseInQuad, 0.1);
                while(!_offsetY.IsComplete)
                    yield return null;
            }

            if(!_transitioning)
                _world.Paused = false;
            _world.UpdateHook -= UpdateHook;
            _world.DrawHook -= DrawHook;

            if(_completedEvent != null)
                _completedEvent(_entity);

            IsShowingCompleted = true;
        }

        private void UpdateHook(object sender, Game.UpdateEventArgs e)
        {
            if(_offsetY == 0)
            {
                if(!_closed && InputMap.FindMapping(InputAction.Inventory).Pressed(_world.Input))
                {
                    InventoryUI.Show(_world, _entity, null, true);
                    _transitioning = true;
                }

                if(!_closed && InputMap.FindMapping(InputAction.Spells).Pressed(_world.Input))
                {
                    SpellsUI.Show(_world, null, true);
                    _transitioning = true;
                }

                if(InputMap.FindMapping(InputAction.Back).Pressed(_world.Input) ||
                   InputMap.FindMapping(InputAction.Inventory).Pressed(_world.Input) ||
                   InputMap.FindMapping(InputAction.Spells).Pressed(_world.Input))
                    _closed = true;
            }
        }

        private void DrawHook(object sender, WorldState.DrawEventArgs args)
        {
            WorldState world = sender as WorldState;
            SpriteBatch batch = args.Batch;
            Vector2 screenSize = args.ScreenSize;

            Texture2D bg = world.Assets.Get<Texture2D>("ui/player.png");
            batch.Texture(new Vector2(0, 270 - bg.Height + (bg.Height + 20) * (float)_offsetY.Value), bg, Color.White);

            Font font = world.Assets.Get<Font>("fonts/bitcell.ttf");

            int x = 16;

            batch.Text(font,
                16,
                "[" + InputMap.FindMapping(InputAction.Inventory).Key + "] Inventory",
                new Vector2(x, 8 + (bg.Height + 20) * (float)_offsetY.Value),
                Color.White * 0.6f);

            x += (int)font.Measure(16, "[" + InputMap.FindMapping(InputAction.Inventory).Key + "] Inventory").X + 6;

            batch.Text(font,
                16,
                "[" + InputMap.FindMapping(InputAction.Spells).Key + "] Spells",
                new Vector2(x, 8 + (bg.Height + 20) * (float)_offsetY.Value),
                Color.White * 0.6f);

            x += (int)font.Measure(16, "[" + InputMap.FindMapping(InputAction.Spells).Key + "] Spells").X + 6;

            batch.Text(font,
                16,
                "[" + InputMap.FindMapping(InputAction.Player).Key + "] " + _entity.Name,
                new Vector2(x, 8 + (bg.Height + 20) * (float)_offsetY.Value),
                Color.White);

            Player player = world.Player;
            Point oldPosition = player.Position;
            Direction oldHeading = player.Heading;
            player.MoveInstantly(new Point(0, 0));

            switch(((int)(_world.Game.Time * 3) % 4))
            {
                case 0:
                    player.Heading = Direction.Left;
                    break;

                case 1:
                    player.Heading = Direction.Up;
                    break;

                case 2:
                    player.Heading = Direction.Right;
                    break;

                case 3:
                    player.Heading = Direction.Down;
                    break;
            }

            Matrix oldTransform = batch.Transform;
            Vector3 scale, translation;
            Quaternion rot;
            batch.Transform.Decompose(out scale, out rot, out translation);
            batch.Transform = oldTransform * Matrix.CreateTranslation((screenSize.X / 2 - 8) * scale.X, (70 + 16 + (bg.Height + 20) * (float)_offsetY.Value) * scale.Y, 0);

            player.Draw(batch, false);
            player.Draw(batch, true);

            batch.Transform = oldTransform;
            player.MoveInstantly(oldPosition);
            player.Heading = oldHeading;

            Vector2 measure = font.Measure(16, "Level " + player.Level);
            batch.Text(font, 16, "Level " + player.Level, new Vector2(screenSize.X / 2 - measure.X / 2, 100 + (bg.Height + 20) * (float)_offsetY.Value), Color.White * 0.8f);

            Rectangle xpBarRectangle = new Rectangle((int)screenSize.X / 2 - 80, (int)(120 + (bg.Height + 20) * (float)_offsetY.Value), 160, 2);
            batch.Rectangle(xpBarRectangle, Color.White * 0.4f);

            xpBarRectangle.Width = (int)(xpBarRectangle.Width * ((float)player.XP / player.NeededXP));
            batch.Rectangle(xpBarRectangle, Color.White * 0.4f);
        }

        public static IEnumerator<ICoroutineOperation> Show(WorldState world, OnPlayerUICompleted completeEvent = null, bool noOffset = false)
        {
            PlayerUI pui = new PlayerUI(world, world.Player, noOffset);
            pui._completedEvent = completeEvent;
            return world.Coroutine.Start(pui.ShowCoroutine);
        }
    }
}