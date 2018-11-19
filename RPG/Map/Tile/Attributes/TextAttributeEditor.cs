using Microsoft.Xna.Framework;
using TeamStor.Engine.Tween;
using TeamStor.RPG.Editor;

namespace TeamStor.RPG.Attributes
{
	/// <summary>
	/// An attribute with an editable string.
	/// </summary>
	public class TextAttributeEditor : TileAttributeEditor
	{
		public TextField TextField { get; private set; }
		public ScrollableTextField ScrollableTextField { get; private set; }

		public override string ValueAsString
		{
			get
			{
				if(ScrollableTextField != null)
					return ScrollableTextField.Text;
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
		
		public TextAttributeEditor(string name, MapEditorState state, bool scrollable, bool monospace, ref int currentY) : base(name, state, ref currentY)
		{
			if(scrollable)
			{
				State.ScrollableTextFields.Add("editor-" + GetType().Name + "-" + Name, ScrollableTextField = new ScrollableTextField()
				{
					Label = name + ": ",
					Text = "",
					Area = new TweenedRectangle(state.Game, new Rectangle(state.Game.GraphicsDevice.Viewport.Bounds.Width / 2 - 400 / 2, currentY, 400, 200)),
					Font = monospace ? state.Game.DefaultFonts.MonoBold : state.Game.DefaultFonts.Bold,
				});
				
				currentY += ScrollableTextField.Area.Value.Height + 4;
			}
			else
			{
				State.TextFields.Add("editor-" + GetType().Name + "-" + Name, TextField = new TextField()
				{
					Label = name + ": ",
					Text = "",
					Position = new TweenedVector2(state.Game, new Vector2(state.Game.GraphicsDevice.Viewport.Bounds.Width / 2 - 400 / 2, currentY)),
					Font = monospace ? state.Game.DefaultFonts.MonoBold : state.Game.DefaultFonts.Bold,
					Width = 400,
					Lines = 1
				});

				currentY += TextField.Rectangle.Height + 4;
			}
		}

		public override void ValueFromExistingTile(string value)
		{
			if(ScrollableTextField != null)
				ScrollableTextField.Text = value;
			else
				TextField.Text = value;
		}

		public override void Dispose()
		{
			if(State.TextFields.ContainsKey("editor-" + GetType().Name + "-" + Name))
				State.TextFields.Remove("editor-" + GetType().Name + "-" + Name);
			if(State.ScrollableTextFields.ContainsKey("editor-" + GetType().Name + "-" + Name))
				State.ScrollableTextFields.Remove("editor-" + GetType().Name + "-" + Name);
		}
	}
}