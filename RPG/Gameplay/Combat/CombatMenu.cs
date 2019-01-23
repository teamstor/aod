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

    public enum CombatPendingPlayerAction
    {
        None,

        /// <summary>
        /// Attempts to run away from the enemy.
        /// </summary>
        AttemptRunAway
    }

    /// <summary>
    /// Player menu during combat.
    /// </summary>
    public class CombatMenu
    {
        private CombatMenuPage _page;
        private string _messageTarget = "";
        private double _messageResetTime = -1;
        private double _untilNextChar = 0;

        /// <summary>
        /// Current message.
        /// </summary>
        public string Message
        {
            get; private set;
        } = "";

        /// <summary>
        /// The current combat menu page.
        /// </summary>
        public CombatMenuPage Page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value;
                SelectedButton = 0;
            }
        }

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
        /// Pending player action.
        /// </summary>
        public CombatPendingPlayerAction PendingAction
        {
            get; private set;
        }

        /// <summary>
        /// Called at the start of a new turn.
        /// </summary>
        public void NewTurn()
        {
            Page = CombatMenuPage.ActionSelection;
            PendingAction = CombatPendingPlayerAction.None;
        }

        /// <summary>
        /// Shows a message in the combat menu until the specified time.
        /// </summary>
        /// <param name="message">Message to show.</param>
        /// <param name="writeOut">If the message should be written out like a text box.</param>
        /// <param name="showUntil">When the message should stop showing.</param>
        /// <returns>The time the message will stop showing.</returns>
        public double ShowMessage(string message, bool writeOut, double showUntil)
        {
            _messageTarget = message;
            Message = writeOut ? "" : message;
            _messageResetTime = showUntil;
            _untilNextChar = 0.03;

            return showUntil;
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        /// <param name="state">The combat state using this menu.</param>
        public void Update(CombatState state)
        {
            if(Page == CombatMenuPage.ActionSelection)
            {
                if(InputMap.FindMapping(InputAction.Left).Pressed(state.Input))
                    SelectedButton--;
                if(InputMap.FindMapping(InputAction.Right).Pressed(state.Input))
                    SelectedButton++;
            }

            if(Buttons.ContainsKey(Page))
            {
                if(SelectedButton >= Buttons[Page].Count)
                    SelectedButton = 0;
                if(SelectedButton < 0)
                    SelectedButton = Buttons[Page].Count - 1;

                if(InputMap.FindMapping(InputAction.Action).Pressed(state.Input))
                {
                    switch(Page)
                    {
                        case CombatMenuPage.ActionSelection:
                            if(Buttons[Page][SelectedButton] == "Magic")
                                Page = CombatMenuPage.MagicSelection;
                            if(Buttons[Page][SelectedButton] == "Inventory")
                                Page = CombatMenuPage.Inventory;
                            if(Buttons[Page][SelectedButton] == "Run")
                            {
                                PendingAction = CombatPendingPlayerAction.AttemptRunAway;
                                Page = CombatMenuPage.None;
                            }
                            break;

                        case CombatMenuPage.MagicSelection:
                        case CombatMenuPage.Inventory:
                            if(Buttons[Page][SelectedButton] == "Back")
                                Page = CombatMenuPage.ActionSelection;
                            break;
                    }
                }
            }

            if(_messageResetTime != -1 && state.Game.Time > _messageResetTime)
            {
                Message = _messageTarget = "";
                _messageResetTime = -1;
            }

            _untilNextChar -= state.Game.DeltaTime;
            while(_untilNextChar <= 0)
            {
                if(Message.Length < _messageTarget.Length)
                    Message += _messageTarget[Message.Length];

                _untilNextChar += 0.03;
            }
        }

        /// <summary>
        /// Draws the menu text on top of the menu background.
        /// </summary>
        /// <param name="batch">The sprite batch to draw with.</param>
        /// <param name="menuRectangle">The rectangle the background was drawn in.</param>
        public void Draw(SpriteBatch batch, Rectangle menuRectangle, CombatState state)
        {
            Font font = state.Assets.Get<Font>("fonts/bitcell.ttf");

            if(Buttons.ContainsKey(Page))
            {
                int x = 20;
                int y = 34;

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
                    if(Buttons[Page][SelectedButton] == "Attack")
                        action = "Attacks \"" + state.Enemy.Name + "\" with your equipped weapon";
                    if(Buttons[Page][SelectedButton] == "Magic")
                         action = "Choose a magic attack to use on \"" + state.Enemy.Name + "\"";
                    if(Buttons[Page][SelectedButton] == "Inventory")
                        action = "Choose an item to use on yourself";
                    if(Buttons[Page][SelectedButton] == "Run")
                        action = "Attempt to run away from combat";

                    Rectangle hpRectangle = new Rectangle(480 - 90 - 30, menuRectangle.Y + menuRectangle.Height / 2 - (16 * 2 + 4) / 2 + 2, 90, 16);

                    batch.Rectangle(hpRectangle, Color.White * 0.1f);
                    hpRectangle.Width = (int)(90 * ((float)state.Combatant.Health / state.Combatant.MaxHealth));
                    batch.Rectangle(hpRectangle, Color.White * 0.3f);
                    hpRectangle.Width = 90;

                    batch.Text(font, 16, "HP", new Vector2(hpRectangle.X + 4, hpRectangle.Y - 4), Color.White);

                    Vector2 measure = font.Measure(16, state.Combatant.Health + "/" + state.Combatant.MaxHealth);
                    batch.Text(font, 16, state.Combatant.Health + "/" + state.Combatant.MaxHealth, new Vector2(hpRectangle.X + hpRectangle.Width - 4 - measure.X, hpRectangle.Y - 4), Color.White);

                    hpRectangle.Y += 16 + 4;
                    batch.Rectangle(hpRectangle, Color.White * 0.1f);
                    hpRectangle.Width = (int)(90 * ((float)state.Enemy.Health / state.Enemy.MaxHealth));
                    batch.Rectangle(hpRectangle, Color.White * 0.3f);
                    hpRectangle.Width = 90;

                    batch.Text(font, 16, "Enemy", new Vector2(hpRectangle.X + 4, hpRectangle.Y - 4), Color.White);

                    measure = font.Measure(16, state.Enemy.Health + "/" + state.Enemy.MaxHealth);
                    batch.Text(font, 16, state.Enemy.Health + "/" + state.Enemy.MaxHealth, new Vector2(hpRectangle.X + hpRectangle.Width - 4 - measure.X, hpRectangle.Y - 4), Color.White);
                }

                if(Page == CombatMenuPage.MagicSelection || Page == CombatMenuPage.Inventory)
                {
                    if(Page == CombatMenuPage.MagicSelection)
                    {
                        Rectangle mpRectangle = new Rectangle(480 - 90 - 30, menuRectangle.Y + menuRectangle.Height / 2 - 6, 90, 16);

                        batch.Rectangle(mpRectangle, Color.White * 0.1f);
                        mpRectangle.Width = (int)(90 * ((float)state.Combatant.Magicka / state.Combatant.MaxMagicka));
                        batch.Rectangle(mpRectangle, Color.White * 0.3f);
                        mpRectangle.Width = 90;

                        batch.Text(font, 16, "MP", new Vector2(mpRectangle.X + 4, mpRectangle.Y - 4), Color.White);

                        Vector2 measure = font.Measure(16, state.Combatant.Magicka + "/" + state.Combatant.MaxMagicka);
                        batch.Text(font, 16, state.Combatant.Magicka + "/" + state.Combatant.MaxMagicka, new Vector2(mpRectangle.X + mpRectangle.Width - 4 - measure.X, mpRectangle.Y - 4), Color.White);
                    }

                    if(Buttons[Page][SelectedButton] == "Back")
                        action = "Go back to main page";
                }

                batch.Text(font, 16, action, new Vector2(menuRectangle.X + 20, menuRectangle.Y + y + 8), Color.White * (0.4f + (float)Math.Sin(state.Game.Time * 10) * 0.1f));
            }

            batch.Text(font, 16, Message, new Vector2(menuRectangle.X + 20, menuRectangle.Y + 14), Color.White);
        }
    }
}
