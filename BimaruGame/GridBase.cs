using BimaruInterfaces;
using System;
using System.Collections.Generic;
using Utility;

namespace BimaruGame
{
    /// <summary>
    /// Implementation of a two-dimensional grid of fixed size
    /// </summary>
    /// <typeparam name="T"> The type of the field values </typeparam>
    public class GridBase<T>: ICloneable
    {
        /// <summary>
        /// Creates a grid with given number of rows and columns and all fields are initialized to the default value
        /// </summary>
        /// <param name="numRows"> Number of rows of the grid </param>
        /// <param name="numColumns"> Number of columns of the grid </param>
        /// <param name="defaultValue"> Default value for all fields </param>
        public GridBase(int numRows, int numColumns, T defaultValue = default(T))
        {
            NumRows = numRows;
            NumColumns = numColumns;

            _grid = AllocateGrid(numRows, numColumns, defaultValue);
        }

        #region Grid properties
        private T[,] _grid;

        /// <summary>
        /// Allocates a grid with fix dimensions and default value
        /// </summary>
        /// <param name="numRows"> Number of rows of the grid </param>
        /// <param name="numColumns"> Number of columns of the grid </param>
        /// <param name="defaultValue"> Default value for all grid fields </param>
        /// <returns> A new grid of given dimensions and filled with default values </returns>
        private T[,] AllocateGrid(int numRows, int numColumns, T defaultValue)
        {
            var grid = new T[numRows, numColumns];
            foreach (GridPoint p in AllPoints())
            {
                grid[p.RowIndex, p.ColumnIndex] = defaultValue;
            }

            return grid;
        }

        private int _numRows;

        /// <summary>
        /// Number of rows of the grid. Must be positive.
        /// </summary>
        public int NumRows
        {
            get => _numRows;

            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("The number of grid rows has to be > 0.");
                }

                _numRows = value;
            }
        }

        private int _numColumns;

        /// <summary>
        /// The number of columns of the grid. Must be positive.
        /// </summary>
        public int NumColumns
        {
            get => _numColumns;

            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("The number of grid columns has to be > 0.");
                }

                _numColumns = value;
            }
        }
        #endregion

        #region Get/set fields
        /// <summary>
        /// Whether the index is in the range.
        /// </summary>
        /// <param name="index"> Index </param>
        /// <param name="length"> Length of the indexer </param>
        /// <returns> True, if the index is valid </returns>
        protected bool IsIndexValid(int index, int length)
        {
            return index >= 0 && index < length;
        }

        /// <summary>
        /// Whether the point is inside the grid or not.
        /// </summary>
        /// <param name="point"> Point to check </param>
        /// <returns> True, if the point is in the grid </returns>
        protected bool IsPointInGrid(GridPoint point)
        {
            return IsIndexValid(point.RowIndex, NumRows) &&
                IsIndexValid(point.ColumnIndex, NumColumns);
        }

        /// <summary>
        /// Get the field value at the given point without checking whether the point is inside the grid or not.
        /// </summary>
        /// <param name="point"> Point whose value is returned. </param>
        /// <returns> Value of the field at the given grid point. </returns>
        protected T GetFieldValueNoCheck(GridPoint point)
        {
            return _grid[point.RowIndex, point.ColumnIndex];
        }

        /// <summary>
        /// Get the field value at the given point.
        /// </summary>
        /// <param name="point"> Point whose grid value is returned </param>
        /// <returns></returns>
        public virtual T GetFieldValue(GridPoint point)
        {
            if (!IsPointInGrid(point))
            {
                throw new ArgumentOutOfRangeException();
            }

            return GetFieldValueNoCheck(point);
        }

        /// <summary>
        /// Sets the field value at the given point.
        /// </summary>
        /// <param name="point"> Point whose field value is set. </param>
        /// <param name="value"> Value which the field value is set to. </param>
        public virtual void SetFieldValue(GridPoint point, T value)
        {
            if (!IsPointInGrid(point))
            {
                throw new InvalidFieldChange();
            }

            T origValue = GetFieldValueNoCheck(point);
            if (!origValue.Equals(value))
            {
                _grid[point.RowIndex, point.ColumnIndex] = value;

                var e = new FieldValueChangedEventArgs<T>(point, origValue);
                OnAfterFieldValueSet(e);
                OnFieldValueChanged(e);
            }
        }

        /// <summary>
        /// Is called after a field value is changed.
        /// </summary>
        /// <param name="e"> Event arguments of the field change. </param>
        protected virtual void OnAfterFieldValueSet(FieldValueChangedEventArgs<T> e)
        {

        }

        /// <summary>
        /// Event raised after a field value has changed.
        /// </summary>
        public event EventHandler<FieldValueChangedEventArgs<T>> FieldValueChanged;

        /// <summary>
        /// FieldValueChanged event-raising method
        /// </summary>
        /// <param name="e"> Event arguments </param>
        protected virtual void OnFieldValueChanged(FieldValueChangedEventArgs<T> e)
        {
            FieldValueChanged?.Invoke(this, e);
        }
        #endregion

        #region Point enumerables
        /// <summary>
        /// Enumerable over all points of the given row starting from the zero column.
        /// </summary>
        /// <param name="rowIndex"> Index of the row to enumerate over. </param>
        /// <returns></returns>
        public IEnumerable<GridPoint> PointsOfRow(int rowIndex)
        {
            GridPoint currentPoint = new GridPoint(rowIndex, 0);
            while (currentPoint.ColumnIndex < NumColumns)
            {
                yield return currentPoint;

                currentPoint.ColumnIndex++;
            }
        }

        /// <summary>
        /// Enumerable over all points of the given column starting from the zero row.
        /// </summary>
        /// <param name="columnIndex"> Index of the column to enumerate over. </param>
        /// <returns></returns>
        public IEnumerable<GridPoint> PointsOfColumn(int columnIndex)
        {
            GridPoint currentPoint = new GridPoint(0, columnIndex);
            while (currentPoint.RowIndex < NumRows)
            {
                yield return currentPoint;

                currentPoint.RowIndex++;
            }
        }

        /// <summary>
        /// Enumerable over all points of the grid. Arbitrary order.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GridPoint> AllPoints()
        {
            for (int rowIndex = 0; rowIndex < NumRows; rowIndex++)
            {
                foreach (GridPoint p in PointsOfRow(rowIndex))
                {
                    yield return p;
                }
            }
        }
        #endregion

        /// <summary>
        /// Clone the grid by a deep copy (except the T instances are shallowly copied)
        /// </summary>
        /// <returns> The clone of the grid </returns>
        public virtual object Clone()
        {
            GridBase<T> clonedGrid = (GridBase<T>)MemberwiseClone();

            clonedGrid.FieldValueChanged = null;
            clonedGrid._grid = (T[,])_grid.Clone();

            return clonedGrid;
        }
    }
}
