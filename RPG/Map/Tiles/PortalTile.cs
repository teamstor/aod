using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.RPG.Editor;
using TeamStor.RPG.Attributes;
using TeamStor.RPG.Gameplay;

namespace TeamStor.RPG
{
    public class PortalTile : Tile
    {
        private PortalEvents _events;

        public PortalTile(byte id, MapLayer layer, string name, Point textureSlot, bool solid = false, int transitionPriority = 1000) : base(id, layer, name, textureSlot, solid, transitionPriority)
        {
            _events = new PortalEvents(this);
        }

        public override bool HasEditableAttributes
        {
            get
            {
                return true;
            }
        }

        public override TileAttributeEditor[] AttributeEditors(MapEditorState state, ref int currentY)
        {
            return new TileAttributeEditor[]
            {
                new TextAttributeEditor("map-file", state, 1, true, ref currentY),
                new EnumAttributeEditor<Direction>("spawn-direction", state, ref currentY),
                new TextAttributeEditor("custom-spawn-position", state, 1, true, ref currentY),
                new BoolAttributeEditor("transition", state, true, ref currentY),
                new BoolAttributeEditor("needs-user-interaction", state, true, ref currentY)
            };
        }

        public override TileEventBase Events { get { return _events; } }
    }
}
