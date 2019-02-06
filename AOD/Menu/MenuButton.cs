using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using TeamStor.Engine.Graphics;
using TeamStor.Engine.Tween;
using TeamStor.Engine.Coroutine;
using SpriteBatch = TeamStor.Engine.Graphics.SpriteBatch;

namespace TeamStor.AOD.Menu
{
    /// <summary>
    /// Basic menu button.
    /// </summary>
    public class MenuButton
    {
        private double _selectTime;
        private bool _blink;
        private IEnumerator<ICoroutineOperation> _coroutine;
        
        /// <summary>
        /// Button label.
        /// </summary>
        public string Label;

        /// <summary>
        /// Label font.
        /// </summary>
        public Font Font;

        /// <summary>
        /// Menu controller this menu button belongs to.
        /// </summary>
        public MenuController Controller;

        public delegate void OnClickedDelegate(MenuButton button);

        /// <summary>
        /// Function to run when the user clicks on the button.
        /// </summary>
        public OnClickedDelegate Clicked;

        public delegate void OnLeftButtonDelegate(MenuButton button);

        /// <summary>
        /// Function to run when the "left" <- button is clicked.
        /// </summary>
        public OnLeftButtonDelegate LeftButton;

        public delegate void OnRightButtonDelegate(MenuButton button);

        /// <summary>
        /// Function to run when the "right" -> button is clicked.
        /// </summary>
        public OnRightButtonDelegate RightButton;

        /// <summary>
        /// If this button is selected/focused.
        /// </summary>
        public bool IsSelected
        {
            get; protected set;
        }

        /// <summary>
        /// If the button is in the click animation.
        /// </summary>
        public bool IsInClickAnimation
        {
            get
            {
                return _coroutine != null;
            }
        }


        /// <summary>
        /// Size of the button.
        /// </summary>
        public virtual Vector2 Size
        {
            get
            {
                return Controller.Game.Assets.Get<Texture2D>("ui/button/normal.png").Bounds.Size.ToVector2();
            }
        }

        public MenuButton(string text, Font font = null)
        {
            Label = text;
            Font = font;
        }

        /// <summary>
        /// Called when the button is selected/focused.
        /// </summary>
        /// <param name="previous">The previously selected button. Can be null.</param>
        public virtual void OnSelected(MenuButton previous)
        {
            _selectTime = Controller.Game.Time;
            IsSelected = true;
        }

        /// <summary>
        /// Called when the button is deselected/out of focus.
        /// </summary>
        /// <param name="next">The next selection.</param>
        public virtual void OnDeselected(MenuButton next)
        {
            IsSelected = false;
        }

        private IEnumerator<ICoroutineOperation> ClickAnimation()
        {
            for(int i = 0; i < 2; i++)
            {
                _blink = true;
                yield return Wait.Seconds(Controller.Game, 0.05);
                _blink = false;
                yield return Wait.Seconds(Controller.Game, 0.05);
            }

            if(Clicked != null)
                Clicked(this);

            _coroutine = null;
        }

        /// <summary>
        /// Called when the button is clicked.
        /// </summary>
        public virtual void OnClicked()
        {
            _coroutine = ClickAnimation();
            Controller.Game.CurrentState.Coroutine.AddExisting(_coroutine);
        }

        /// <summary>
        /// Called when the "left" <- button is clicked.
        /// </summary>
        public virtual void OnLeftButton()
        {
            if(LeftButton != null)
                LeftButton(this);
        }

        /// <summary>
        /// Called when the "right" -> button is clicked.
        /// </summary>
        public virtual void OnRightButton()
        {
            if(RightButton != null)
                RightButton(this);
        }

        public virtual void Update(MenuController controller, double deltaTime, double totalTime, long count)
        {
        }

        public virtual void Draw(SpriteBatch batch, Vector2 pos)
        {
            Texture2D texture = Controller.Game.Assets.Get<Texture2D>("ui/button/normal.png");
            if(IsSelected)
                texture = Controller.Game.Assets.Get<Texture2D>("ui/button/hover.png");
            if(_blink)
                texture = Controller.Game.Assets.Get<Texture2D>("ui/button/blink.png");

            batch.Texture(pos, texture, Color.White);

            Vector2 measure = Font.Measure(16, Label);
            batch.Text(Font, 16, Label, pos + Size / 2 - measure / 2 - new Vector2(0, 2), 
                _blink ? Color.White : 
                IsSelected ? new Color(233, 188, 255) : 
                new Color(59, 54, 54));
        }
    }
}
