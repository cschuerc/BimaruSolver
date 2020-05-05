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
        int NumRows { get; }

        /// <summary>
        /// Number of columns of the grid
        /// </summary>
        int NumColumns { get; }

        /// <summary>
        /// Grid field value at a given point.
        /// </summary>
        /// <param name="point"> Point in the grid </param>
        /// <returns> Value at the given point </returns>
        BimaruValue this[GridPoint point] { get; set; }

        /// <summary>
        /// Fill all undetermined fields in a row to satisfy a given constraint.
        /// </summary>
        /// <param name="rowIndex"> The index of the row to fill. </param>
        /// <param name="constraint"> Constraint the undetermined fields need to satisfy. </param>
        void FillUndeterminedFieldsRow(int rowIndex, BimaruValueConstraint constraint);

        /// <summary>
        /// Fill all undetermined fields in a column to satisfy a given constraint.
        /// </summary>
        /// <param name="columnIndex"> Index of the column to fill. </param>
        /// <param name="constraint"> Constraint the undetermined fields need to satisfy. </param>
        void FillUndeterminedFieldsColumn(int columnIndex, BimaruValueConstraint constraint);

        /// <summary>
        /// Event raised after every field value change.
        /// </summary>
        event EventHandler<FieldValueChangedEventArgs<BimaruValue>> FieldValueChanged;

        /// <summary>
        /// Get the number of ship fields per row.
        /// </summary>
        /// <returns> Number of fields per row that are ship values. </returns>
        IReadOnlyList<int> GetNumShipFieldsRow { get; }

        /// <summary>
        /// Get the number of ship fields per column.
        /// </summary>
        /// <returns> Number of fields per column that are ship values. </returns>
        IReadOnlyList<int> GetNumShipFieldsColumn { get; }

        /// <summary>
        /// Get the number of UNDETERMINED fields per row
        /// </summary>
        /// <returns> Number of fields per row that are UNDETERMINED </returns>
        IReadOnlyList<int> GetNumUndeterminedFieldsRow { get; }

        /// <summary>
        /// Get the number of UNDETERMINED fields per column
        /// </summary>
        /// <returns> Number of fields per column that are UNDETERMINED </returns>
        IReadOnlyList<int> GetNumUndeterminedFieldsColumn { get; }

        /// <summary>
        /// Get the number of ships per length. The largest index is the length of
        /// the row or column depending on which is larger.
        /// </summary>
        /// <returns> Number of ships per length in the grid. </returns>
        IReadOnlyList<int> GetNumShips { get; }

        /// <summary>
        /// True, if the grid values are compatible to each other according to the Bimaru rules.
        /// </summary>
        bool IsValid { get; }


        /// <summary>
        /// True, if all field values are fully determined.
        /// </summary>
        bool IsFullyDetermined { get; }

        /// <summary>
        /// Enumerable over all points of the grid. Arbitrary order.
        /// </summary>
        /// <returns></returns>
        IEnumerable<GridPoint> AllPoints();

        /// <summary>
        /// Enumerable over all points of the given row starting from the zero column.
        /// </summary>
        /// <param name="rowIndex"> Index of the row to enumerate over. </param>
        /// <returns></returns>
        IEnumerable<GridPoint> PointsOfRow(int rowIndex);

        /// <summary>
        /// Enumerable over all points of the given column starting from the zero row.
        /// </summary>
        /// <param name="columnIndex"> Index of the column to enumerate over. </param>
        /// <returns></returns>
        IEnumerable<GridPoint> PointsOfColumn(int columnIndex);
    }
}
