using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine.Graphics;

namespace TeamStor.RPG.Gameplay.World
{
    /// <summary>
    /// Heading direction.
    /// </summary>
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    };

    public static class DirectionHelper
    {
        public static Point ToPoint(this Direction dir)
        {
            switch(dir)
            {
                case Direction.Left:
                    return new Point(-1, 0);

                case Direction.Right:
                    return new Point(1, 0);

                case Direction.Up:
                    return new Point(0, -1);

                case Direction.Down:
                    return new Point(0, 1);
            }

            return Point.Zero;
        }
    }

    /// <summary>
    /// An entity in the world with a position and hitbox.
    /// </summary>
    public abstract class PositionedEntity
    {
        protected Point _position;
        protected double _walkCompletionTime = 0;
        protected double _multiplyBy = 1;

        /// <summary>
        /// The world the entity is in.
        /// </summary>
        public WorldState World
        {
            get; protected set;
        }

        /// <summary>
        /// Current top-left pixel in the world.
        /// </summary>
        public virtual Vector2 WorldPosition
        {
            get
            {
                Vector2 vec = Vector2.Lerp(
                    (Position * new Point(16, 16)).ToVector2(), 
                    (NextPosition * new Point(16, 16)).ToVector2(), 
                    (float)WalkCompletion);
                vec.X = (int)vec.X;
                vec.Y = (int)vec.Y;

                return vec;
            }
        }

        /// <summary>
        /// Current position of the entity in tiles.
        /// </summary>
        public virtual Point Position
        {
            get
            {
                return World.Game.Time >= _walkCompletionTime ? NextPosition : _position;
            }
        }

        /// <summary>
        /// Amount the entity has walked from the current to the next position.
        /// </summary>
        public virtual double WalkCompletion
        {
            get
            {
                return 1.0f - MathHelper.Clamp((float)(_walkCompletionTime - World.Game.Time) * (float)_multiplyBy, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// Position the entity is walking to.
        /// Position = NextPosition if the entity is standing still.
        /// </summary>
        public virtual Point NextPosition
        {
            get; protected set;
        }

        /// <summary>
        /// If the entity is walking to another position.
        /// </summary>
        public bool IsWalking
        {
            get
            {
                return Position != NextPosition;
            }
        }

        /// <summary>
        /// Direction the entity is heading.
        /// </summary>
        public Direction Heading = Direction.Down;

        /// <summary>
        /// Speed in tiles/second the player walks.
        /// </summary>
        public virtual double Speed
        {
            get; protected set;
        } = 1;

        private void UpdateDirection()
        {
            Point distance = NextPosition - Position;

            if(distance.X <= -1)
                Heading = Direction.Left;

            if(distance.X >= 1)
                Heading = Direction.Right;

            if(distance.Y <= -1)
                Heading = Direction.Up;

            if(distance.Y >= 1)
                Heading = Direction.Down;
        }

        /// <summary>
        /// Moves this entity to a new position with walking speed.
        /// The entity will not be moved if it's already walking.
        /// </summary>
        /// <param name="position">The new position the entity should move to.</param>
        /// <returns>The time the move will complete.</returns>
        public virtual double MoveTo(Point position)
        {
            if(IsWalking || position == NextPosition)
                return _walkCompletionTime;

            _multiplyBy = Speed;
            _position = Position;
            NextPosition = position;
            _walkCompletionTime = World.Game.Time + (1.0 / Speed);

            UpdateDirection();

            return _walkCompletionTime;
        }

        /// <summary>
        /// Moves this entity to a new position with a custom speed.
        /// The entity will not be moved if it's already walking.
        /// </summary>
        /// <param name="position">The new position the entity should move to.</param>
        /// <param name="speed">The speed the entity should walk with.</param>
        /// <returns>The time the move will complete.</returns>
        public virtual double MoveWithSpeed(Point position, double speed)
        {
            if(IsWalking || position == NextPosition)
                return _walkCompletionTime;

            _multiplyBy = speed;
            _position = Position;
            NextPosition = position;
            _walkCompletionTime = World.Game.Time + (1.0 / speed);

            UpdateDirection();

            return _walkCompletionTime;
        }

        /// <summary>
        /// Moves this entity to a new position at a custom time.
        /// The entity will not be moved if it's already walking.
        /// </summary>
        /// <param name="position">The new position the entity should move to.</param>
        /// <param name="time">The time when the entity will arrive at the new position.</param>
        /// <returns>The time the move will complete.</returns>
        public virtual double MoveUntilTime(Point position, double time)
        {
            if(IsWalking || position == NextPosition)
                return _walkCompletionTime;

            _multiplyBy = time - World.Game.Time;
            _position = Position;
            NextPosition = position;
            _walkCompletionTime = time;

            UpdateDirection();

            return _walkCompletionTime;
        }

        /// <summary>
        /// Moves this entity to a new position instantly.
        /// This works even if the player is walking.
        /// </summary>
        /// <param name="position">The new position the entity should move to.</param>
        public virtual void MoveInstantly(Point position)
        {
            NextPosition = position;

            UpdateDirection();

            _position = position;
            _walkCompletionTime = World.Game.Time;
        }

        public PositionedEntity(WorldState world)
        {
            World = world;
        }
    }
}
