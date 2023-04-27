using System;
using System.Collections.Generic;

namespace Bimaru.Interface.Utility
{
    /// <summary>
    /// A point on a two-dimensional integer grid
    /// </summary>
    public readonly struct GridPoint
    {
        public GridPoint(int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }

        public int RowIndex { get; }

        public int ColumnIndex { get; }

        private static readonly Dictionary<Direction, Tuple<int, int>> indexOffsets =
            new Dictionary<Direction, Tuple<int, int>>()
            {
                { Direction.DOWN      , new Tuple<int, int>(-1, 0) },
                { Direction.LEFT      , new Tuple<int, int>( 0,-1) },
                { Direction.LEFT_DOWN , new Tuple<int, int>(-1,-1) },
                { Direction.LEFT_UP   , new Tuple<int, int>( 1,-1) },
                { Direction.RIGHT     , new Tuple<int, int>( 0, 1) },
                { Direction.RIGHT_DOWN, new Tuple<int, int>(-1, 1) },
                { Direction.RIGHT_UP  , new Tuple<int, int>( 1, 1) },
                { Direction.UP        , new Tuple<int, int>( 1, 0) }
            };

        /// <summary>
        /// Next integer grid point in the specified direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public GridPoint GetNextPoint(Direction direction)
        {
            var offsets = indexOffsets[direction];

            return new GridPoint(RowIndex + offsets.Item1, ColumnIndex + offsets.Item2);
        }

        /// <summary>
        /// Field boundary when going from this point in the given direction.
        /// </summary>
        public FieldBoundary GetBoundary(Direction direction)
        {
            return new FieldBoundary(this, direction);
        }
    }
}
