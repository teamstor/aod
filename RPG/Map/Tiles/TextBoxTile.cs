using Microsoft.Xna.Framework;
using TeamStor.RPG.Attributes;
using TeamStor.RPG.Editor;

namespace TeamStor.RPG
{
	public class TextBoxTile : Tile
	{
		
		public TextBoxTile(byte id, string name, Point textureSlot) : base(id, MapLayer.Control, name, textureSlot, false, -1)
		{
		}

		public override bool HasEditableAttributes
		{
			get { return true; }
		}

		public override TileAttributeEditor[] AttributeEditors(MapEditorState state, ref int currentY)
		{
			return new TileAttributeEditor[]
			{
				new TextAttributeEditor("value", state, 8, true, ref currentY),
                new BoolAttributeEditor("needs-user-interaction", state, true, ref currentY)
			};
		}
	}
}