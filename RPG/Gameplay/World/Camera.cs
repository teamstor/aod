using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.RPG.Gameplay.World
{
    /// <summary>
    /// Camera following the player in the world.
    /// TODO: kanske inte behövs.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// The world the camera is in.
        /// </summary>
        public WorldState World
        {
            get; private set;
        }

        /// <summary>
        /// Camera offset.
        /// </summary>
        public Vector2 Offset
        {
            get
            {
                Vector2 position = -new Vector2(World.Player.WorldPosition.X - 480 / 2, World.Player.WorldPosition.Y - 270 / 2);

                // TODO smooth???
                return position;
            }
        }

        public Camera(WorldState world)
        {
            World = world;
        }
    }
}
