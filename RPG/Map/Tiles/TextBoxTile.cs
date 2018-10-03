using Microsoft.Xna.Framework;
using TeamStor.RPG.Attributes;
using TeamStor.RPG.Editor;

namespace TeamStor.RPG
{
	public class TextBoxTile : Tile
	{
		private bool _isTriggeredAutomatically;
		
		public TextBoxTile(byte id, string name, Point textureSlot, bool autoTrigger) : base(id, MapLayer.Control, name, textureSlot, false, -1)
		{
			_isTriggeredAutomatically = autoTrigger;
		}

		public override bool HasEditableAttributes
		{
			get { return true; }
		}

		public override TileAttributeEditor[] AttributeEditors(MapEditorState state, ref int currentY)
		{
			return new TileAttributeEditor[]
			{
				new TextAttributeEditor("value", state, 8, true, ref currentY)
			};
		}
	}
}