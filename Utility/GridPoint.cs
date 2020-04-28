using System;
using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// A point on a two-dimensional integer grid
    /// </summary>
    public struct GridPoint
    {
        /// <summary>
        /// Constructs a grid point
        /// </summary>
        /// <param name="rowIndex"> Row index of the point </param>
        /// <param name="columnIndex"> Column index of the point </param>
        public GridPoint(int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }

        /// <summary>
        /// Get the row index of the point
        /// </summary>
        /// <returns> Row index of the point </returns>
        public int RowIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Get the column index of the point
        /// </summary>
        /// <returns> Column index of the point </returns>
        public int ColumnIndex
        {
            get;
            set;
        }

        private static readonly Dictionary<Direction, Tuple<int, int>> _indexOffsets =
            new Dictionary<Direction, Tuple<int, int>>()
            {
                {Direction.DOWN      , new Tuple<int, int>(-1, 0) },
                {Direction.LEFT      , new Tuple<int, int>( 0,-1) },
                {Direction.LEFT_DOWN , new Tuple<int, int>(-1,-1) },
                {Direction.LEFT_UP   , new Tuple<int, int>( 1,-1) },
                {Direction.RIGHT     , new Tuple<int, int>( 0, 1) },
                {Direction.RIGHT_DOWN, new Tuple<int, int>(-1, 1) },
                {Direction.RIGHT_UP  , new Tuple<int, int>( 1, 1) },
                {Direction.UP        , new Tuple<int, int>( 1, 0) }
            };

        /// <summary>
        /// Get the next integer grid point in the specified direction
        /// </summary>
        /// <param name="direction"> Direction </param>
        /// <returns> The next point in the specified direction </returns>
        public GridPoint GetNextPoint(Direction direction)
        {
            var offsets = _indexOffsets[direction];

            return new GridPoint(RowIndex + offsets.Item1, ColumnIndex + offsets.Item2);
        }

        /// <summary>
        /// Get the boundary when going from the point in the direction.
        /// </summary>
        /// <param name="direction"> Direction to go </param>
        /// <returns> Field boundary from this point in the direction </returns>
        public FieldBoundary GetBoundary(Direction direction)
        {
            return new FieldBoundary(this, direction);
        }
    }
}
