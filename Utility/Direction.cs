using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// Horizontal, vertical and diagonal directions.
    /// </summary>
    public enum Directions
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
    /// Extensions for directions
    /// </summary>
    public static class DirectionsExtensions
    {
        private static readonly Dictionary<Directions, Directions> _opposite =
            new Dictionary<Directions, Directions>()
                {
                    { Directions.DOWN, Directions.UP },
                    { Directions.LEFT, Directions.RIGHT },
                    { Directions.RIGHT, Directions.LEFT },
                    { Directions.UP, Directions.DOWN },
                    { Directions.LEFT_DOWN, Directions.RIGHT_UP },
                    { Directions.LEFT_UP, Directions.RIGHT_DOWN },
                    { Directions.RIGHT_DOWN, Directions.LEFT_UP },
                    { Directions.RIGHT_UP, Directions.LEFT_DOWN },
                };

        /// <summary>
        /// Get the opposite direction
        /// </summary>
        /// <param name="value"> Direction to take the opposite from </param>
        /// <returns> Opposite of the direction </returns>
        public static Directions GetOpposite(this Directions value)
        {
            return _opposite[value];
        }


        private static readonly HashSet<Directions> _isDiagonal =
            new HashSet<Directions>()
            {
                Directions.LEFT_DOWN,
                Directions.LEFT_UP,
                Directions.RIGHT_DOWN,
                Directions.RIGHT_UP
            };

        /// <summary>
        /// Whether the direction is diagonal or not.
        /// </summary>
        /// <param name="value"> Direction </param>
        /// <returns>True, if the direction is diagonal. </returns>
        public static bool IsDiagonal(this Directions value)
        {
            return _isDiagonal.Contains(value);
        }
    }
}
