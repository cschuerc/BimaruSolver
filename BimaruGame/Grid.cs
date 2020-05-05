using BimaruInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace BimaruGame
{
    /// <summary>
    /// Bimaru grid implementation
    /// </summary>
    [Serializable]
    public class Grid : GridBase<BimaruValue>, IGrid, ICloneable
    {
        /// <summary>
        /// Instantiates a bimaru grid with corresponding number of rows/columns
        /// </summary>
        /// <param name="numRows"> Positive number of rows </param>
        /// <param name="numColumns"> Positive number of columns </param>
        public Grid(int numRows, int numColumns)
            : base(numRows, numColumns, BimaruValue.UNDETERMINED)
        {
            _numUndeterminedFieldsRow = new int[numRows].InitValues(numColumns);
            _numShipFieldsRow = new int[numRows].InitValues(0);

            _numUndeterminedFieldsColumn = new int[numColumns].InitValues(numRows);
            _numShipFieldsColumn = new int[numColumns].InitValues(0);

            int maxShipLength = Math.Max(numRows, numColumns);
            _numShipsOfLength = new int[maxShipLength + 1].InitValues(0); // +1 due to ship length of zero

            _numNotFullyDeterminedFields = numRows * numColumns;

            _invalidBoundaries = new HashSet<FieldBoundary>();
        }

        #region Get/set fields
        /// <summary>
        /// As the base class but allows off-grid points where WATER is returned.
        /// </summary>
        /// <param name="point"> Point whose grid field value is returned </param>
        /// <returns> Value of the desired grid field </returns>
        public override BimaruValue this[GridPoint point]
        {
            get
            {
                if (!IsPointInGrid(point))
                {
                    return BimaruValue.WATER;
                }

                return base[point];
            }

            set
            {
                if (!IsPointInGrid(point) && value == BimaruValue.WATER)
                {
                    // No contradiction as the world around the grid is water.
                    return;
                }

                base[point] = value;
            }
        }

        /// <inheritdoc/>
        public void FillUndeterminedFieldsRow(int rowIndex, BimaruValueConstraint constraint)
        {
            BimaruValue valueToSet = constraint.GetRepresentativeValue();
            if (valueToSet == BimaruValue.UNDETERMINED ||
                _numUndeterminedFieldsRow[rowIndex] == 0)
            {
                return;
            }

            foreach (GridPoint p in PointsOfRow(rowIndex).Where( p => GetFieldValueNoCheck(p) == BimaruValue.UNDETERMINED))
            {
                this[p] = valueToSet;
            }
        }

        /// <inheritdoc/>
        public void FillUndeterminedFieldsColumn(int columnIndex, BimaruValueConstraint constraint)
        {
            BimaruValue valueToSet = constraint.GetRepresentativeValue();
            if (valueToSet == BimaruValue.UNDETERMINED ||
                _numUndeterminedFieldsColumn[columnIndex] == 0)
            {
                return;
            }

            foreach (GridPoint p in PointsOfColumn(columnIndex).Where( p => GetFieldValueNoCheck(p) == BimaruValue.UNDETERMINED))
            {
                this[p] = valueToSet;
            }
        }

        /// <inheritdoc/>
        protected override void OnAfterFieldValueSet(FieldValueChangedEventArgs<BimaruValue> e)
        {
            UpdateFieldCountNumbers(e);
            UpdateInvalidBoundaries(e.Point);
            UpdateNumShips(e);
        }
        #endregion

        #region Field counts
        private int[] _numShipFieldsRow;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumShipFieldsRow
            => Array.AsReadOnly(_numShipFieldsRow);


        private int[] _numShipFieldsColumn;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumShipFieldsColumn
            => Array.AsReadOnly(_numShipFieldsColumn);


        private int[] _numUndeterminedFieldsRow;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumUndeterminedFieldsRow
            => Array.AsReadOnly(_numUndeterminedFieldsRow);


        private int[] _numUndeterminedFieldsColumn;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumUndeterminedFieldsColumn
            => Array.AsReadOnly(_numUndeterminedFieldsColumn);


        private int _numNotFullyDeterminedFields;

        /// <inheritdoc/>
        public bool IsFullyDetermined
            => _numNotFullyDeterminedFields == 0;


        private void UpdateFieldCountNumbers(FieldValueChangedEventArgs<BimaruValue> e)
        {
            var newValue = this[e.Point];

            int numShipChange = GetFieldCountChange(e.OriginalValue.IsShip(), newValue.IsShip());
            _numShipFieldsRow[e.Point.RowIndex] += numShipChange;
            _numShipFieldsColumn[e.Point.ColumnIndex] += numShipChange;

            int numUndeterminedChange = GetFieldCountChange(e.OriginalValue == BimaruValue.UNDETERMINED, newValue == BimaruValue.UNDETERMINED);
            _numUndeterminedFieldsRow[e.Point.RowIndex] += numUndeterminedChange;
            _numUndeterminedFieldsColumn[e.Point.ColumnIndex] += numUndeterminedChange;

            _numNotFullyDeterminedFields += GetFieldCountChange(!e.OriginalValue.IsFullyDetermined(), !newValue.IsFullyDetermined());
        }

        private int GetFieldCountChange(bool wasBefore, bool isAfter)
        {
            return (isAfter ? 1 : 0) - (wasBefore ? 1 : 0);
        }
        #endregion

        #region IsValid
        private HashSet<FieldBoundary> _invalidBoundaries;

        /// <inheritdoc/>
        public bool IsValid
            => _invalidBoundaries.Count == 0;

        private void UpdateInvalidBoundaries(GridPoint center)
        {
            BimaruValue centerValue = GetFieldValueNoCheck(center);
            foreach (Direction direction in DirectionExtensions.AllDirections())
            {
                BimaruValue neighbourValue = this[center.GetNextPoint(direction)];
                FieldBoundary boundary = center.GetBoundary(direction);
                if (centerValue.IsCompatibleWith(direction, neighbourValue))
                {
                    _invalidBoundaries.Remove(boundary);
                }
                else
                {
                    _invalidBoundaries.Add(boundary);
                }
            }
        }
        #endregion

        #region Ship count
        private int[] _numShipsOfLength;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumShips
            => Array.AsReadOnly(_numShipsOfLength);

        private int? GetShipPartLength(GridPoint startPoint, BimaruValue startValue, Direction direction)
        {
            if (startValue == BimaruValue.SHIP_SINGLE)
            {
                return 1;
            }

            int shipLength = 0;

            GridPoint currentPoint = startPoint;
            BimaruValue currentValue = startValue;
            if (currentValue == direction.GetFirstShipValue())
            {
                shipLength++;
                currentPoint = currentPoint.GetNextPoint(direction);
                currentValue = this[currentPoint];
            }
            
            while (currentValue == BimaruValue.SHIP_MIDDLE)
            {
                shipLength++;
                currentPoint = currentPoint.GetNextPoint(direction);
                currentValue = this[currentPoint];
            }

            if (currentValue == direction.GetLastShipValue())
            {
                shipLength++;
                return shipLength;
            }

            return null;
        }

        private int? GetShipLength(GridPoint crossPoint, BimaruValue crossValue, DirectionType directionType)
        {
            int? shipLength = null;

            if (directionType == DirectionType.ROW)
            {
                // -1 due to double count of the cross point
                shipLength = GetShipPartLength(crossPoint, crossValue, Direction.LEFT) +
                             GetShipPartLength(crossPoint, crossValue, Direction.RIGHT) - 1;
            }
            else if (directionType == DirectionType.COLUMN)
            {
                // -1 due to double count of the cross point
                shipLength = GetShipPartLength(crossPoint, crossValue, Direction.UP) +
                             GetShipPartLength(crossPoint, crossValue, Direction.DOWN) - 1;
            }

            return shipLength;
        }

        private void UpdateNumShips(FieldValueChangedEventArgs<BimaruValue> e)
        {
            int? horizontalShipLengthOrig = GetShipLength(e.Point, e.OriginalValue, DirectionType.ROW);
            if (horizontalShipLengthOrig.HasValue)
            {
                _numShipsOfLength[horizontalShipLengthOrig.Value]--;
            }

            int? verticalShipLengthOrig = GetShipLength(e.Point, e.OriginalValue, DirectionType.COLUMN);
            if (verticalShipLengthOrig.HasValue &&
                e.OriginalValue != BimaruValue.SHIP_SINGLE) // SHIP_SINGLE already detected horizontally
            {
                _numShipsOfLength[verticalShipLengthOrig.Value]--;
            }


            BimaruValue newValue = GetFieldValueNoCheck(e.Point);
            int? horizontalShipLength = GetShipLength(e.Point, newValue, DirectionType.ROW);
            if (horizontalShipLength.HasValue)
            {
                _numShipsOfLength[horizontalShipLength.Value]++;
            }

            int? verticalShipLength = GetShipLength(e.Point, newValue, DirectionType.COLUMN);
            if (verticalShipLength.HasValue &&
                newValue != BimaruValue.SHIP_SINGLE) // SHIP_SINGLE already detected horizontally
            {
                _numShipsOfLength[verticalShipLength.Value]++;
            }
        }
        #endregion

        #region Cloning
        /// <summary>
        /// Do a deep copy from a Grid template object
        /// </summary>
        /// <param name="template"> Grid to copy from </param>
        public void CopyFrom(Grid template)
        {
            base.CopyFrom(template);

            _numNotFullyDeterminedFields = template._numNotFullyDeterminedFields;

            _numUndeterminedFieldsColumn = (int[])template._numUndeterminedFieldsColumn.Clone();
            _numUndeterminedFieldsRow = (int[])template._numUndeterminedFieldsRow.Clone();
            _numShipFieldsColumn = (int[])template._numShipFieldsColumn.Clone();
            _numShipFieldsRow = (int[])template._numShipFieldsRow.Clone();
            _numShipsOfLength = (int[])template._numShipsOfLength.Clone();

            _invalidBoundaries = new HashSet<FieldBoundary>(template._invalidBoundaries);
        }

        /// <inheritdoc/>
        public virtual object Clone()
        {
            Grid clonedGrid = new Grid(NumRows, NumColumns);
            clonedGrid.CopyFrom(this);
            return clonedGrid;
        }
        #endregion
    }
}
