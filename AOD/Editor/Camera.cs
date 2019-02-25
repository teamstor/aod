using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TeamStor.Engine;
using TeamStor.Engine.Tween;

namespace TeamStor.AOD.Editor
{
	/// <summary>
	/// Map editor camera.
	/// </summary>
	public class Camera
	{
		private MapEditorState _state;
        private Vector2 _prevTotalSize;
        private Vector2 _prevMapSize;

        /// <summary>
        /// Total size of the map on screen.
        /// </summary>
        public Vector2 TotalSizeOnScreen
        {
            get
            {
                return new Vector2(_state.Map.Width * 16 * Zoom, _state.Map.Height * 16 * Zoom);
            }
        }
		
		/// <summary>
		/// Current zoom.
		/// </summary>
		public TweenedDouble Zoom;

		/// <summary>
		/// Current translation.
		/// </summary>
		public Vector2 Translation;
				
		/// <summary>
		/// Current transform.
		/// </summary>
		public Matrix Transform
		{
			get
			{
				return Matrix.CreateScale(Zoom) * Matrix.CreateTranslation(Translation.X, Translation.Y, 0);
			}
		}

		public Camera(MapEditorState state)
		{
			_state = state;
			Zoom = new TweenedDouble(state.Game, 2);
            _prevTotalSize = TotalSizeOnScreen;
            _prevMapSize = new Vector2(state.Map.Width, state.Map.Height);
        }

		public void Update(double deltaTime, double totalTime)
		{
			if(!_state.CurrentState.PauseEditor)
			{
				if(_state.Input.KeyPressed(Keys.D2))
				{
					if(Zoom.TargetValue <= 4)
						Zoom.TweenTo(Zoom.TargetValue * 2, TweenEaseType.EaseInOutSine, 0.15);
				}

				if(_state.Input.KeyPressed(Keys.D1))
				{
					if(Zoom.TargetValue >= 2)
						Zoom.TweenTo(Zoom.TargetValue / 2, TweenEaseType.EaseInOutSine, 0.15);
				}

                if(_state.Input.Mouse(MouseButton.Right))
                    Translation += _state.Input.MouseDelta;
            }

            Vector2 mapSize = new Vector2(_state.Map.Width, _state.Map.Height);

            if(mapSize != _prevMapSize)
                Translation -= (TotalSizeOnScreen - _prevTotalSize) / 2;
            else
            {
                // vet INTE vad som händer här men det funkar
                Vector2 mouseThing = ((_state.Input.MousePosition - Translation) / _prevTotalSize);
                Translation -= (TotalSizeOnScreen - _prevTotalSize) * mouseThing;
            }

            _prevTotalSize = TotalSizeOnScreen;
            _prevMapSize = mapSize;

            if(Translation.X > 200)
                Translation.X = 200;
            if(Translation.Y > 200)
                Translation.Y = 200;
            if(Translation.X < -(_state.Map.Width * (16 * Zoom) - _state.Game.GraphicsDevice.Viewport.Bounds.Width) - 200)
                Translation.X = -(_state.Map.Width * (16 * Zoom) - _state.Game.GraphicsDevice.Viewport.Bounds.Width) - 200;
            if(Translation.Y < -(_state.Map.Height * (16 * Zoom) - _state.Game.GraphicsDevice.Viewport.Bounds.Height) - 200)
                Translation.Y = -(_state.Map.Height * (16 * Zoom) - _state.Game.GraphicsDevice.Viewport.Bounds.Height) - 200;

            if(_state.Game.GraphicsDevice.Viewport.Bounds.Width / 2 > _state.Map.Width * 8 * Zoom)
				Translation.X = _state.Game.GraphicsDevice.Viewport.Bounds.Width / 2 - _state.Map.Width * 8 * Zoom;
			if(_state.Game.GraphicsDevice.Viewport.Bounds.Height / 2 > _state.Map.Height * 8 * Zoom)
				Translation.Y = _state.Game.GraphicsDevice.Viewport.Bounds.Height / 2 - _state.Map.Height * 8 * Zoom;
		}
	}
}