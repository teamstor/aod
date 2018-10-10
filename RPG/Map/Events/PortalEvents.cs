using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.RPG.Gameplay.World;

namespace TeamStor.RPG
{
    public class PortalEvents : TileEventBase
    {
        public PortalEvents(PortalTile tile) : base(tile)
        {
        }

        private void DoTeleport(SortedDictionary<string, string> metadata, WorldState world, Point mapPos)
        {
            throw new Exception("AAAAAAA");
        }

        public override void OnInteract(SortedDictionary<string, string> metadata, WorldState world, Point mapPos)
        {
            if(metadata == null || !metadata.ContainsKey("needs-user-interaction") || metadata["needs-user-interaction"] == "True")
                DoTeleport(metadata, world, mapPos);
        }

        public override void OnWalkEnter(SortedDictionary<string, string> metadata, WorldState world, Point mapPos)
        {
            if(metadata != null && metadata.ContainsKey("needs-user-interaction") && metadata["needs-user-interaction"] == "False")
                DoTeleport(metadata, world, mapPos);
        }
        
        public override void OnStandingOn(SortedDictionary<string, string> metadata, WorldState world, Point mapPos, long tickCount)
        {
        }

        public override void OnWalkLeave(SortedDictionary<string, string> metadata, WorldState world, Point mapPos)
        {
        }
    }
}
