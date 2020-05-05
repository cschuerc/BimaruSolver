using BimaruInterfaces;
using System;
using System.Collections.Generic;

namespace BimaruGame
{
    /// <summary>
    /// Grid with savepoint, rollback and clipboard mechanism.
    /// </summary>
    [Serializable]
    public class RollbackGrid: Grid, IRollbackGrid
    {
        /// <summary>
        /// Creates a grid with savepoints
        /// </summary>
        /// <param name="numRows"> Number of rows </param>
        /// <param name="numColumns"> Number of columns </param>
        public RollbackGrid(int numRows, int numColumns) : base(numRows, numColumns)
        {
            _gridStack = new Stack<Grid>();
        }

        #region Stack operations
        private Stack<Grid> _gridStack;

        /// <inheritdoc/>
        public void SetSavePoint()
        {
            _gridStack.Push((Grid)base.Clone());
        }

        private void Restore(Grid gridToRestore)
        {
            CopyFrom(gridToRestore);

            OnRestoreHappened();
        }

        /// <inheritdoc/>
        public void Rollback()
        {
            Restore(_gridStack.Pop());
        }
        #endregion

        #region Clipboard
        private Grid Clipboard { get; set; }

        /// <inheritdoc/>
        public void CloneToClipboard()
        {
            Clipboard = (Grid)base.Clone();
        }

        /// <inheritdoc/>
        public void RestoreFromClipboard()
        {
            if (Clipboard == null)
            {
                throw new InvalidOperationException("No grid in the clipboard.");
            }

            SetSavePoint();

            Restore(Clipboard);
        }
        #endregion

        #region Events
        /// <inheritdoc/>
        [field: NonSerialized]
        public event Action RestoreHappened;

        /// <summary>
        /// Restore-Happened event-raising method
        /// </summary>
        protected virtual void OnRestoreHappened()
        {
            RestoreHappened?.Invoke();
        }
        #endregion

        /// <inheritdoc/>
        public override object Clone()
        {
            throw new InvalidOperationException();
        }
    }
}
