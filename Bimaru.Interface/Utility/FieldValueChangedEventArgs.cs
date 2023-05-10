namespace Bimaru.Interface.Utility
{
    /// <typeparam name="T"> Type of field value </typeparam>
    public class FieldValueChangedEventArgs<T> : EventArgs
    {
        public FieldValueChangedEventArgs(GridPoint point, T originalValue)
        {
            Point = point;
            OriginalValue = originalValue;
        }

        /// <summary>
        /// Grid point whose field value was changed.
        /// </summary>
        public GridPoint Point { get; private set; }

        /// <summary>
        /// Original value of the field before the change.
        /// </summary>
        public T OriginalValue { get; private set; }
    }
}
