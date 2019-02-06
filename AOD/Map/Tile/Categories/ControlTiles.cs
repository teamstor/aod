using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.AOD.Attributes;
using TeamStor.AOD.Editor;
using TeamStor.AOD.Gameplay;

namespace TeamStor.AOD
{
    public static class ControlTiles
    {
        public static Tile Spawnpoint = new Tile("control/spawnpoint", Tile.MapLayer.Control, "Spawnpoint", "tiles/control/spawnpoint.png");

        public static Tile MapPortal = new Tile("control/map-portal", Tile.MapLayer.Control, "Map Portal", "tiles/control/map_portal.png").
            AttributeEditableAttributes((MapEditorState s, ref int y) =>
            {
                return new TileAttributeEditor[]
                {
                    new TextAttributeEditor("map-file", s, 1, true, ref y),
                    new EnumAttributeEditor<Direction>("spawn-direction", s, ref y),
                    new TextAttributeEditor("custom-spawn-position", s, 1, true, ref y),
                    new BoolAttributeEditor("transition", s, true, ref y),
                    new BoolAttributeEditor("needs-user-interaction", s, true, ref y)
                };
            });

        public static Tile TextBox = new Tile("control/text-box", Tile.MapLayer.Control, "Textbox", "tiles/control/textbox.png").
            AttributeEditableAttributes((MapEditorState s, ref int y) =>
            {
                return new TileAttributeEditor[]
                {
                    new TextAttributeEditor("speaker", s, 1, true, ref y),
                    new TextAttributeEditor("value", s, true, true, ref y),
                    new BoolAttributeEditor("needs-user-interaction", s, true, ref y)
                };
            });

        public static Tile Barrier = new Tile("control/barrier", Tile.MapLayer.Control, "Barrier", "tiles/control/barrier.png", true);
        public static Tile InvertedBarrier = new Tile("control/!barrier", Tile.MapLayer.Control, "Inverted Barrier", "tiles/control/walkthrough.png", false);

        static ControlTiles()
        {
            MapPortal.Events = new PortalEvents(MapPortal);
            TextBox.Events = new TextBoxEvents(TextBox);
        }
    }
}
