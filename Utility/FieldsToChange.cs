using System;
using System.Collections;
using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// Single change of a field value
    /// </summary>
    /// <typeparam name="T"> Field value type </typeparam>
    public struct SingleChange<T> where T : struct
    {
        /// <summary>
        /// Creates a single change.
        /// </summary>
        /// <param name="point"> Point of the field change </param>
        /// <param name="newValue"> New value </param>
        public SingleChange(GridPoint point, T newValue)
        {
            Point = point;
            NewValue = newValue;
        }

        /// <summary>
        /// Point of the field change
        /// </summary>
        public GridPoint Point { get; set; }

        /// <summary>
        /// New value of the field
        /// </summary>
        public T NewValue { get; set; }
    }

    /// <summary>
    /// Container for an arbitrary number of field changes. Only one change per field.
    /// </summary>
    /// <typeparam name="T"> Field value type </typeparam>
    public class FieldsToChange<T> : IEnumerable<SingleChange<T>> where T : struct
    {
        /// <summary>
        /// No field changes yet
        /// </summary>
        public FieldsToChange()
        {
            Changes = new Dictionary<GridPoint, SingleChange<T>>();
        }

        /// <summary>
        /// Single field change
        /// </summary>
        /// <param name="point"> Point of the field change </param>
        /// <param name="newValue"> New value </param>
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
            AddSegment(startPoint, direction, values);
        }

        private Dictionary<GridPoint, SingleChange<T>> Changes
        {
            get;
            set;
        }

        /// <summary>
        /// Add a single field change. Overwrites earlier changes at the same point.
        /// </summary>
        /// <param name="point"> Point of the field change </param>
        /// <param name="newValue"> New value </param>
        public void Add(GridPoint point, T newValue)
        {
            Changes[point] = new SingleChange<T>(point, newValue);
        }

        /// <summary>
        /// Add a segment of field changes. Overwrites earlier changes at the same points.
        /// </summary>
        /// <param name="startPoint"> Starting point </param>
        /// <param name="direction"> Direction to continue from the starting point </param>
        /// <param name="values"> Field values of the changes </param>
        public void AddSegment(GridPoint startPoint, Direction direction, IEnumerable<T> values)
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

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Changes.Values.GetEnumerator();
        }

        /// <inheritdoc/>
        public IEnumerator<SingleChange<T>> GetEnumerator()
        {
            return Changes.Values.GetEnumerator();
        }
    }
}
