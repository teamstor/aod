using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.RPG.Gameplay.World;
using TeamStor.RPG.Gameplay;
using System.IO;

namespace TeamStor.RPG
{
    public class PortalEvents : TileEventBase
    {
        public PortalEvents(Tile tile) : base(tile)
        {
        }

        private void DoTeleport(TileMetadata metadata, WorldState world, Point mapPos)
        {
            if(!metadata.IsKeySet("map-file"))
                return;

            WorldState.SpawnArgs args = new WorldState.SpawnArgs(new Point(-1, -1), Gameplay.Direction.Down);

            if(metadata.IsKeySet("custom-spawn-position"))
            {
                string[] vec = metadata["custom-spawn-position"].Split(',');

                for(int i = 0; i < vec.Length; i++)
                    vec[i] = vec[i].Trim();

                int x = int.Parse(vec[0]);
                int y = int.Parse(vec[1]);

                args.Position = new Point(x, y);
            }

            if(metadata.IsKeySet("spawn-direction"))
                Enum.TryParse<Direction>(metadata["spawn-direction"], out args.Direction);

            world.TransitionToMap("data/maps/" + metadata["map-file"], 
                metadata["transition"] != "False", 
                args);
        }

        public override void OnInteract(TileMetadata metadata, WorldState world, Point mapPos)
        {
            if(metadata == null)
                return;
            if(!metadata.IsKeySet("needs-user-interaction") || metadata["needs-user-interaction"] == "True")
                DoTeleport(metadata, world, mapPos);
        }

        public override void OnWalkEnter(TileMetadata metadata, WorldState world, Point mapPos)
        {
            if(metadata == null)
                return;
            if(metadata["needs-user-interaction"] == "False")
                DoTeleport(metadata, world, mapPos);
        }
        
        public override void OnStandingOn(TileMetadata metadata, WorldState world, Point mapPos, long tickCount)
        {
        }

        public override void OnWalkLeave(TileMetadata metadata, WorldState world, Point mapPos)
        {
        }
    }
}
