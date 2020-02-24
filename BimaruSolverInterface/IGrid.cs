using System;
using System.Collections.Generic;
using Utility;

namespace BimaruInterfaces
{
    /// <summary>
    /// Bimaru grid
    /// </summary>
    public interface IGrid: ICloneable
    {
        /// <summary>
        /// Number of rows of the grid
        /// </summary>
        int NumRows
        {
            get;
        }

        /// <summary>
        /// Number of columns of the grid
        /// </summary>
        int NumColumns
        {
            get;
        }

        /// <summary>
        /// Set the value of a grid field
        /// </summary>
        /// <param name="point"> Point whose grid field value is set </param>
        /// <param name="value"> Value which the field is set to </param>
        void SetFieldValue(IGridPoint point, FieldValues value);

        /// <summary>
        /// Event raised after every field value change.
        /// </summary>
        event EventHandler<FieldValueChangedEventArgs<FieldValues>> FieldValueChanged;

        /// <summary>
        /// Get the value of a grid field
        /// </summary>
        /// <param name="point"> Point whose grid field value is returned </param>
        /// <returns> Value of the desired grid field </returns>
        FieldValues GetFieldValue(IGridPoint point);

        /// <summary>
        /// Get the number of ship fields per row
        /// </summary>
        /// <returns> Number of fields per row that are any ship values </returns>
        IReadOnlyList<int> GetNumShipFieldsRow
        {
            get;
        }

        /// <summary>
        /// Get the number of ship fields per column
        /// </summary>
        /// <returns> Number of fields per column that are any ship value </returns>
        IReadOnlyList<int> GetNumShipFieldsColumn
        {
            get;
        }

        /// <summary>
        /// Get the number of empty fields per row
        /// </summary>
        /// <returns> Number of fields per row that are empty </returns>
        IReadOnlyList<int> GetNumEmptyFieldsRow
        {
            get;
        }

        /// <summary>
        /// Get the number of empty fields per column
        /// </summary>
        /// <returns> Number of fields per column that are empty </returns>
        IReadOnlyList<int> GetNumEmptyFieldsColumn
        {
            get;
        }

        /// <summary>
        /// Get the number of ships per length
        /// </summary>
        /// <returns> Number of ships of the given length that are fully set </returns>
        IReadOnlyList<int> GetNumShips
        {
            get;
        }

        /// <summary>
        /// True, if the grid values are compatible to each other according to the Bimaru rules.
        /// </summary>
        bool IsValid
        {
            get;
        }


        /// <summary>
        /// True if all field values are fully determined.
        /// </summary>
        bool IsFullyDetermined
        {
            get;
        }
    }
}
