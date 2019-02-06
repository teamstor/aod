using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TeamStor.AOD.Gameplay.World;

namespace TeamStor.AOD
{
    public class TallGrassEvents : TileEventBase
    {
        private Map _latestMap;
        private Point _walkedOn = new Point(-1, -1);
        private long _walkedOnTick = 0;

        public TallGrassEvents(Tile tile) : base(tile)
        {
        }

        public override void OnInteract(TileMetadata metadata, WorldState world, Point mapPos)
        {
        }

        public override void OnStandingOn(TileMetadata metadata, WorldState world, Point mapPos, long tickCount)
        {
        }

        public override void OnWalkEnter(TileMetadata metadata, WorldState world, Point mapPos)
        {
            _latestMap = world.Map;
            _walkedOn = mapPos;
            _walkedOnTick = world.Game.TotalFixedUpdates;

            // if this map is a combat area we should just encounter enemies on every single tile
            if(!world.Map.Info.CombatArea)
                RandomEncounter.TryRandomEncounter(world);
        }

        public override void OnWalkLeave(TileMetadata metadata, WorldState world, Point mapPos)
        {
            if(_walkedOn == mapPos)
                _walkedOn = new Point(-1, -1);
        }

        public string CurrentTextureFor(Engine.Game game, Map map, Point mapPos)
        {
            if(map == _latestMap &&
                _walkedOn == mapPos)
            {
                long ticksSince = game.TotalFixedUpdates - _walkedOnTick;

                if(ticksSince > 8)
                    return "enemy_encounter_block_active.png";
                if(ticksSince > 4)
                    return "enemy_encounter_block_walkthrough2.png";

                return "enemy_encounter_block_walkthrough1.png";
            }

            return "enemy_encounter_block.png";
        }
    }
}
