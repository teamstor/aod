using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TeamStor.Engine.Graphics;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Menu.Elements
{
	/// <summary>
	/// Simple menu button. Can have two icons, a label and a sublabel.
	/// </summary>
	public class MenuButton : MenuElement
	{
        private Font _font;

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

		/// <summary>
        /// Alignment of the label.
        /// </summary>
		public MenuTextAlignment LabelAlignment { get; set; } = MenuTextAlignment.Left;

        /// <summary>
        /// Alignment of the sublabel.
        /// </summary>
        public MenuTextAlignment SublabelAlignment { get; set; } = MenuTextAlignment.Left;

        /// <summary>
        /// If an arrow should be shown besides the text when hovering.
        /// Used for showing this button goes to another page.
        /// </summary>
        public bool ShowArrowOnHover { get; set; }


        public override Vector2 Measure
		{
			get
			{
                if(_font == null)
                    _font = Page.Parent.Parent.Assets.Get<Font>("fonts/bitcell.ttf", true);

                int width = Page.FixedWidth != -1 ? Page.FixedWidth : 0;
                width -= (int)Margins.X;
                width -= (int)Margins.Z;

                if(width <= 0)
                {
                    int firstLineWidth = 0;

                    foreach(string s in Label.Split('\n'))
                        firstLineWidth = Math.Max(firstLineWidth, (int)_font.Measure(16, s).X);

                    if(IconLeft != "")
                        firstLineWidth += Page.Parent.Parent.Assets.Get<Texture2D>(IconLeft).Bounds.Width + 5;
                    if(IconRight != "")
                        firstLineWidth += Page.Parent.Parent.Assets.Get<Texture2D>(IconRight).Bounds.Width + 5;
                    if(ShowArrowOnHover)
                        firstLineWidth += 5 * 2;

                    int secondLineWidth = 0;

                    foreach(string s in Sublabel.Split('\n'))
                        secondLineWidth = Math.Max(secondLineWidth, (int)_font.Measure(16, s).X);

                    width = Math.Max(firstLineWidth, secondLineWidth);
                }

                int height = 0;
                if(!string.IsNullOrWhiteSpace(Label))
                    height += 8 * Label.Split('\n').Length;
                if(!string.IsNullOrWhiteSpace(Label) && !string.IsNullOrWhiteSpace(Sublabel))
                    height += 2;
                if(!string.IsNullOrWhiteSpace(Sublabel))
                    height += 8 * Sublabel.Split('\n').Length;

                if(IconLeft != "")
                    height = Math.Max(Page.Parent.Parent.Assets.Get<Texture2D>(IconLeft).Bounds.Height, height);
                if(IconRight != "")
                    height = Math.Max(Page.Parent.Parent.Assets.Get<Texture2D>(IconRight).Bounds.Height, height);

                return new Vector2(width, height);
			}
		}

		public override Vector4 Margins
		{
			get
			{
				return new Vector4(2, 2, 2, 2);
			}
		}

        public MenuButton(MenuPage page,
            string label,
            string icon = "",
            string iconRight = "",
            string sublabel = "",
            bool showArrow = false,
            MenuTextAlignment labelAlign = MenuTextAlignment.Left, 
            MenuTextAlignment sublabelAlign = MenuTextAlignment.Left) : base(page)
		{
            Label = label;
            Sublabel = sublabel;

            IconLeft = icon;
            IconRight = iconRight;

            ShowArrowOnHover = showArrow;

            LabelAlignment = labelAlign;
            SublabelAlignment = sublabelAlign;
        }
		
		public override void OnDraw(SpriteBatch batch, Vector2 position, Vector2 mySize)
		{
            if(_font == null)
                _font = Page.Parent.Parent.Assets.Get<Font>("fonts/bitcell.ttf", true);

            int myWidth = (int)mySize.X;
            int y = (int)position.Y - 9;
            float alpha = Selected ? 0.9f + (float)Math.Sin(Page.Parent.Parent.Game.Time * 10) * 0.1f : 0.6f;

            Texture2D iconLeft = IconLeft == "" ? null : Page.Parent.Parent.Assets.Get<Texture2D>(IconLeft);
            Texture2D iconRight = IconRight == "" ? null : Page.Parent.Parent.Assets.Get<Texture2D>(IconRight);

            if(iconLeft != null)
                batch.Texture(position + new Vector2(0, (int)mySize.Y / 2 - iconLeft.Height / 2), iconLeft, Color.White * alpha);
            if(iconRight != null)
                batch.Texture(position + new Vector2(myWidth - iconRight.Width, (int)mySize.Y / 2 - iconRight.Height / 2), iconRight, Color.White * alpha);

            if(!string.IsNullOrWhiteSpace(Label))
            {
                foreach(string s in Label.Split('\n'))
                {
                    int x = (int)position.X;

                    if(IconLeft != "")
                        x += iconLeft.Bounds.Width + 5;

                    Vector2 measure = _font.Measure(16, s);

                    if(LabelAlignment == MenuTextAlignment.Center)
                        x = (int)position.X + myWidth / 2 - (int)measure.X / 2;
                    if(LabelAlignment == MenuTextAlignment.Right)
                    {
                        x = (int)position.X + myWidth - (int)measure.X;

                        if(IconRight != "")
                            x -= iconRight.Bounds.Width + 5;
                        if(ShowArrowOnHover)
                            x -= 5 * 2;
                    }

                    batch.Text(_font, 16, s, new Vector2(x, y), Color.White * alpha);

                    if(ShowArrowOnHover && Selected)
                        batch.Texture(new Vector2(x + measure.X + 5, y + 9), Page.Parent.Parent.Assets.Get<Texture2D>("icons/arrow_right.png"), Color.White * alpha);

                    y += 8;
                }
            }

            if(!string.IsNullOrWhiteSpace(Label) && !string.IsNullOrWhiteSpace(Sublabel))
                y += 2;

            if(!string.IsNullOrWhiteSpace(Sublabel))
            {
                foreach(string s in Sublabel.Split('\n'))
                {
                    int x = (int)position.X;

                    if(IconLeft != "")
                        x += iconLeft.Width + 5;

                    Vector2 measure = _font.Measure(16, s);

                    if(SublabelAlignment == MenuTextAlignment.Center)
                        x = (int)position.X + myWidth / 2 - (int)measure.X / 2;
                    if(SublabelAlignment == MenuTextAlignment.Right)
                    {
                        x = (int)position.X + myWidth - (int)measure.X;

                        if(IconRight != "")
                            x -= iconRight.Width + 5;
                        if(ShowArrowOnHover)
                            x -= 5 * 2;
                    }

                    batch.Text(_font, 16, s, new Vector2(x, y), Color.White * alpha * 0.6f);
                    y += 8;
                }
            }
        }
    }
}