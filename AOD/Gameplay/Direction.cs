using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD.Gameplay
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
}
