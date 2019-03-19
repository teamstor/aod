using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TeamStor.Engine;
using TeamStor.Engine.Coroutine;
using TeamStor.Engine.Graphics;

namespace TeamStor.AOD.Menu
{
	/// <summary>
	/// A menu UI page.
	/// </summary>
	public class MenuPage : IEnumerable<MenuElement>
	{
		private double _timeUpHeld = 0;
		private double _timeDownHeld = 0;

        private List<MenuElement> _elements = new List<MenuElement>();
		
		/// <summary>
		/// Parent menu.
		/// </summary>
		public MenuUI Parent { get; set; }
		
		/// <summary>
		/// The fixed width of this page. -1 = dynamic width
		/// </summary>
		public int FixedWidth { get; set; }
		
		/// <summary>
		/// Menu elements on this page.
		/// </summary>
		public ICollection<MenuElement> Elements { get { return _elements; } }
		
		/// <summary>
		/// Current selected element.
		/// -1 = transitioning
		/// </summary>
		public int SelectedElement { get; private set; }

        public Vector2 Area
        {
            get
            {
                int maxWidth = FixedWidth;
                int height = 0;

                foreach(MenuElement element in _elements)
                {
                    Vector2 measure = element.Measure;
                    Vector4 margins = element.Margins;
                    height += (int)margins.Y;
                    height += (int)measure.Y;
                    height += (int)margins.W;

                    if(FixedWidth == -1)
                    {
                        int width = (int)margins.X + (int)measure.Y + (int)margins.Z;
                        if(width > maxWidth)
                            maxWidth = width;
                    }
                }

                return new Vector2(maxWidth, height);
            }
        }

        public MenuPage(int fixedWidth = -1)
		{
			FixedWidth = fixedWidth;
		}

        /// <summary>
        /// Adds a new element to this page.
        /// </summary>
        /// <param name="element">The element to add.</param>
        /// <param name="after">The element this element should be added after (null = end or start of page)</param>
        /// <param name="over">If the element should be added over <paramref name="after"/>. </param>
        /// <returns>The added element, or an exception if the element does not belong to this page.</returns>
        public MenuElement Add(MenuElement element, MenuElement after = null, bool over = false)
        {
            if(element.Page == null)
                element.Page = this;
            if(element.Page != this)
                throw new Exception("MenuElement must be created with the correct page");

            int indexOf = _elements.IndexOf(after);
            if(after == null || indexOf == -1 || (indexOf == 0 && over) || (indexOf == _elements.Count - 1 && !over))
            {
                if(over)
                    _elements.Insert(0, element);
                else
                    _elements.Add(element);
            }
            else
                _elements.Insert(indexOf + (over ? -1 : 1), element);

            if(_elements.Count == 1)
                _elements[0].OnSelected(null);

            return element;
        }

