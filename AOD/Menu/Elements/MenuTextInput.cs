using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDL2;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Menu.Elements
{
    /// <summary>
    /// Menu text input.
    /// </summary>
    public class MenuTextInput : MenuElement
    {
        private Font _font;

        /// <summary>
        /// Text input by the user.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Main label of the text box. \n = new line
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Sublabel of the text box. \n = new line
        /// </summary>
        public string Sublabel { get; set; }

        /// <summary>
        /// Icon to show on the left side of the text box.
        /// </summary>
        public string IconLeft { get; set; }

        /// <summary>
        /// Icon to show on the right side of the text box.
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

                    foreach(string s in (Label + Text).Split('\n'))
                        firstLineWidth = Math.Max(firstLineWidth, (int)_font.Measure(16, s).X);

                    if(IconLeft != "")
                        firstLineWidth += Page.Parent.Parent.Assets.Get<Texture2D>(IconLeft).Bounds.Width + 5;
                    if(IconRight != "")
                        firstLineWidth += Page.Parent.Parent.Assets.Get<Texture2D>(IconRight).Bounds.Width + 5;

                    int secondLineWidth = 0;

                    foreach(string s in Sublabel.Split('\n'))
                        secondLineWidth = Math.Max(secondLineWidth, (int)_font.Measure(16, s).X);

                    width = Math.Max(firstLineWidth, secondLineWidth);
                }

                int height = 0;
                if(!string.IsNullOrWhiteSpace((Label + Text)))
                    height += 8 * (Label + Text).Split('\n').Length;
                if(!string.IsNullOrWhiteSpace((Label + Text)) && !string.IsNullOrWhiteSpace(Sublabel))
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

        public MenuTextInput(MenuPage page,
            string initialText,
            string label,
            string icon = "",
            string iconRight = "",
            string sublabel = "",
            MenuTextAlignment labelAlign = MenuTextAlignment.Left,
            MenuTextAlignment sublabelAlign = MenuTextAlignment.Left) : base(page)
        {
            Text = initialText;

            Label = label;
            Sublabel = sublabel;

            IconLeft = icon;
            IconRight = iconRight;

            LabelAlignment = labelAlign;
            SublabelAlignment = sublabelAlign;
        }

        public override void OnSelected(MenuElement lastElement)
        {
            base.OnSelected(lastElement);

            TextInputEXT.TextInput += OnTextInput;
            TextInputEXT.StartTextInput();
            Page.Parent.Parent.Game.OnStateChange += OnStateChange;
        }

        private void OnStateChange(object sender, Engine.Game.ChangeStateEventArgs e)
        {
            TextInputEXT.StopTextInput();
            TextInputEXT.TextInput -= OnTextInput;
        }

        private void OnTextInput(char c)
        {
            string oldText = Text;

            if(c == (char)22)
                Text += SDL.SDL_GetClipboardText();

            if(c == '\b' && Text.Length > 0)
                Text = Text.Substring(0, Text.Length - 1);
            else if(Char.IsLetterOrDigit(c) || Char.IsPunctuation(c) || c == ' ')
                Text += c;

            Text = Text.TrimStart();
        }

        public override void OnDeselected(MenuElement nextElement)
        {
            base.OnDeselected(nextElement);

            TextInputEXT.StopTextInput();
            TextInputEXT.TextInput -= OnTextInput;
            Page.Parent.Parent.Game.OnStateChange -= OnStateChange;
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

            if(!string.IsNullOrWhiteSpace((Label + Text)))
            {
                foreach(string s in (Label + Text).Split('\n'))
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
                    }

                    string s2 = s;
                    if(Selected && (Page.Parent.Parent.Game.TotalFixedUpdates % 30) < 15)
                        s2 += "|";

                    batch.Text(_font, 16, s2, new Vector2(x, y), Color.White * alpha);

                    y += 8;
                }
            }

            if(!string.IsNullOrWhiteSpace((Label + Text)) && !string.IsNullOrWhiteSpace(Sublabel))
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
                    }

                    batch.Text(_font, 16, s, new Vector2(x, y), Color.White * alpha * 0.6f);
                    y += 8;
                }
            }
        }
    }
}
