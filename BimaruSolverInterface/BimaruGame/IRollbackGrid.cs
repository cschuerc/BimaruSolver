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
        /// Removes the previous savepoint without changing the current grid.
        /// </summary>
        void RemovePrevious();

        /// <summary>
        /// Event raised after every rollback.
        /// </summary>
        event System.Action RollbackHappened;
    }
}
