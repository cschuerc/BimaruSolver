using System;
using System.Collections;
using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// Single change of a field value
    /// </summary>
    /// <typeparam name="T"> Type of field value </typeparam>
    public struct SingleChange<T>
    {
        public SingleChange(GridPoint point, T newValue)
        {
            Point = point;
            NewValue = newValue;
        }

        public GridPoint Point
        {
            get;
            private set;
        }

        public T NewValue
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Container for an arbitrary number of field changes. Only one change per field.
    /// </summary>
    /// <typeparam name="T"> Type of field value </typeparam>
    public class FieldsToChange<T> : IEnumerable<SingleChange<T>>
    {
        public FieldsToChange()
        {
            fieldChanges = new Dictionary<GridPoint, SingleChange<T>>();
        }

        /// <summary>
        /// Single field change
        /// </summary>
        public FieldsToChange(GridPoint point, T newValue) : this()
        {
            Add(point, newValue);
        }

        /// <summary>
        /// A segment of field changes
        /// </summary>
        /// <param name="startPoint"> Starting point </param>
        /// <param name="direction"> Direction to continue from the starting point </param>
        /// <param name="values"> Field values of the changes </param>
        public FieldsToChange(GridPoint startPoint, Direction direction, IEnumerable<T> values) : this()
        {
            if (values == null)
            {
                throw new ArgumentNullException();
            }

            GridPoint currentPoint = startPoint;
            foreach (T v in values)
            {
                Add(currentPoint, v);
                currentPoint = currentPoint.GetNextPoint(direction);
            }
        }

        private readonly Dictionary<GridPoint, SingleChange<T>> fieldChanges;

        /// <summary>
        /// Add a single field change. Overwrites earlier changes at the same point.
        /// </summary>
        public void Add(GridPoint point, T newValue)
        {
            fieldChanges[point] = new SingleChange<T>(point, newValue);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return fieldChanges.Values.GetEnumerator();
        }

        public IEnumerator<SingleChange<T>> GetEnumerator()
        {
            return fieldChanges.Values.GetEnumerator();
        }
    }
}
