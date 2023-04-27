using System;
using System.Collections.Generic;
using Utility;

namespace Bimaru.Interface.Game
{
    public interface IBimaruGrid : ICloneable, IOverwritable
    {
        int NumberOfRows
        {
            get;
        }

        int NumberOfColumns
        {
            get;
        }

        /// <summary>
        /// Field value at the given point.
        /// </summary>
        BimaruValue this[GridPoint point]
        {
            get;
            set;
        }

        void ApplyFieldChanges(FieldsToChange<BimaruValue> changes);

        /// <summary>
        /// Fill all undetermined fields in a row to satisfy a given constraint.
        /// <see cref="BimaruValue.UNDETERMINED"/>
        /// </summary>
        void FillUndeterminedFieldsRow(int rowIndex, BimaruValueConstraint constraint);

        /// <summary>
        /// Fill all undetermined fields in a column to satisfy a given constraint.
        /// <see cref="BimaruValue.UNDETERMINED"/>
        /// </summary>
        void FillUndeterminedFieldsColumn(int columnIndex, BimaruValueConstraint constraint);

        /// <summary>
        /// Event raised after a field value has changed.
        /// </summary>
        event EventHandler<FieldValueChangedEventArgs<BimaruValue>> FieldValueChanged;

        /// <summary>
        /// <see cref="BimaruValues.IsShip(BimaruValue)"/>
        /// </summary>
        IReadOnlyList<int> NumberOfShipFieldsPerRow
        {
            get;
        }

        /// <summary>
        /// <see cref="BimaruValues.IsShip(BimaruValue)"/>
        /// </summary>
        IReadOnlyList<int> NumberOfShipFieldsPerColumn
        {
            get;
        }

        /// <summary>
        /// <see cref="BimaruValue.UNDETERMINED"/>
        /// </summary>
        IReadOnlyList<int> NumberOfUndeterminedFieldsPerRow
        {
            get;
        }

        /// <summary>
        /// <see cref="BimaruValue.UNDETERMINED"/>
        /// </summary>
        IReadOnlyList<int> NumberOfUndeterminedFieldsPerColumn
        {
            get;
        }

        /// <summary>
        /// The largest ship length (index) is the
        /// maximum of the number of rows and columns.
        /// </summary>
        IReadOnlyList<int> NumberOfShipsPerLength
        {
            get;
        }

        /// <summary>
        /// True, if the field values in the grid are compatible
        /// to each other according to the Bimaru rules.
        /// <see cref="BimaruValues.IsCompatibleWith(BimaruValue, Direction, BimaruValue)"/>
        /// </summary>
        bool IsValid
        {
            get;
        }


        /// <summary>
        /// True, if all field values in the grid are fully determined.
        /// <see cref="BimaruValues.IsFullyDetermined(BimaruValue)"/>
        /// </summary>
        bool IsFullyDetermined
        {
            get;
        }

        IEnumerable<GridPoint> AllPoints();

        IEnumerable<GridPoint> PointsOfRow(int rowIndex);

        IEnumerable<GridPoint> PointsOfColumn(int columnIndex);
    }
}
