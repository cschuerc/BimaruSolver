using Bimaru.General;
using System;

namespace Bimaru
{
    /// <summary>
    /// Implementation of a two-dimensional grid of fixed size
    /// </summary>
    /// <typeparam name="T"> The type of the field values </typeparam>
    public class GridBase<T>: ICloneable
    {
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
            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < numColumns; columnIndex++)
                {
                    grid[rowIndex, columnIndex] = defaultValue;
                }
            }

            return grid;
        }

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
                    throw new System.ArgumentOutOfRangeException("The number of grid rows has to be > 0.");
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
                    throw new System.ArgumentOutOfRangeException("The number of grid columns has to be > 0.");
                }

                _numColumns = value;
            }
        }

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
        /// Whether the point is inside the grid.
        /// </summary>
        /// <param name="point"> Point to check </param>
        /// <returns> True, if the point is in the grid </returns>
        protected bool IsPointInGrid(IGridPoint point)
        {
            return IsIndexValid(point.RowIndex, NumRows) &&
                IsIndexValid(point.ColumnIndex, NumColumns);
        }

        /// <summary>
        /// Get the field value at the given point without checking whether the point is inside the grid or not.
        /// </summary>
        /// <param name="point"> Point inside grid whose value is returned </param>
        /// <returns> Value of the field at the given grid point </returns>
        protected T GetFieldValueNoCheck(IGridPoint point)
        {
            return _grid[point.RowIndex, point.ColumnIndex];
        }

        /// <summary>
        /// Get the field value at the given point
        /// </summary>
        /// <param name="point"> Point whose grid value is returned </param>
        /// <returns></returns>
        public virtual T GetFieldValue(IGridPoint point)
        {
            if (!IsPointInGrid(point))
            {
                throw new ArgumentOutOfRangeException();
            }

            return GetFieldValueNoCheck(point);
        }

        /// <summary>
        /// Is called before a field value is set.
        /// </summary>
        /// <param name="point"> Point whose field value will be set </param>
        protected virtual void OnBeforeFieldValueSet(IGridPoint point)
        {

        }

        /// <summary>
        /// Is called after a field value is set
        /// </summary>
        /// <param name="point"> Point whose field value was set </param>
        protected virtual void OnAfterFieldValueSet(IGridPoint point)
        {

        }

        /// <summary>
        /// Event raised after every field value change.
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

        /// <summary>
        /// Sets the field value at the given point
        /// </summary>
        /// <param name="point"> Point whose field value is set </param>
        /// <param name="value"> Value which the field value is set to </param>
        public void SetFieldValue(IGridPoint point, T value)
        {
            if (!IsPointInGrid(point))
            {
                throw new ArgumentOutOfRangeException();
            }

            T origValue = GetFieldValueNoCheck(point);
            if (!origValue.Equals(value))
            {
                OnBeforeFieldValueSet(point);

                _grid[point.RowIndex, point.ColumnIndex] = value;

                OnAfterFieldValueSet(point);

                OnFieldValueChanged(new FieldValueChangedEventArgs<T>(point, origValue));
            }
        }

        /// <summary>
        /// Clone the grid by a deep copy (except the T instances are shallowly copied)
        /// </summary>
        /// <returns> The clone of the grid </returns>
        public virtual object Clone()
        {
            GridBase<T> clonedGrid = (GridBase<T>)this.MemberwiseClone();

            clonedGrid._grid = (T[,])this._grid.Clone();

            return clonedGrid;
        }
    }
}
