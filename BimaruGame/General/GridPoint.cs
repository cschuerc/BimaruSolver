using System;
using System.Collections.Generic;
using Bimaru.General;

namespace Bimaru
{
    /// <summary>
    /// A point on a two-dimensional integer grid
    /// </summary>
    public struct GridPoint : IGridPoint
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

        /// <inheritdoc/>
        public int RowIndex
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public int ColumnIndex
        {
            get;
            private set;
        }

        private static readonly Dictionary<Directions, Tuple<int, int>> _indexOffsets =
            new Dictionary<Directions, Tuple<int, int>>()
            {
                {Directions.DOWN      , new Tuple<int, int>(-1, 0) },
                {Directions.LEFT      , new Tuple<int, int>( 0,-1) },
                {Directions.LEFT_DOWN , new Tuple<int, int>(-1,-1) },
                {Directions.LEFT_UP   , new Tuple<int, int>( 1,-1) },
                {Directions.RIGHT     , new Tuple<int, int>( 0, 1) },
                {Directions.RIGHT_DOWN, new Tuple<int, int>(-1, 1) },
                {Directions.RIGHT_UP  , new Tuple<int, int>( 1, 1) },
                {Directions.UP        , new Tuple<int, int>( 1, 0) }
            };

        /// <inheritdoc/>
        public IGridPoint GetNextPoint(Directions direction)
        {
            var offsets = _indexOffsets[direction];

            return new GridPoint(RowIndex + offsets.Item1, ColumnIndex + offsets.Item2);
        }

        /// <inheritdoc/>
        public IFieldBoundary GetBoundary(Directions direction)
        {
            return new FieldBoundary(this, direction);
        }
    }
}
