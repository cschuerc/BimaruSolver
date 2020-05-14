using System;
using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// Horizontal, vertical and diagonal directions.
    /// </summary>
    public enum Direction
    {
        LEFT,

        RIGHT,

        UP,

        DOWN,

        LEFT_UP,

        LEFT_DOWN,

        RIGHT_UP,

        RIGHT_DOWN
    }

    public enum DirectionType
    {
        /// <summary>
        /// Row (horizontal) direction
        /// </summary>
        ROW,

        /// <summary>
        /// Column (vertical) direction
        /// </summary>
        COLUMN,

        DIAGONAL
    }

    public static class Directions
    {
        /// <summary>
        /// Arbitrary order.
        /// </summary>
        public static IEnumerable<Direction> AllDirections()
        {
            return (IEnumerable<Direction>)Enum.GetValues(typeof(Direction));
        }

        /// <summary>
        /// Arbitrary order.
        /// </summary>
        public static IEnumerable<Direction> AllDiagonalDirections()
        {
            yield return Direction.LEFT_UP;
            yield return Direction.RIGHT_UP;
            yield return Direction.RIGHT_DOWN;
            yield return Direction.LEFT_DOWN;
        }

        /// <summary>
        /// Arbitrary order.
        /// </summary>
        public static IEnumerable<Direction> AllNonDiagonalDirections()
        {
            yield return Direction.LEFT;
            yield return Direction.UP;
            yield return Direction.RIGHT;
            yield return Direction.DOWN;
        }

        /// <summary>
        /// Arbitrary order.
        /// </summary>
        public static IEnumerable<Direction> AllHorizontalDirections()
        {
            yield return Direction.LEFT;
            yield return Direction.RIGHT;
        }

        /// <summary>
        /// Arbitrary order.
        /// </summary>
        public static IEnumerable<Direction> AllVerticalDirections()
        {
            yield return Direction.DOWN;
            yield return Direction.UP;
        }

        private static readonly Dictionary<Direction, Direction> opposite =
            new Dictionary<Direction, Direction>()
            {
                { Direction.DOWN, Direction.UP },
                { Direction.LEFT, Direction.RIGHT },
                { Direction.RIGHT, Direction.LEFT },
                { Direction.UP, Direction.DOWN },
                { Direction.LEFT_DOWN, Direction.RIGHT_UP },
                { Direction.LEFT_UP, Direction.RIGHT_DOWN },
                { Direction.RIGHT_DOWN, Direction.LEFT_UP },
                { Direction.RIGHT_UP, Direction.LEFT_DOWN },
            };

        public static Direction GetOpposite(this Direction direction)
        {
            return opposite[direction];
        }


        private static readonly Dictionary<Direction, DirectionType> directionType =
            new Dictionary<Direction, DirectionType>()
            {
                { Direction.DOWN, DirectionType.COLUMN },
                { Direction.LEFT, DirectionType.ROW },
                { Direction.RIGHT, DirectionType.ROW },
                { Direction.UP, DirectionType.COLUMN },
                { Direction.LEFT_DOWN, DirectionType.DIAGONAL },
                { Direction.LEFT_UP, DirectionType.DIAGONAL },
                { Direction.RIGHT_DOWN, DirectionType.DIAGONAL },
                { Direction.RIGHT_UP, DirectionType.DIAGONAL },
            };

        public static DirectionType GetDirectionType(this Direction direction)
        {
            return directionType[direction];
        }
    }
}
