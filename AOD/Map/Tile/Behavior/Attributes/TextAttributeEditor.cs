using Microsoft.Xna.Framework;
using TeamStor.Engine.Tween;
using TeamStor.AOD.Editor;

namespace TeamStor.AOD.Attributes
{
	/// <summary>
	/// An attribute with an editable string.
	/// </summary>
	public class TextAttributeEditor : TileAttributeEditor
	{
		public TextField TextField { get; private set; }

		public override string ValueAsString
		{
			get
			{
				return TextField.Text;
			}
		}
		
		public override bool IsDefaultValue
		{
			get
			{
				return ValueAsString == "";
			}
		}

		public TextAttributeEditor(string name, MapEditorState state, int maxLines, bool monospace, ref int currentY) : base(name, state, ref currentY)
		{			
			State.TextFields.Add("editor-" + GetType().Name + "-" + Name, TextField = new TextField()
			{
				Label = name + ": ",
				Text = "",
				Position = new TweenedVector2(state.Game, new Vector2(state.Game.GraphicsDevice.Viewport.Bounds.Width / 2 - 400 / 2, currentY)),
				Font = monospace ? state.Game.DefaultFonts.MonoBold : state.Game.DefaultFonts.Bold,
				Width = 400,
                Lines = maxLines
            });

			currentY += TextField.Rectangle.Height + 4;
		}

		public override void ValueFromExistingTile(string value)
		{
			TextField.Text = value;
		}

		public override void Dispose()
		{
			State.TextFields.Remove("editor-" + GetType().Name + "-" + Name);
		}
	}
}