using Microsoft.Xna.Framework;
using TeamStor.RPG.Attributes;
using TeamStor.RPG.Editor;

namespace TeamStor.RPG
{
	public class TextBoxTile : Tile
	{
		private TextBoxEvents _events;

		public TextBoxTile(string id, string name, Point textureSlot) : base(id, MapLayer.Control, name, textureSlot, false, -1)
		{
			_events = new TextBoxEvents(this);
		}

		public override bool HasEditableAttributes
		{
			get { return true; }
		}

		public override TileAttributeEditor[] AttributeEditors(MapEditorState state, ref int currentY)
		{
			return new TileAttributeEditor[]
			{
                new TextAttributeEditor("speaker", state, 1, true, ref currentY),
                new TextAttributeEditor("value", state, 8, true, ref currentY),
                new BoolAttributeEditor("needs-user-interaction", state, true, ref currentY)
			};
		}
		
		public override TileEventBase Events { get { return _events; } }
	}
}