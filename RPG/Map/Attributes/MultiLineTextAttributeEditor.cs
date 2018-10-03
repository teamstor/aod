using Microsoft.Xna.Framework;
using TeamStor.Engine.Tween;
using TeamStor.RPG.Editor;

namespace TeamStor.RPG.Attributes
{
	/// <summary>
	/// A text attribute that can have multiple lines.
	/// </summary>
	public class MultiLineTextAttributeEditor : TileAttributeEditor
	{
		public TextField TextField { get; private set; }
		public int MaxLines { get; private set; }

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

		public MultiLineTextAttributeEditor(string name, MapEditorState state, int maxLines, ref int currentY) : base(name, state, ref currentY)
		{
			MaxLines = maxLines;
			
			State.TextFields.Add("editor-" + GetType().Name + "-" + Name, TextField = new TextField()
			{
				Label = name + ": ",
				Text = "",
				Position = new TweenedVector2(state.Game, new Vector2(state.Game.GraphicsDevice.Viewport.Bounds.Width / 2 - 400 / 2, currentY)),
				Font = state.Game.DefaultFonts.Bold,
				Width = 400,
				
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