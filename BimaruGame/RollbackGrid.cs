using BimaruInterfaces;
using System;
using System.Collections.Generic;
using Utility;

namespace BimaruGame
{
    /// <summary>
    /// Grid with savepoint and rollback mechanism.
    /// </summary>
    public class RollbackGrid: IRollbackGrid
    {
        /// <summary>
        /// Initializes the grid with an initial grid.
        /// </summary>
        /// <param name="initialGrid"> Initial grid </param>
        public RollbackGrid(IGrid initialGrid)
        {
            _gridStack = new Stack<IGrid>();

            if (initialGrid == null)
            {
                throw new ArgumentNullException();
            }

            Push((IGrid)initialGrid.Clone());
        }

        #region Stack operations
        private Stack<IGrid> _gridStack;

        /// <summary>
        /// Currently active grid
        /// </summary>
        protected IGrid ActiveGrid
            => _gridStack.Peek();

        /// <summary>
        /// Pushes the grid onto the grid stack
        /// </summary>
        /// <param name="gridToPush"> Grid to be psuhed. Must be != null. </param>
        protected void Push(IGrid gridToPush)
        {
            if (_gridStack.Count > 0)
            {
                ActiveGrid.FieldValueChanged -= GridFieldValueChanged;
            }

            gridToPush.FieldValueChanged += GridFieldValueChanged;
            _gridStack.Push(gridToPush);
        }

        /// <summary>
        /// Pops the top grid from the grid stack
        /// </summary>
        protected void Pop()
        {
            IGrid poppedGrid = _gridStack.Pop();
            poppedGrid.FieldValueChanged -= GridFieldValueChanged;

            if (_gridStack.Count > 0)
            {
                ActiveGrid.FieldValueChanged += GridFieldValueChanged;
            }
        }

        /// <inheritdoc/>
        public void SetSavePoint()
        {
            IGrid clonedGrid = (IGrid)ActiveGrid.Clone();
            Push(clonedGrid);
        }

        /// <inheritdoc/>
        public void Rollback()
        {
            if (_gridStack.Count <= 1)
            {
                throw new InvalidOperationException();
            }

            Pop();

            OnRollbackHappened();
        }

        /// <inheritdoc/>
        public void RemovePrevious()
        {
            if (_gridStack.Count > 1)
            {
                IGrid activeGrid = ActiveGrid;

                Pop();
                Pop();

                Push(activeGrid);
            }
        }
        #endregion

        #region Events
        /// <inheritdoc/>
        public event Action RollbackHappened;

        /// <summary>
        /// RollbackHappened event-raising method
        /// </summary>
        protected virtual void OnRollbackHappened()
        {
            RollbackHappened?.Invoke();
        }

        /// <inheritdoc/>
        public event EventHandler<FieldValueChangedEventArgs<BimaruValue>> FieldValueChanged;

        private void GridFieldValueChanged(object sender, FieldValueChangedEventArgs<BimaruValue> e)
        {
            // Don't use 'sender' as for the outside world the RollbackGrid is the sender
            FieldValueChanged?.Invoke(this, e);
        }
        #endregion

        #region Relayed methods
        /// <inheritdoc/>
        public void SetFieldValue(GridPoint point, BimaruValue value)
            => ActiveGrid.SetFieldValue(point, value);

        /// <inheritdoc/>
        public BimaruValue GetFieldValue(GridPoint point)
            => ActiveGrid.GetFieldValue(point);

        /// <inheritdoc/>
        public void FillUndeterminedFieldsRow(int rowIndex, BimaruValueConstraint constraint)
            => ActiveGrid.FillUndeterminedFieldsRow(rowIndex, constraint);

        /// <inheritdoc/>
        public void FillUndeterminedFieldsColumn(int columnIndex, BimaruValueConstraint constraint)
            => ActiveGrid.FillUndeterminedFieldsColumn(columnIndex, constraint);

        /// <inheritdoc/>
        public IEnumerable<GridPoint> AllPoints()
            => ActiveGrid.AllPoints();

        /// <inheritdoc/>
        public IEnumerable<GridPoint> PointsOfRow(int rowIndex)
            => ActiveGrid.PointsOfRow(rowIndex);

        /// <inheritdoc/>
        public IEnumerable<GridPoint> PointsOfColumn(int columnIndex)
            => ActiveGrid.PointsOfColumn(columnIndex);

        /// <inheritdoc/>
        public int NumRows
            => ActiveGrid.NumRows;

        /// <inheritdoc/>
        public int NumColumns
            => ActiveGrid.NumColumns;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumShipFieldsRow
            => ActiveGrid.GetNumShipFieldsRow;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumShipFieldsColumn
            => ActiveGrid.GetNumShipFieldsColumn;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumUndeterminedFieldsRow
            => ActiveGrid.GetNumUndeterminedFieldsRow;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumUndeterminedFieldsColumn
            => ActiveGrid.GetNumUndeterminedFieldsColumn;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumShips
            => ActiveGrid.GetNumShips;

        /// <inheritdoc/>
        public bool IsValid
            => ActiveGrid.IsValid;

        /// <inheritdoc/>
        public bool IsFullyDetermined
            => ActiveGrid.IsFullyDetermined;
        #endregion

        /// <inheritdoc/>
        public object Clone()
        {
            throw new InvalidOperationException();
        }
    }
}
