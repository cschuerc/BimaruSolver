using System;
using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// Horizontal, vertical and diagonal directions.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Left
        /// </summary>
        LEFT,

        /// <summary>
        /// Right
        /// </summary>
        RIGHT,

        /// <summary>
        /// Up
        /// </summary>
        UP,

        /// <summary>
        /// Down
        /// </summary>
        DOWN,

        /// <summary>
        /// Diagonally left-up
        /// </summary>
        LEFT_UP,

        /// <summary>
        /// Diagonally left-down
        /// </summary>
        LEFT_DOWN,

        /// <summary>
        /// Diagonally right-up
        /// </summary>
        RIGHT_UP,

        /// <summary>
        /// Diagonally right-down
        /// </summary>
        RIGHT_DOWN
    }

    /// <summary>
    /// Direction types
    /// </summary>
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

        /// <summary>
        /// Diagonal direction
        /// </summary>
        DIAGONAL
    }

    /// <summary>
    /// Extensions for directions
    /// </summary>
    public static class DirectionExtensions
    {
        /// <summary>
        /// Get all directions in arbitrary order.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Direction> AllDirections()
        {
            return (IEnumerable<Direction>)Enum.GetValues(typeof(Direction));

        }

        private static readonly Dictionary<Direction, Direction> _opposite =
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

        /// <summary>
        /// Get the opposite direction
        /// </summary>
        /// <param name="direction"> Direction to take the opposite from </param>
        /// <returns> Opposite of the direction </returns>
        public static Direction GetOpposite(this Direction direction)
        {
            return _opposite[direction];
        }


        private static readonly Dictionary<Direction, DirectionType> _directionType =
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

        /// <summary>
        /// Type of the direction
        /// </summary>
        /// <param name="direction"> Direction </param>
        /// <returns> Direction type </returns>
        public static DirectionType GetDirectionType(this Direction direction)
        {
            return _directionType[direction];
        }
    }
}
