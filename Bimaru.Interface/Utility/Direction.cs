namespace Bimaru.Interface.Utility
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
        private static readonly Dictionary<Direction, Direction> opposite =
            new()
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

        /// <summary>
        /// Arbitrary order.
        /// </summary>
        public static IEnumerable<Direction> GetAllDirections()
        {
            return Enum.GetValues(typeof(Direction)).Cast<Direction>();
        }

        /// <summary>
        /// Arbitrary order.
        /// </summary>
        public static IEnumerable<Direction> GetDirections(DirectionType type)
        {
            return directionsOfType[type];
        }

        private static readonly Dictionary<DirectionType, List<Direction>> directionsOfType =
            new()
            {
                {
                    DirectionType.ROW,
                    new List<Direction>()
                    {
                        Direction.LEFT,
                        Direction.RIGHT
                    }
                },
                {
                    DirectionType.COLUMN,
                    new List<Direction>()
                    {
                        Direction.UP,
                        Direction.DOWN
                    }
                },
                {
                    DirectionType.DIAGONAL,
                    new List<Direction>()
                    {
                        Direction.LEFT_DOWN,
                        Direction.LEFT_UP,
                        Direction.RIGHT_DOWN,
                        Direction.RIGHT_UP
                    }
                }
            };

        /// <summary>
        /// Arbitrary order.
        /// </summary>
        public static IEnumerable<Direction> GetNonDiagonalDirections()
        {
            yield return Direction.LEFT;
            yield return Direction.RIGHT;

            yield return Direction.UP;
            yield return Direction.DOWN;
        }

        public static DirectionType GetDirectionType(this Direction direction)
        {
            return directionType[direction];
        }

        private static readonly Dictionary<Direction, DirectionType> directionType =
            new()
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
    }
}
