using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.AOD.Gameplay.World;

namespace TeamStor.AOD.Gameplay
{
    /// <summary>
    /// Save file that stores player positions, NPC positions, events that advance the world, world time, etc.
    /// TODO: loading, events
    /// </summary>
    public class Save
    {
        /// <summary>
        /// Map file.
        /// </summary>
        public string Map
        {
            get; private set;
        }

        /// <summary>
        /// Position of the player in this save.
        /// </summary>
        public Tuple<Point, Direction> PlayerPosition
        {
            get; private set;
        }

        /// <summary>
        /// Positions of NPCs in this save.
        /// Tuple is origin tile + [current tile, heading]
        /// </summary>
        public List<Tuple<Point, Tuple<Point, Direction>>> NPCPositions
        {
            get; private set;
        } = new List<Tuple<Point, Tuple<Point, Direction>>>();

        /// <summary>
        /// All records in this save.
        /// Will be saved and loaded in chronological order.
        /// </summary>
        public Dictionary<DateTime, SaveRecord> Records
        {
            get; private set;
        } = new Dictionary<DateTime, SaveRecord>();

        /// <summary>
        /// Loads a save from a stream.
        /// </summary>
        public Save(Stream stream)
        {

        }

        /// <summary>
        /// Creates a save from the world the player is in.
        /// </summary>
        public Save(WorldState world)
        {
            Map = world.Map.File;
            PlayerPosition = new Tuple<Point, Direction>(world.Player.Position, world.Player.Heading);

            foreach(NPC npc in world.NPCs)
                NPCPositions.Add(new Tuple<Point, Tuple<Point, Direction>>(npc.OriginTile, new Tuple<Point, Direction>(npc.Position, npc.Heading)));
        }
    }
}
