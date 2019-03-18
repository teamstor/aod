using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.Engine.Graphics;

namespace TeamStor.AOD.Menu.Elements
{
    /// <summary>
    /// Menu margin spacing.
    /// </summary>
    public class MenuSpacer : MenuElement
    {
        /// <summary>
        /// Amount of spacing added by this element.
        /// </summary>
        public int Amount
        {
            get; set;
        }

        public override Vector2 Measure { get { return new Vector2(0, Amount); } }
        public override bool Selectable { get { return false; } }

        public MenuSpacer(int amount) : base(null)
        {
            Amount = amount;
        }

        public override void OnDraw(SpriteBatch batch, Vector2 position, Vector2 mySize)
        {
        }
    }
}
