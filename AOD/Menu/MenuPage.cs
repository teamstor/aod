using System;
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
	public class MenuPage
	{
		private double _timeUpHeld = 0;
		private double _timeDownHeld = 0;
		
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
		public List<MenuElement> Elements { get; private set; } = new List<MenuElement>();
		
		/// <summary>
		/// Current selected element.
		/// -1 = transitioning
		/// </summary>
		public int SelectedElement { get; private set; }

		public MenuPage(int fixedWidth, MenuElement initialElement)
		{
			FixedWidth = fixedWidth;
			
			Elements.Add(initialElement);
			initialElement.Page = this;
			initialElement.OnSelected(null);
		}

		public Vector2 Area
		{
			get
			{
				int maxWidth = FixedWidth;
				int height = 0;

				foreach(MenuElement element in Elements)
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

		private IEnumerator<ICoroutineOperation> ChangeSelectionCoroutine(int selection, MenuElement oldElement, float waitTimeBefore, float waitTimeAfter)
		{
			yield return Wait.Seconds(Parent.Parent.Game, waitTimeBefore);
			Elements[selection].OnSelected(oldElement);
			yield return Wait.Seconds(Parent.Parent.Game, waitTimeAfter);
			SelectedElement = selection;
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
					nextElement = SelectedElement == 0 ? Elements.Count - 1 : SelectedElement - 1;
					isKeyRepeat = !InputMap.FindMapping(InputAction.Up).Pressed(input);
				}

				if(InputMap.FindMapping(InputAction.Down).Pressed(input) || _timeDownHeld > 0.3)
				{
					nextElement = SelectedElement == Elements.Count - 1 ? 0 : SelectedElement + 1;
					isKeyRepeat = !InputMap.FindMapping(InputAction.Down).Pressed(input);
				}

				if(nextElement != SelectedElement)
				{
					MenuElement oldElement = Elements[SelectedElement];
					Elements[SelectedElement].OnDeselected(Elements[nextElement]);
					
					SelectedElement = -1;
					Parent.Parent.Coroutine.AddExisting(ChangeSelectionCoroutine(nextElement, oldElement, 0.05f, isKeyRepeat ? 0.03f : 0f));

					return;
				}
				
				if(InputMap.FindMapping(InputAction.Action).Held(input))
					Elements[SelectedElement].OnClicked(!InputMap.FindMapping(InputAction.Action).Pressed(input));
				
				if(InputMap.FindMapping(InputAction.Left).Held(input))
					Elements[SelectedElement].OnLeftButton(!InputMap.FindMapping(InputAction.Left).Pressed(input));
				if(InputMap.FindMapping(InputAction.Right).Held(input))
					Elements[SelectedElement].OnRightButton(!InputMap.FindMapping(InputAction.Right).Pressed(input));
			}
		}

		/// <summary>
		/// Draws this menu page inside of the specified area.
		/// </summary>
		/// <param name="area">The area to draw inside.</param>
		public void DrawInsideArea(SpriteBatch batch, Rectangle area)
		{
			int y = area.Y;
			
			foreach(MenuElement element in Elements)
			{
				Vector2 measure = element.Measure;
				Vector4 margins = element.Margins;

				y += (int)margins.Y;
				measure.X += margins.X + margins.Z;
				element.OnDraw(batch, new Vector2(element.Center ? area.X + area.Width / 2 - measure.X / 2 : area.X + margins.X, y));
				y += (int)measure.Y;
				y += (int)margins.W;
			}
		}
	}
}