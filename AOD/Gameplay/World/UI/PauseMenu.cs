using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.AOD.Menu;
using TeamStor.AOD.Menu.Elements;
using TeamStor.Engine;
using TeamStor.Engine.Coroutine;
using TeamStor.Engine.Tween;
using Game = TeamStor.Engine.Game;

namespace TeamStor.AOD.Gameplay.World.UI
{
    /// <summary>
    /// Game pause menu.
    /// </summary>
    public class PauseMenuUI
    {
        private bool _closed = false;
        private TweenedDouble _alpha;

        private MenuUI _menu;
        private MenuOptions _options;

        private OnPauseMenuCompleted _completedEvent;
        public delegate void OnPauseMenuCompleted();

        private WorldState _world;

        /// <summary>
        /// If this pause menu UI has been closed and has finished showing.
        /// </summary>
        public bool IsShowingCompleted
        {
            get; private set;
        } = false;

        private PauseMenuUI(WorldState world)
        {
            _world = world;
        }

        private IEnumerator<ICoroutineOperation> ShowCoroutine()
        {
            MenuPage mainPage = new MenuPage(150);
            mainPage.Add(new MenuButton(mainPage, "Resume Game", "icons/start_game.png")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) _closed = true; });
            mainPage.Add(new MenuButton(mainPage, "Options", "icons/settings.png", "", "")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) _options.SwitchToOptionsPage(); });
            // TODO: add "are you sure"
            mainPage.Add(new MenuButton(mainPage, "Return To Main Menu", "icons/arrow_left.png")).
                RegisterEvent(MenuElement.EventType.Clicked, (e, h) => { if(!h) _world.Game.CurrentState = new MainMenuState(); });

            _menu = new MenuUI(_world, "main", mainPage, true);
            _options = new MenuOptions(_menu, "main");

            _alpha = new TweenedDouble(_world.Game, 0);

            _world.Paused = true;
            _world.UpdateHook += UpdateHook;
            _world.DrawHook += DrawHook;

            IEnumerator<ICoroutineOperation> uiEnumerator = _menu.Toggle();
            _alpha.TweenTo(1, TweenEaseType.Linear, 0.08f);
            yield return Wait.Seconds(_world.Game, 0.18f);

            while(!_closed)
                yield return null;

            uiEnumerator = _menu.Toggle();
            yield return Wait.Seconds(_world.Game, 0.18f - 0.08f);
            _alpha.TweenTo(0, TweenEaseType.Linear, 0.08f);
            yield return Wait.Seconds(_world.Game, 0.08f);

            _world.UpdateHook -= UpdateHook;
            _world.DrawHook -= DrawHook;
            _world.Paused = false;

            if(_completedEvent != null)
                _completedEvent();

            IsShowingCompleted = true;
        }

        private void UpdateHook(object sender, Game.UpdateEventArgs e)
        {
            if(InputMap.FindMapping(InputAction.Menu).Pressed(_world.Input))
                _closed = true;

            _options.UpdateLabels();
            _menu.Update(e.DeltaTime, _world.Input);
        }

        private void DrawHook(object sender, WorldState.DrawEventArgs e)
        {
            e.Batch.Rectangle(new Rectangle(0, 0, (int)e.ScreenSize.X, (int)e.ScreenSize.Y), Color.Black * _alpha * 0.3f);

            Vector2 uiPos = e.ScreenSize / 2 - _menu.Area.Value / 2;
            _menu.Draw(uiPos, e.Batch, e.ScreenSize);
        }

        public static IEnumerator<ICoroutineOperation> Show(WorldState world, OnPauseMenuCompleted completeEvent = null)
        {
            PauseMenuUI pui = new PauseMenuUI(world);
            pui._completedEvent = completeEvent;
            return world.Coroutine.Start(pui.ShowCoroutine);
        }
    }
}
