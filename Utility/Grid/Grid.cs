using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    /// <summary>
    /// Implementation of a two-dimensional grid of fixed size
    /// </summary>
    /// <typeparam name="T"> Type of the field values </typeparam>
    [Serializable]
    public class Grid<T>
    {
        public Grid(int numberOfRows, int numberOfColumns, T defaultFieldValue = default)
        {
            NumberOfRows = numberOfRows;
            NumberOfColumns = numberOfColumns;

            CreateFieldValues(defaultFieldValue);
        }

        #region Grid properties
        private int numberOfRows;

        public int NumberOfRows
        {
            get => numberOfRows;

            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                numberOfRows = value;
            }
        }


        private int numberOfColumns;

        public int NumberOfColumns
        {
            get => numberOfColumns;

            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                numberOfColumns = value;
            }
        }


        private T[,] fieldValues;

        private void CreateFieldValues(T defaultValue)
        {
            fieldValues = new T[NumberOfRows, NumberOfColumns];

            foreach (var p in AllPoints())
            {
                fieldValues[p.RowIndex, p.ColumnIndex] = defaultValue;
            }
        }
        #endregion

        #region Get/set field values
        private static bool IsIndexValid(int index, int length)
        {
            return index >= 0 && index < length;
        }

        protected bool IsPointInGrid(GridPoint point)
        {
            return IsIndexValid(point.RowIndex, NumberOfRows) &&
                IsIndexValid(point.ColumnIndex, NumberOfColumns);
        }

        /// <summary>
        /// Field value at the given point.
        /// </summary>
        /// <param name="point"> Point in the grid </param>
        public virtual T this[GridPoint point]
        {
            get
            {
                if (!IsPointInGrid(point))
                {
                    throw new ArgumentOutOfRangeException(nameof(point));
                }

                return GetFieldValueNoCheck(point);
            }

            set
            {
                if (!IsPointInGrid(point))
                {
                    throw new ArgumentOutOfRangeException(nameof(point));
                }

                SetFieldValueNoCheck(point, value);
            }
        }

        protected T GetFieldValueNoCheck(GridPoint point)
        {
            return fieldValues[point.RowIndex, point.ColumnIndex];
        }

        protected void SetFieldValueNoCheck(GridPoint point, T newValue)
        {
            var oldValue = GetFieldValueNoCheck(point);
            if (oldValue.Equals(newValue))
            {
                return;
            }

            fieldValues[point.RowIndex, point.ColumnIndex] = newValue;

            var e = new FieldValueChangedEventArgs<T>(point, oldValue);
            OnAfterFieldValueSet(e);
            OnFieldValueChanged(e);
        }

        public void ApplyFieldChanges(FieldsToChange<T> changes)
        {
            if (changes == null)
            {
                return;
            }

            foreach (var c in changes)
            {
                this[c.Point] = c.NewValue;
            }
        }

        /// <summary>
        /// Is called after a field value is changed, but before
        /// the FieldValueChanged event is raised.
        /// </summary>
        /// <param name="e"> Event arguments of the field change. </param>
        protected virtual void OnAfterFieldValueSet(FieldValueChangedEventArgs<T> e)
        {

        }

        /// <summary>
        /// Event raised after a field value has changed.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<FieldValueChangedEventArgs<T>> FieldValueChanged;

        private void OnFieldValueChanged(FieldValueChangedEventArgs<T> e)
        {
            FieldValueChanged?.Invoke(this, e);
        }
        #endregion

        #region Point enumerables
        /// <summary>
        /// Order: Increasing column index
        /// </summary>
        public IEnumerable<GridPoint> PointsOfRow(int rowIndex)
        {
            if (!IsIndexValid(rowIndex, NumberOfRows))
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            return PointsOfRowNoCheck(rowIndex);
        }

        private IEnumerable<GridPoint> PointsOfRowNoCheck(int rowIndex)
        {
            return Enumerable.Range(0, NumberOfColumns).Select(columnIndex => new GridPoint(rowIndex, columnIndex));
        }

        /// <summary>
        /// Order: Increasing row index
        /// </summary>
        public IEnumerable<GridPoint> PointsOfColumn(int columnIndex)
        {
            if (!IsIndexValid(columnIndex, NumberOfColumns))
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }

            return PointsOfColumnNoCheck(columnIndex);
        }

        private IEnumerable<GridPoint> PointsOfColumnNoCheck(int columnIndex)
        {
            return Enumerable.Range(0, NumberOfRows).Select(rowIndex => new GridPoint(rowIndex, columnIndex));
        }

        /// <summary>
        /// Order: Arbitrary
        /// </summary>
        public IEnumerable<GridPoint> AllPoints()
        {
            return Enumerable.Range(0, NumberOfRows).SelectMany(PointsOfRow);
        }
        #endregion

        #region Cloning
        /// <summary>
        /// Keeps subscribers to events from this.
        /// </summary>
        public virtual void OverwriteWith(object source)
        {
            if (!(source is Grid<T> sourceAsGrid))
            {
                throw new ArgumentException("Source is not a Grid.");
            }

            numberOfRows = sourceAsGrid.numberOfRows;
            numberOfColumns = sourceAsGrid.numberOfColumns;
            fieldValues = (T[,])sourceAsGrid.fieldValues.Clone();
        }
        #endregion
    }
}
