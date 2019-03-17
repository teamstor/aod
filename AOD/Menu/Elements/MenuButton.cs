using Microsoft.Xna.Framework;
using TeamStor.Engine.Graphics;

namespace TeamStor.AOD.Menu.Elements
{
	/// <summary>
	/// Simple menu button. Can have two icons, a label and a sublabel.
	/// </summary>
	public class MenuButton : MenuElement
	{
		/// <summary>
		/// Main label of the button. \n = new line
		/// </summary>
		public string Label { get; set; }
		
		/// <summary>
		/// Sublabel of the button. \n = new line
		/// </summary>
		public string Sublabel { get; set; }
		
		/// <summary>
		/// Icon to show on the left side of the button.
		/// </summary>
		public string IconLeft { get; set; }
		
		/// <summary>
		/// Icon to show on the right side of the button.
		/// </summary>
		public string IconRight { get; set; }

		
		public MenuTextAlignment LabelAlignment { get; set; } = MenuTextAlignment.Left;
		
		public override Vector2 Measure
		{
			get
			{
				return new Vector2(0, 8);
			}
		}

		public override Vector4 Margins
		{
			get
			{
				return new Vector4(2, 2, 2, 2);
			}
		}

		public MenuButton(MenuPage page) : base(page)
		{
		}
		
		public override void OnDraw(SpriteBatch batch, Vector2 position)
		{
			batch.Text(Page.Parent.Parent.Assets.Get<Font>("fonts/bitcell.ttf"), 16, "ewfewgwgew", position - new Vector2(0, 9), Color.White * (Selected ? 1 : 0.6f));
		}
	}
}