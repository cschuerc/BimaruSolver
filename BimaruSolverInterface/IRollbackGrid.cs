﻿namespace BimaruInterfaces
{
    /// <summary>
    /// Grid with savepoints and rollback mechanism
    /// </summary>
    public interface IRollbackGrid: IGrid
    {
        /// <summary>
        /// Sets a savepoint such that the next Rollback goes back to the state when the savepoint was set.
        /// </summary>
        void SetSavePoint();

        /// <summary>
        /// Rolls back to the last savepoint.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Rolls back to the first savepoint
        /// </summary>
        void RollbackToInitial();

        /// <summary>
        /// Removes the previous savepoint.
        /// </summary>
        void RemovePrevious();

        /// <summary>
        /// Event raised after every rollback.
        /// </summary>
        event System.Action RollbackHappened;
    }
}
