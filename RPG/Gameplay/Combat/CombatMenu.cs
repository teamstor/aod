using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;

using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.RPG.Gameplay
{
    public enum CombatMenuPage
    {
        None,

        /// <summary>
        /// Start page that is used for selecting a combat action.
        /// </summary>
        ActionSelection,
        /// <summary>
        /// Page that is used for selecting what magic attack to use.
        /// </summary>
        MagicSelection,
        /// <summary>
        /// Used for selecting an item.
        /// </summary>
        Inventory
    }

    /// <summary>
    /// Player menu during combat.
    /// </summary>
    public class CombatMenu
    {
        /// <summary>
        /// The current combat menu page.
        /// </summary>
        public CombatMenuPage Page { get; set; } = CombatMenuPage.None;

        /// <summary>
        /// Combat menu buttons.
        /// </summary>
        public Dictionary<CombatMenuPage, List<string>> Buttons { get; private set; } = new Dictionary<CombatMenuPage, List<string>>()
        {
            {
                CombatMenuPage.ActionSelection, new List<string>()
                {
                    "Attack", "Magic", "Inventory", "Run"
                }
            },

            {
                CombatMenuPage.MagicSelection, new List<string>()
                {
                    "Back"
                }
            },

            {
                CombatMenuPage.Inventory, new List<string>()
                {
                    "Back"
                }
            }
        };

        /// <summary>
        /// Button selected on the current page.
        /// </summary>
        public int SelectedButton { get; private set; }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        /// <param name="state">The combat state using this menu.</param>
        public void Update(CombatState state)
        {
            if(Page == CombatMenuPage.ActionSelection)
            {
                if(state.Input.KeyPressed(Keys.Left))
                    SelectedButton--;
                if(state.Input.KeyPressed(Keys.Right))
                    SelectedButton++;
            }

            if(Buttons.ContainsKey(Page))
            {
                if(SelectedButton >= Buttons[Page].Count)
                    SelectedButton = 0;
                if(SelectedButton < 0)
                    SelectedButton = Buttons[Page].Count - 1;
            }
        }

        /// <summary>
        /// Draws the menu text on top of the menu background.
        /// </summary>
        /// <param name="batch">The sprite batch to draw with.</param>
        /// <param name="menuRectangle">The rectangle the background was drawn in.</param>
        public void Draw(SpriteBatch batch, Rectangle menuRectangle, CombatState state)
        {
            if(Buttons.ContainsKey(Page))
            {
                int x = 20;
                int y = 34;
                Font font = state.Assets.Get<Font>("fonts/bitcell.ttf");

                batch.Text(font, 16, state.Combatant.Name, new Vector2(menuRectangle.X + 20, menuRectangle.Y + 14), Color.White);

                for(int i = 0; i < Buttons[Page].Count; i++)
                {
                    string s = Buttons[Page][i];
                    Vector2 measure = font.Measure(16, s);

                    if(menuRectangle.X + x + measure.X >= 480 - 12)
                    {
                        x = 10;
                        y += 16 + 2;
                    }

                    if(SelectedButton == i)
                        batch.Texture(new Vector2(menuRectangle.X + x - 8, menuRectangle.Y + y), state.Assets.Get<Texture2D>("ui/arrow.png"), Color.White);
                    batch.Text(font, 16, s, new Vector2(menuRectangle.X + x, menuRectangle.Y + y - 8), Color.White * (SelectedButton == i ? 0.8f : 0.6f));

                    x += (int)measure.X + 20;
                }

                string action = "";

                if(Page == CombatMenuPage.ActionSelection)
                {
                    switch(Buttons[Page][SelectedButton])
                    {
                        case "Attack":
                            action = "Attacks \"" + state.Enemy.Name + "\" with your equipped weapon";
                            break;

                        case "Magic":
                            action = "Choose a magic attack to use on \"" + state.Enemy.Name + "\"";
                            break;

                        case "Inventory":
                            action = "Choose an item to use on yourself";
                            break;

                        case "Run":
                            action = "Attempt to run away from combat";
                            break;
                    }

                    Rectangle hpRectangle = new Rectangle(480 - 90 - 30, menuRectangle.Y + menuRectangle.Height / 2 - (16 * 2 + 4) / 2, 90, 16);

                    batch.Rectangle(hpRectangle, Color.White * 0.1f);
                    hpRectangle.Width = (int)(90 * ((float)state.Combatant.Health / state.Combatant.MaxHealth));
                    batch.Rectangle(hpRectangle, Color.White * 0.3f);
                    hpRectangle.Width = 90;

                    Vector2 measure = font.Measure(16, state.Combatant.Name);
                    batch.Text(font, 16, state.Combatant.Name, new Vector2(hpRectangle.X + hpRectangle.Width / 2 - measure.X / 2, hpRectangle.Y - 4), Color.White);

                    hpRectangle.Y += 16 + 4;
                    batch.Rectangle(hpRectangle, Color.White * 0.1f);
                    hpRectangle.Width = (int)(90 * ((float)state.Enemy.Health / state.Enemy.MaxHealth));
                    batch.Rectangle(hpRectangle, Color.White * 0.3f);
                    hpRectangle.Width = 90;

                    measure = font.Measure(16, state.Enemy.Name);
                    batch.Text(font, 16, state.Enemy.Name, new Vector2(hpRectangle.X + hpRectangle.Width / 2 - measure.X / 2, hpRectangle.Y - 4), Color.White);
                }

                batch.Text(font, 16, action, new Vector2(menuRectangle.X + 20, menuRectangle.Y + y + 8), Color.White * (0.4f + (float)Math.Sin(state.Game.Time * 10) * 0.1f));
            }
        }
    }
}
