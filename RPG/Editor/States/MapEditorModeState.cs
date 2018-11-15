using Microsoft.Xna.Framework;
using TeamStor.Engine;

namespace TeamStor.RPG.Editor.States
{
	/// <summary>
	/// Map editor mode.
	/// </summary>
	public abstract class MapEditorModeState : GameState
	{
		/// <summary>
		/// If the editor (camera, etc) should be paused.
		/// </summary>
		public abstract bool PauseEditor { get; }

        public virtual string CurrentHelpText
        {
            get
            {
                return "";
            }
        }


        /// <summary>
        /// Base map editor state.
        /// </summary>
        public MapEditorState BaseState;

        public virtual bool IsPointObscured(Vector2 point)
        {
            return false;
        }

    }
}