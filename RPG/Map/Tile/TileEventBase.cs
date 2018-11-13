using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.RPG.Gameplay.World;

namespace TeamStor.RPG
{
    /// <summary>
    /// Events that can occur when interacting with a tile.
    /// This is divided up into a different class for organization.
    /// </summary>
    public abstract class TileEventBase
    {
        /// <summary>
        /// Tile that has these events.
        /// </summary>
        public Tile Tile
        {
            get; private set;
        }

        public TileEventBase(Tile tile)
        {
            Tile = tile;
        }

        /// <summary>
        /// Called once when the player walks onto this tile.
        /// </summary>
        /// <param name="metadata">Metadata for this tile.</param>
        /// <param name="world">The world this tile is in.</param>
        /// <param name="mapPos">The position on the map of this tile.</param>
        public abstract void OnWalkEnter(TileMetadata metadata, WorldState world, Point mapPos);

        /// <summary>
        /// Called every fixed tick that the player stands on this tile.
        /// </summary>
        /// <param name="metadata">Metadata for this tile.</param>
        /// <param name="world">The world this tile is in.</param>
        /// <param name="mapPos">The position on the map of this tile.</param>
        /// <param name="tickCount">The current tick.</param>
        public abstract void OnStandingOn(TileMetadata metadata, WorldState world, Point mapPos, long tickCount);

        /// <summary>
        /// Called once when the player walks off this tile.
        /// </summary>
        /// <param name="metadata">Metadata for this tile.</param>
        /// <param name="world">The world this tile is in.</param>
        /// <param name="mapPos">The position on the map of this tile.</param>
        public abstract void OnWalkLeave(TileMetadata metadata, WorldState world, Point mapPos);

        /// <summary>
        /// Called when the player is facing this tile and presses the interact button.
        /// </summary>
        /// <param name="metadata">Metadata for this tile.</param>
        /// <param name="world">The world this tile is in.</param>
        /// <param name="mapPos">The position on the map of this tile.</param>
        public abstract void OnInteract(TileMetadata metadata, WorldState world, Point mapPos);
    }
}
