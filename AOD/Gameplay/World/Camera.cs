using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD.Gameplay.World
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
                Vector2 position = new Vector2(World.Player.WorldPosition.X - 480 / 2, World.Player.WorldPosition.Y - 270 / 2);

                if(position.X < 0)
                    position.X = 0;
                if(position.Y < 0)
                    position.Y = 0;
                if(position.X > World.Map.Width * 16 - 480)
                    position.X = World.Map.Width * 16 - 480;
                if(position.Y > World.Map.Height * 16 - 270)
                    position.Y = World.Map.Height * 16 - 270;

                if(World.Map.Width * 16 < 480)
                    position.X = World.Map.Width * 16 / 2 - 480 / 2;
                if(World.Map.Height * 16 < 270)
                    position.Y = World.Map.Height * 16 / 2 - 270 / 2;

                return -position;
            }
        }

        public Camera(WorldState world)
        {
            World = world;

            Update(World.Game.DeltaTime);
        }

        public void Update(double deltaTime)
        {
            // TODO: do something here?
        }
    }
}
