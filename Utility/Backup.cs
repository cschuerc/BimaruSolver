using System;
using System.Collections.Generic;

namespace Utility
{
    [Serializable]
    public class Backup<T> : IBackup<T> where T : ICloneable, IOverwritable
    {
        public Backup()
        {
            savePointStack = new Stack<T>();
        }

        #region Stack operations
        private readonly Stack<T> savePointStack;

        public void SetSavePoint(T source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Source of save point.");
            }

            savePointStack.Push((T)source.Clone());
        }

        public void RestoreAndDeleteLastSavepoint(T destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("Destination of save point.");
            }

            destination.OverwriteWith(savePointStack.Pop());
        }
        #endregion

        #region Clipboard
        private T Clipboard
        {
            get;
            set;
        }

        public void CloneToClipboard(T source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Source for clipboard.");
            }

            Clipboard = (T)source.Clone();
        }

        public void RestoreFromClipboardTo(T destination)
        {
            if (Clipboard == null)
            {
                throw new InvalidOperationException("Empty clipboard.");
            }

            if (destination == null)
            {
                throw new ArgumentNullException("Destination for clipboard restore.");
            }

            destination.OverwriteWith(Clipboard);
        }
        #endregion
    }
}
