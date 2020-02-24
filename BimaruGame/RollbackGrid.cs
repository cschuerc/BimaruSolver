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
        private Stack<IGrid> _gridStack;

        /// <summary>
        /// Initializes the grid with an initial grid.
        /// </summary>
        /// <param name="initialGrid"> Initial grid </param>
        public RollbackGrid(IGrid initialGrid)
        {
            if (initialGrid == null)
            {
                throw new ArgumentNullException();
            }

            _gridStack = new Stack<IGrid>();
            Push((IGrid)initialGrid.Clone());
        }

        /// <summary>
        /// Pushes the grid onto the grid stack
        /// </summary>
        /// <param name="gridToPush"> Grid to be psuhed. Must be != null. </param>
        protected void Push(IGrid gridToPush)
        {
            if (_gridStack.Count > 0)
            {
                IGrid activeGrid = ActiveGrid;
                if (activeGrid != null)
                {
                    activeGrid.FieldValueChanged -= GridFieldValueChanged;
                }
            }

            gridToPush.FieldValueChanged += GridFieldValueChanged;
            _gridStack.Push(gridToPush);
        }

        private void GridFieldValueChanged(object sender, FieldValueChangedEventArgs<FieldValues> e)
        {
            OnFieldValueChanged(e);
        }

        /// <summary>
        /// Pops the top grid from the grid stack
        /// </summary>
        protected void Pop()
        {
            IGrid poppedGrid = _gridStack.Pop();
            poppedGrid.FieldValueChanged -= GridFieldValueChanged;

            // By design, there has to be a grid after a Pop operation
            ActiveGrid.FieldValueChanged += GridFieldValueChanged;
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
        }

        /// <inheritdoc/>
        public void RollbackToInitial()
        {
            int numGridsToBackTrack = _gridStack.Count - 1;

            while (numGridsToBackTrack > 0)
            {
                Pop();

                numGridsToBackTrack--;
            }
        }

        /// <inheritdoc/>
        public void RemoveIntermediate()
        {
            if (_gridStack.Count > 1)
            {
                IGrid activeGrid = ActiveGrid;

                RollbackToInitial();

                Push(activeGrid);
            }
        }

        /// <summary>
        /// Currently active grid
        /// </summary>
        protected IGrid ActiveGrid
            => _gridStack.Peek();

        /// <inheritdoc/>
        public event EventHandler<FieldValueChangedEventArgs<FieldValues>> FieldValueChanged;

        /// <summary>
        /// FieldValueChanged event-raising method
        /// </summary>
        /// <param name="e"> Event arguments </param>
        protected virtual void OnFieldValueChanged(FieldValueChangedEventArgs<FieldValues> e)
        {
            FieldValueChanged?.Invoke(this, e);
        }

        /// <inheritdoc/>
        public void SetFieldValue(IGridPoint point, FieldValues value)
            => ActiveGrid.SetFieldValue(point, value);

        /// <inheritdoc/>
        public FieldValues GetFieldValue(IGridPoint point)
            => ActiveGrid.GetFieldValue(point);

        /// <inheritdoc/>
        public object Clone()
        {
            throw new InvalidOperationException();
        }

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
        public IReadOnlyList<int> GetNumEmptyFieldsRow
            => ActiveGrid.GetNumEmptyFieldsRow;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumEmptyFieldsColumn
            => ActiveGrid.GetNumEmptyFieldsColumn;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumShips
            => ActiveGrid.GetNumShips;

        /// <inheritdoc/>
        public bool IsValid
            => ActiveGrid.IsValid;

        /// <inheritdoc/>
        public bool IsFullyDetermined
            => ActiveGrid.IsFullyDetermined;
    }
}
