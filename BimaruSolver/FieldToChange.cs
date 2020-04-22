using BimaruInterfaces;
using Utility;

namespace BimaruSolver
{
    /// <summary>
    /// Represents a field in a Bimaru grid and how it is supposed to be changed.
    /// </summary>
    public struct FieldToChange
    {
        /// <summary>
        /// New field to change.
        /// </summary>
        /// <param name="point"> Grid point of the field </param>
        /// <param name="newValue"> New value of the field at the given point. </param>
        public FieldToChange(IGridPoint point, FieldValues newValue)
        {
            Point = point;
            NewValue = newValue;
        }

        /// <summary>
        /// Grid point whose field is shall be changed.
        /// </summary>
        public IGridPoint Point
        {
            get;
            private set;
        }

        /// <summary>
        /// New value of the field.
        /// </summary>
        public FieldValues NewValue
        {
            get;
            private set;
        }
    }
}
