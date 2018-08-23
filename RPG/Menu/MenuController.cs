using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using TeamStor.Engine;
using TeamStor.Engine.Graphics;

using Game = TeamStor.Engine.Game;

namespace TeamStor.RPG.Menu
{
    public class MenuController
    {
        private List<MenuButton> _buttons = new List<MenuButton>();

        public int SelectedIndex = 0;
        public MenuButton SelectedButton
        {
            get
            {
                return _buttons[SelectedIndex];
            }
        }

        public Game Game
        {
            get; private set;
        }

        public MenuController(Game game)
        {
            Game = game;
        }

        public void Add(MenuButton btn, int at = -1)
        {
            if(at < 0)
                _buttons.Add(btn);
            else
                _buttons.Insert(at, btn);
            
            btn.Controller = this;

            if(btn.Font == null)
                btn.Font = Game.Assets.Get<Font>("fonts/Alkhemikal.ttf");
            
            if(_buttons.Count == 1)
                _buttons[SelectedIndex].OnSelected(null);
        }

        public void Remove(MenuButton btn)
        {
            if(_buttons.Contains(btn))
            {
                _buttons.Remove(btn);

                if(SelectedIndex > _buttons.Count - 1)
                {
                    SelectedIndex = Math.Max(0, _buttons.Count - 1);

                    if(_buttons.Count > 0)
                        _buttons[SelectedIndex].OnSelected(btn);
                }
            }
        }

        public void Remove(int at)
        {
            _buttons.RemoveAt(at);
        }

        public bool HasButton(MenuButton btn)
        {
            return _buttons.Contains(btn);
        }

        public int IndexOf(MenuButton btn)
        {
            return _buttons.IndexOf(btn);
        }

        public void Update(double deltaTime, double totalTime, long count)
        {
            foreach(MenuButton button in _buttons)
                button.Update(this, deltaTime, totalTime, count);

            MenuButton currentBtn = _buttons.Count > 0 ? _buttons[SelectedIndex] : null;

            if(currentBtn != null)
            {
                if(Game.Input.KeyPressed(Keys.Up))
                {
                    SelectedIndex--;
                    
                    if(SelectedIndex < 0)
                        SelectedIndex = _buttons.Count - 1;
                    
                    currentBtn.OnDeselected(_buttons[SelectedIndex]);
                    _buttons[SelectedIndex].OnSelected(currentBtn);
                }
                
                if(Game.Input.KeyPressed(Keys.Down))
                {
                    SelectedIndex++;
                    
                    if(SelectedIndex > _buttons.Count - 1)
                        SelectedIndex = 0;
                    
                    currentBtn.OnDeselected(_buttons[SelectedIndex]);
                    _buttons[SelectedIndex].OnSelected(currentBtn);
                }
            }
        }

        public Vector2 Measure()
        {
            Vector2 measure = Vector2.Zero;

            foreach(MenuButton button in _buttons)
            {
                Vector2 measureBtn = button.Measure();

                if(measureBtn.X > measure.X)
                    measure.X = measureBtn.X;

                if(measure.Y != 0)
                    measure.Y += 2;
                measure.Y += measureBtn.Y;
            }

            return measure;
        }

        public void Draw(SpriteBatch batch, Vector2 pos, bool centerBtns)
        {
            Vector2 sizeOfMe = Measure();
            int y = (int)pos.Y;

            foreach(MenuButton button in _buttons)
            {
                Vector2 sizeOfBtn = button.Measure();
                int x = (int)pos.X;

                if(centerBtns)
                    x += (int)sizeOfMe.X / 2 - (int)sizeOfBtn.X / 2;

                button.Draw(batch, new Vector2(x, y));

                if(y != 0)
                    y += 2;
                y += (int)sizeOfBtn.Y;
            }
        }
    }
}
