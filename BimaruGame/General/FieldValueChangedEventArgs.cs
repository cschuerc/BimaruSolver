﻿using System;

namespace Bimaru.General
{
    /// <summary>
    /// Arguments for a grid field value changed event
    /// </summary>
    /// <typeparam name="T"> Field value type </typeparam>
    public class FieldValueChangedEventArgs<T>: EventArgs
    {
        /// <summary>
        /// Constructs the arguments
        /// </summary>
        /// <param name="point"> Grid point whose field value was changed. </param>
        /// <param name="origValue"> Original value of the field before the change. </param>
        public FieldValueChangedEventArgs(IGridPoint point, T origValue)
        {
            Point = point;
            OriginalValue = origValue;
        }

        /// <summary>
        /// Grid point whose field value was changed.
        /// </summary>
        public IGridPoint Point
        {
            get;
            private set;
        }

        /// <summary>
        /// Original value of the field before the change.
        /// </summary>
        public T OriginalValue
        {
            get;
            private set;
        }
    }
}
