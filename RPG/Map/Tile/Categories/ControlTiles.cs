using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.RPG.Attributes;
using TeamStor.RPG.Gameplay;

namespace TeamStor.RPG
{
    public static class ControlTiles
    {
        public static Tile Spawnpoint = new Tile("spawnpoint", Tile.MapLayer.Control, "Spawnpoint", "tiles/control/spawnpoint.png");

        public static Tile MapPortal = new Tile("map-portal", Tile.MapLayer.Control, "Map Portal", "tiles/control/map_portal.png").
            AttributeTileAttributeEditors((s, ref y) =>
            {
                return new TileAttributeEditor[]
                {
                    new TextAttributeEditor("map-file", s, 1, true, ref y),
                    new EnumAttributeEditor<Direction>("spawn-direction", s, ref y),
                    new TextAttributeEditor("custom-spawn-position", s, 1, true, ref y),
                    new BoolAttributeEditor("transition", s, true, ref y),
                    new BoolAttributeEditor("needs-user-interaction", s, true, ref y)
                };
            }).Events = new PortalEvents(MapPortal);
    }
}
