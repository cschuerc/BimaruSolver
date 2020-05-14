using System;

namespace Utility
{
    public interface IBackup<T> where T : ICloneable, IOverwritable
    {
        /// <summary>
        /// Sets a save point with a clone of the source.
        /// </summary>
        void SetSavePoint(T source);

        /// <summary>
        /// Restores the last savepoint to the destination and deletes that savepoint.
        /// </summary>
        void RestoreAndDeleteLastSavepoint(T destination);

        /// <summary>
        /// Saves a clone of the source to the clipboard
        /// </summary>
        void CloneToClipboard(T source);

        /// <summary>
        /// Restores the clipboard to the destination.
        /// </summary>
        void RestoreFromClipboardTo(T destination);
    }
}
