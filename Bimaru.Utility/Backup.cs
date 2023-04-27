using System;
using System.Collections.Generic;
using Bimaru.Interface.Game;

namespace Bimaru.Utility
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
                throw new ArgumentNullException(nameof(source));
            }

            savePointStack.Push((T)source.Clone());
        }

        public void RestoreAndDeleteLastSavepoint(T destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
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
                throw new ArgumentNullException(nameof(source));
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
                throw new ArgumentNullException(nameof(destination));
            }

            destination.OverwriteWith(Clipboard);
        }
        #endregion
    }
}
