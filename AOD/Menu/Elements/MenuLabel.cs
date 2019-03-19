using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TeamStor.Engine.Graphics;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Menu.Elements
{
	/// <summary>
	/// Text alignment for menu elements.
	/// </summary>
	public enum MenuTextAlignment
	{
		Left,
		Center,
		Right
	}
	
    /// <summary>
    /// Simple menu label.
    /// </summary>
	public class MenuLabel : MenuElement
	{
        private Font _font;

        /// <summary>
        /// Text on this label. \n = new line
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Icon to show on the left side of the label.
        /// </summary>
        public string IconLeft { get; set; }

        /// <summary>
        /// Icon to show on the right side of the label.
        /// </summary>
        public string IconRight { get; set; }

        /// <summary>
        /// Alignment of the label.
        /// </summary>
        public MenuTextAlignment Alignment { get; set; } = MenuTextAlignment.Left;
        
        /// <summary>
        /// Opacity of the text.
        /// </summary>
        public float Opacity { get; set; } = 1.0f;

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
                    int labelWidth = 0;

                    foreach(string s in Text.Split('\n'))
                        labelWidth = Math.Max(labelWidth, (int)_font.Measure(16, s).X);

                    if(IconLeft != "")
                        labelWidth += Page.Parent.Parent.Assets.Get<Texture2D>(IconLeft).Bounds.Width + 5;
                    if(IconRight != "")
                        labelWidth += Page.Parent.Parent.Assets.Get<Texture2D>(IconRight).Bounds.Width + 5;

                    width = labelWidth;
                }

                int height = 0;
                if(!string.IsNullOrWhiteSpace(Text))
                    height += 8 * Text.Split('\n').Length;

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

        public override bool Selectable
        {
            get
            {
                return false;
            }
        }

        public MenuLabel(MenuPage page, string label, string icon = "", string iconRight = "", float opacity = 1, MenuTextAlignment align = MenuTextAlignment.Left) : base(page)
        {
            Text = label;

            IconLeft = icon;
            IconRight = iconRight;

            Opacity = opacity;
            Alignment = align;
        }

        public override void OnDraw(SpriteBatch batch, Vector2 position, Vector2 mySize)
        {
            int myWidth = (int)mySize.X;
            int y = (int)position.Y - 9;

            Texture2D iconLeft = IconLeft == "" ? null : Page.Parent.Parent.Assets.Get<Texture2D>(IconLeft);
            Texture2D iconRight = IconRight == "" ? null : Page.Parent.Parent.Assets.Get<Texture2D>(IconRight);

            if(iconLeft != null)
                batch.Texture(position + new Vector2(0, (int)mySize.Y / 2 - iconLeft.Height / 2), iconLeft, Color.White * Opacity);
            if(iconRight != null)
                batch.Texture(position + new Vector2(myWidth - iconRight.Width, (int)mySize.Y / 2 - iconRight.Height / 2), iconRight, Color.White * Opacity);

            if(!string.IsNullOrWhiteSpace(Text))
            {
                foreach(string s in Text.Split('\n'))
                {
                    int x = (int)position.X;

                    if(IconLeft != "")
                        x += iconLeft.Bounds.Width + 5;

                    Vector2 measure = _font.Measure(16, s);

                    if(Alignment == MenuTextAlignment.Center)
                        x = (int)position.X + myWidth / 2 - (int)measure.X / 2;
                    if(Alignment == MenuTextAlignment.Right)
                    {
                        x = (int)position.X + myWidth - (int)measure.X;

                        if(IconRight != "")
                            x -= iconRight.Bounds.Width + 5;
                    }

                    batch.Text(_font, 16, s, new Vector2(x, y), Color.White * Opacity);
                    y += 8;
                }
            }
        }
    }
}