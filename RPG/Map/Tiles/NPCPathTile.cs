using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.RPG.Editor;
using TeamStor.RPG.Attributes;

namespace TeamStor.RPG
{
    public class NPCPathTile : Tile
    {
        public enum RepeatStyle
        {
            Mirror,
            Repeat
        };

        public NPCPathTile(string id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, textureSlot, solid, transitionPriority)
        {
        }

        public override bool HasEditableAttributes { get { return true; } }

        public override TileAttributeEditor[] AttributeEditors(MapEditorState state, ref int currentY)
        {
            return new TileAttributeEditor[]
            {
                new IntAttributeEditor("walk-speed", state, 1, 100, ref currentY),
                new EnumAttributeEditor<RepeatStyle>("repeat-style", state, ref currentY)
            };
        }
    }
}
