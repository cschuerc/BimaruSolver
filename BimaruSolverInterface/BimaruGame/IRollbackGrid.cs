namespace BimaruInterfaces
{
    /// <summary>
    /// Grid with savepoints and rollback mechanism
    /// </summary>
    public interface IRollbackGrid: IGrid
    {
        /// <summary>
        /// Sets a savepoint such that the next Rollback goes back to the current state.
        /// </summary>
        void SetSavePoint();

        /// <summary>
        /// Rolls back to the last savepoint.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Saves the current grid to the clipboard (separate from the savepoint, rollback area).
        /// </summary>
        void CloneToClipboard();

        /// <summary>
        /// Sets a savepoint and puts the grid in the clipboard as the current grid.
        /// </summary>
        void RestoreFromClipboard();

        /// <summary>
        /// Event raised after every restore (from clipboard or by a rollback).
        /// </summary>
        event System.Action RestoreHappened;
    }
}
