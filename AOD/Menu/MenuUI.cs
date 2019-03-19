using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine;
using TeamStor.Engine.Coroutine;
using TeamStor.Engine.Tween;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Menu
{
	public enum MenuUIState
	{
		Closed,
		Transitioning,
		Open
	}
	
	/// <summary>
	/// Menu UI containing several pages and buttons.
	/// </summary>
	public class MenuUI
	{
		private Dictionary<string, MenuPage> _pages = new Dictionary<string, MenuPage>();

        private string _nextPageID = "";
        private TweenedDouble _pageTransition;
        private bool _pageTransitionGoingBack = false;

		private TweenedDouble _alpha;
		
		/// <summary>
		/// Game state this menu UI is associated with.
		/// </summary>
		public GameState Parent { get; private set; }
				
		/// <summary>
		/// If the menu should use transitions when switching between pages.
		/// </summary>
		public bool UseTransitions { get; set; } = true;
		
		/// <summary>
		/// The state the UI is in.
		/// Transitioning can be either open or closed.
		/// </summary>
		public MenuUIState State { get; private set; }
		
		/// <summary>
		/// The current area of the menu.
		/// </summary>
		public TweenedVector2 Area { get; private set; }

        /// <summary>
        /// The ID of the current page.
        /// </summary>
        public string SelectedPageID { get; private set; }

        /// <summary>
        /// The currently selected page.
        /// </summary>
        public MenuPage SelectedPage
		{
			get { return _pages[SelectedPageID]; }
		}

        /// <summary>
        /// If the menu UI should have a background.
        /// </summary>
        public bool DrawBackground { get; set; } = true;

		private Vector2 RecalculateArea(string page)
		{
			Vector2 measure = _pages[page].Area + new Vector2(7 * 2,  7 * 2);
			return measure;
		}

		public MenuUI(GameState state, string initialPageId, MenuPage initialPage, bool startClosed)
		{
			Parent = state;
			_pages.Add(initialPageId, initialPage);
			SelectedPageID = initialPageId;
			initialPage.Parent = this;
			SelectedPage.OnPageEnter("");

			State = startClosed ? MenuUIState.Closed : MenuUIState.Open;
			_alpha = new TweenedDouble(state.Game, startClosed ? 0 : 1);
			Area = new TweenedVector2(state.Game, startClosed ? new Vector2(RecalculateArea(SelectedPageID).X, 7 * 2) : RecalculateArea(SelectedPageID));
            _pageTransition = new TweenedDouble(state.Game, 0);
        }

        public MenuPage AddPage(string pageID, MenuPage page)
        {
            _pages.Add(pageID, page);
            page.Parent = this;
            return page;
        }

        private IEnumerator<ICoroutineOperation> ToggleCoroutine(bool closing)
		{
			if(!UseTransitions)
			{
				State = closing ? MenuUIState.Closed : MenuUIState.Open;
				_alpha = new TweenedDouble(Parent.Game, closing ? 0 : 1);
				Area = new TweenedVector2(Parent.Game, closing ? new Vector2(RecalculateArea(SelectedPageID).X, 7 * 2) : RecalculateArea(SelectedPageID));
				
				yield break;
			}

			if(closing)
			{
				Area.TweenTo(new Vector2(RecalculateArea(SelectedPageID).X, 7 * 2), TweenEaseType.EaseInOutSine, 0.14f);
				yield return Wait.Seconds(Parent.Game, 0.14f - 0.06f);
				_alpha.TweenTo(closing ? 0 : 1, TweenEaseType.EaseInOutSine, 0.06f);
				yield return Wait.Seconds(Parent.Game, 0.06f);
			}
			else
			{
				Area.TweenTo(RecalculateArea(SelectedPageID), TweenEaseType.EaseInOutSine, 0.14f);
				_alpha.TweenTo(closing ? 0 : 1, TweenEaseType.EaseInOutSine, 0.06f);
				yield return Wait.Seconds(Parent.Game, 0.14f);
			}

			State = closing ? MenuUIState.Closed : MenuUIState.Open;
		}

        /// <summary>
        /// Toggles the open/close state of this menu.
        /// You can check result_value.Current == null to see when the toggle is done.
        /// This function returns null if a transition is in progress.
        /// </summary>
        public IEnumerator<ICoroutineOperation> Toggle()
		{
			if(State == MenuUIState.Transitioning)
				return null;
			
			IEnumerator<ICoroutineOperation> toggleOperation = ToggleCoroutine(State == MenuUIState.Open);
			State = MenuUIState.Transitioning;
			
			Parent.Coroutine.AddExisting(toggleOperation);
			return toggleOperation;
		}

        private IEnumerator<ICoroutineOperation> SwitchCoroutine()
        {
            string lastID = SelectedPageID;
            SelectedPage.OnPageLeave(_nextPageID);
            Area.TweenTo(RecalculateArea(_nextPageID), TweenEaseType.EaseInOutSine, 0.2f);
            _pageTransition.TweenTo(1, TweenEaseType.EaseInOutSine, 0.2f);

            yield return Wait.Seconds(Parent.Game, 0.2f);
            SelectedPageID = _nextPageID;
            SelectedPage.OnPageEnter(lastID);
            _nextPageID = "";

            State = MenuUIState.Open;
        }

        /// <summary>
        /// Switches to another page with a transition.
        /// You can check result_value.Current == null to see when the switch is done.
        /// This function returns null if:
        /// * a transition is in progress 
        /// * the page is the same as the current page
        /// * the menu is closed
        /// In the case of the menu being closed, the pages will be switched but no transition will occur.
        /// </summary>
        /// <param name="nextPageID">The page to switch to.</param>
        /// <param name="goingBack">If the transition should go to the left rather than the right.</param>
        public IEnumerator<ICoroutineOperation> SwitchPage(string nextPageID, bool goingBack = false)
        {
            if(State == MenuUIState.Closed || !UseTransitions)
            {
                SelectedPage.OnPageLeave(nextPageID);
                string lastID = SelectedPageID;
                SelectedPageID = nextPageID;
                SelectedPage.OnPageEnter(lastID);
                return null;
            }

            if(nextPageID == SelectedPageID || State == MenuUIState.Transitioning)
                return null;

            IEnumerator<ICoroutineOperation> switchOperation = SwitchCoroutine();
            State = MenuUIState.Transitioning;
            _pageTransition.TweenTo(0, TweenEaseType.Linear, 0);
            _pageTransitionGoingBack = goingBack;
            _nextPageID = nextPageID;

            if(!goingBack)
	            _pages[_nextPageID].ResetSelectedElement();

            Parent.Coroutine.AddExisting(switchOperation);
            return switchOperation;
        }

        /// <summary>
        /// Updates this MenuUI.
        /// </summary>
        public void Update(double deltaTime, InputManager input)
		{
			if(State == MenuUIState.Open)
				SelectedPage.UpdateElements(deltaTime, input);
		}

		/// <summary>
		/// Draws this MenuUI at the specified position.
		/// </summary>
		/// <param name="position">The position of the top-left corner of the menu.</param>
		/// <param name="batch">The sprite batch to draw with.</param>
		/// <param name="screenSize">The size of the screen.</param>
		public void Draw(Vector2 position, SpriteBatch batch, Vector2 screenSize)
		{
			position = new Vector2((int)position.X, (int)position.Y);

            if(DrawBackground)
            {
                batch.Texture(position, Parent.Assets.Get<Texture2D>("ui/menu_corner.png"), Color.White * _alpha, null, null, 0, null, SpriteEffects.None);
                batch.Texture(position + new Vector2((int)Area.Value.X - 7, 0), Parent.Assets.Get<Texture2D>("ui/menu_corner.png"), Color.White * _alpha, null, null, 0, null, SpriteEffects.FlipHorizontally);
                batch.Texture(position + new Vector2(0, (int)Area.Value.Y - 7), Parent.Assets.Get<Texture2D>("ui/menu_corner.png"), Color.White * _alpha, null, null, 0, null, SpriteEffects.FlipVertically);
                batch.Texture(position + new Vector2((int)Area.Value.X - 7, (int)Area.Value.Y - 7), Parent.Assets.Get<Texture2D>("ui/menu_corner.png"), Color.White * _alpha, null, null, 0, null, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically);
            }

			Vector2 fillArea = Area.Value - new Vector2(7 * 2, 7 * 2);
            if(DrawBackground)
			    batch.Rectangle(new Rectangle((int)position.X + 7, (int)position.Y + 7, (int)fillArea.X, (int)fillArea.Y), new Color(68, 68, 242) * _alpha);
			
			batch.SamplerState = SamplerState.PointWrap;

            if(DrawBackground)
            {
                batch.Texture(new Rectangle((int)position.X + 7, (int)position.Y, (int)fillArea.X, 7), Parent.Assets.Get<Texture2D>("ui/menu_top.png"), Color.White * _alpha);
                batch.Texture(new Rectangle((int)position.X + 7, (int)position.Y + (int)Area.Value.Y - 7, (int)fillArea.X, 7), Parent.Assets.Get<Texture2D>("ui/menu_top.png"), Color.White * _alpha, null, 0, null, SpriteEffects.FlipVertically);

                batch.Texture(new Rectangle((int)position.X, (int)position.Y + 7, 7, (int)fillArea.Y), Parent.Assets.Get<Texture2D>("ui/menu_side.png"), Color.White * _alpha);
                batch.Texture(new Rectangle((int)position.X + (int)Area.Value.X - 7, (int)position.Y + 7, 7, (int)fillArea.Y), Parent.Assets.Get<Texture2D>("ui/menu_side.png"), Color.White * _alpha, null, 0, null, SpriteEffects.FlipHorizontally);
            }

			batch.Scissor = new Rectangle((int)position.X + 7, (int)position.Y + 7, (int)fillArea.X, (int)fillArea.Y);
			Vector2 transformLT = Vector2.Transform(batch.Scissor.Value.Location.ToVector2(), batch.Transform);
			Vector2 transformRB = Vector2.Transform(new Vector2(batch.Scissor.Value.Right, batch.Scissor.Value.Bottom), batch.Transform);
			batch.Scissor = new Rectangle(
				(int)transformLT.X,
				(int)transformLT.Y,
				(int)(transformRB.X - transformLT.X),
				(int)(transformRB.Y - transformLT.Y));
            if(Area.Value == Area.TargetValue && _alpha.Value == 1)
                SelectedPage.DrawInsideArea(batch, 
                    new Rectangle((int)position.X + 7, (int)position.Y + 7, (int)fillArea.X, (int)fillArea.Y));

			batch.Scissor = null;
		}
	}
}