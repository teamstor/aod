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
        private double _lastMoveTime;
        private Vector2 _offset;
        private Vector2 _lastPlayerPos;

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
                Vector2 position = _offset;

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
            _offset = new Vector2(World.Player.WorldPosition.X - 480 / 2, World.Player.WorldPosition.Y - 270 / 2);
            _lastMoveTime = World.Game.Time;
            _lastPlayerPos = World.Player.WorldPosition;

            Update(World.Game.DeltaTime);
        }

        public void Update(double deltaTime)
        {
            Vector2 oldOffset = _offset;

            if(_offset.X > World.Player.WorldPosition.X - 480 / 2 + 16 * 2)
                _offset.X = World.Player.WorldPosition.X - 480 / 2 + 16 * 2;
            if(_offset.X < World.Player.WorldPosition.X - 480 / 2 - 16 * 2)
                _offset.X = World.Player.WorldPosition.X - 480 / 2 - 16 * 2;

            if(_offset.Y > World.Player.WorldPosition.Y - 270 / 2 + 16 * 2)
                _offset.Y = World.Player.WorldPosition.Y - 270 / 2 + 16 * 2;
            if(_offset.Y < World.Player.WorldPosition.Y - 270 / 2 - 16 * 2)
                _offset.Y = World.Player.WorldPosition.Y - 270 / 2 - 16 * 2;

            if(_lastPlayerPos != World.Player.WorldPosition)
                _lastMoveTime = World.Game.Time;

            _lastPlayerPos = World.Player.WorldPosition;

            if(World.Game.Time - _lastMoveTime > 2)
            {
                _offset = Vector2.Lerp(_offset,
                    new Vector2(World.Player.WorldPosition.X - 480 / 2, World.Player.WorldPosition.Y - 270 / 2),
                    (float)deltaTime * 4f);
            }
        }
    }
}
