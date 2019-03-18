using Microsoft.Xna.Framework;
using TeamStor.Engine.Graphics;

namespace TeamStor.AOD.Menu
{
	/// <summary>
	/// An element that can be added to a MenuUI.
	/// </summary>
	public abstract class MenuElement
	{
		/// <summary>
		/// Page this element belongs to.
		/// </summary>
		public MenuPage Page { get; set; }
		
		/// <summary>
		/// The amount of width/height this element takes up.
		/// </summary>
		public abstract Vector2 Measure { get; }

		/// <summary>
		/// Amount of margin on each side of the element.
		/// X = Left
		/// Y = Top
		/// Z = Right
		/// W = Bottom
		/// </summary>
		public virtual Vector4 Margins
		{
			get { return Vector4.Zero; }
		}
		
		/// <summary>
		/// If the page should automatically center this element when drawing.
		/// </summary>
		public virtual bool Center
		{
			get { return false; }
		}

		/// <summary>
		/// If this element is selected.
		/// </summary>
		public bool Selected = false;

        /// <summary>
        /// If this element can be selected.
        /// </summary>
        public virtual bool Selectable { get { return true; } }

		public MenuElement(MenuPage page)
		{
			Page = page;
		}
		
		/// <summary>
		/// Called when this element is clicked by the user.
		/// </summary>
		/// <param name="holding">If the key is being held or just pressed.</param>
		public virtual void OnClicked(bool holding) { }
		
		/// <summary>
		/// Called when the left arrow is being held by the user.
		/// </summary>
		/// <param name="holding">If the key is being held or just pressed.</param>
		public virtual void OnLeftButton(bool holding) { }
		
		/// <summary>
		/// Called when the right arrow is being held by the user.
		/// </summary>
		/// <param name="holding">If the key is being held or just pressed.</param>
		public virtual void OnRightButton(bool holding) { }

		/// <summary>
		/// Called when this item is selected by the user, or when the page is entered.
		/// </summary>
		/// <param name="lastElement">The last element to be selected, or null.</param>
		public virtual void OnSelected(MenuElement lastElement)
		{
			Selected = true;
		}

		/// <summary>
		/// Called when another item is selected by the user, or when the page is left.
		/// </summary>
		/// <param name="nextElement">The new element to be selected, or null.</param>
		public virtual void OnDeselected(MenuElement nextElement)
		{
			Selected = false;
		}
		
		/// <summary>
		/// Called when this element should be drawn.
		/// </summary>
		/// <param name="batch">The sprite batch to draw with.</param>
		/// <param name="position">The position to draw the element at.</param>
        /// <param name="mySize">Size of the element being drawn.</param>
		public abstract void OnDraw(SpriteBatch batch, Vector2 position, Vector2 mySize);
	}
}