        /// <summary>
        /// Removes a menu element. 
        /// </summary>
        /// <param name="element">The element to remove</param>
        /// <returns>True if menu element was removed.</returns>
        public bool Remove(MenuElement element)
        {
            if(element.Selected)
                element.OnDeselected(null);
            return _elements.Remove(element);
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The index to remove an element from.</param>
        public void RemoveAt(int index)
        {
            if(_elements[index].Selected)
                _elements[index].OnDeselected(null);
            _elements.RemoveAt(index);
        }

		private IEnumerator<ICoroutineOperation> ChangeSelectionCoroutine(int selection, MenuElement oldElement, float waitTimeBefore, float waitTimeAfter)
		{
            SelectedElement = -1;
            yield return Wait.Seconds(Parent.Parent.Game, waitTimeBefore);
            _elements[selection].OnSelected(oldElement);
			yield return Wait.Seconds(Parent.Parent.Game, waitTimeAfter);
			SelectedElement = selection;
		}

		public void ResetSelectedElement()
		{
			if(SelectedElement != 0)
			{
				int lastElement = SelectedElement;
				_elements[SelectedElement].OnDeselected(_elements[0]);
				SelectedElement = 0;
				_elements[SelectedElement].OnSelected(_elements[lastElement]);
			}
		}

		/// <summary>
		/// Called when this page is entered.
		/// </summary>
		/// <param name="lastPageID">The last page, or "".</param>
		public void OnPageEnter(string lastPageID)
		{
		}
		
		/// <summary>
		/// Called when this page is left.
		/// </summary>
		/// <param name="lastPageID">The next page, or "".</param>
		public void OnPageLeave(string nextPageID)
		{
			
		}

		/// <summary>
		/// Updates element selection and key events.
		/// </summary>
		public void UpdateElements(double deltaTime, InputManager input)
		{
			if(SelectedElement != -1)
			{
                int direction = 1;
				int nextElement = SelectedElement;

				double lastTimeUpHeld = _timeUpHeld;
				double lastTimeDownHeld = _timeDownHeld;

				if(InputMap.FindMapping(InputAction.Up).Held(input))
					_timeUpHeld += deltaTime;
				else
					_timeUpHeld = 0;
				
				if(InputMap.FindMapping(InputAction.Down).Held(input))
					_timeDownHeld += deltaTime;
				else
					_timeDownHeld = 0;

				bool isKeyRepeat = false;

				if(InputMap.FindMapping(InputAction.Up).Pressed(input) || _timeUpHeld > 0.3)
				{
					nextElement = SelectedElement == 0 ? _elements.Count - 1 : SelectedElement - 1;
					isKeyRepeat = !InputMap.FindMapping(InputAction.Up).Pressed(input);
                    direction = -1;
                }

				if(InputMap.FindMapping(InputAction.Down).Pressed(input) || _timeDownHeld > 0.3)
				{
					nextElement = SelectedElement == _elements.Count - 1 ? 0 : SelectedElement + 1;
					isKeyRepeat = !InputMap.FindMapping(InputAction.Down).Pressed(input);
                    direction = 1;
				}

                int originPoint = nextElement;
                while(!_elements[nextElement].Selectable)
                {
                    nextElement += direction;
                    if(direction == -1 && nextElement < 0)
                        nextElement = _elements.Count - 1;
                    if(direction == 1 && nextElement > _elements.Count - 1)
                        nextElement = 0;

                    // if we've gone a full loop there's no point in trying
                    if(nextElement == originPoint)
                        break;
                }

				if(nextElement != SelectedElement)
				{
					MenuElement oldElement = _elements[SelectedElement];
                    _elements[SelectedElement].OnDeselected(_elements[nextElement]);
					
					SelectedElement = -1;
					Parent.Parent.Coroutine.AddExisting(ChangeSelectionCoroutine(nextElement, oldElement, 0.05f, isKeyRepeat ? 0.03f : 0f));

					return;
				}
				
				if(InputMap.FindMapping(InputAction.Action).Held(input))
                    _elements[SelectedElement].OnClicked(!InputMap.FindMapping(InputAction.Action).Pressed(input));
				
				if(InputMap.FindMapping(InputAction.Left).Held(input))
                    _elements[SelectedElement].OnLeftButton(!InputMap.FindMapping(InputAction.Left).Pressed(input));
				if(InputMap.FindMapping(InputAction.Right).Held(input))
                    _elements[SelectedElement].OnRightButton(!InputMap.FindMapping(InputAction.Right).Pressed(input));
			}
		}

		/// <summary>
		/// Draws this menu page inside of the specified area.
		/// </summary>
		/// <param name="area">The area to draw inside.</param>
		public void DrawInsideArea(SpriteBatch batch, Rectangle area)
		{
			int y = area.Y;
			
			foreach(MenuElement element in _elements)
			{
				Vector2 measure = element.Measure;
				Vector4 margins = element.Margins;

				y += (int)margins.Y;
				element.OnDraw(batch, new Vector2(element.Center ? area.X + area.Width / 2 - measure.X / 2 : area.X + margins.X, y), measure);
				y += (int)measure.Y;
				y += (int)margins.W;
			}
		}

        public IEnumerator<MenuElement> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Elements.GetEnumerator();
        }
    }
}