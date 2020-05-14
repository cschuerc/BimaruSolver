using BimaruInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace BimaruGame
{
    [Serializable]
    public class BimaruGrid : Grid<BimaruValue>, IBimaruGrid, ICloneable
    {
        public BimaruGrid(int numberOfRows, int numberOfColumns)
            : base(numberOfRows, numberOfColumns, BimaruValue.UNDETERMINED)
        {
            numberOfUndeterminedFieldsPerRow = new int[numberOfRows].InitValues(numberOfColumns);
            numberOfShipFieldsPerRow = new int[numberOfRows].InitValues(0);

            numberOfUndeterminedFieldsPerColumn = new int[numberOfColumns].InitValues(numberOfRows);
            numberOfShipFieldsPerColumn = new int[numberOfColumns].InitValues(0);

            int maximumShipLength = Math.Max(numberOfRows, numberOfColumns);
            numberOfShipsPerLength = new int[maximumShipLength + 1].InitValues(0); // +1 due to ship length of zero

            numberOfNotFullyDeterminedFields = numberOfRows * numberOfColumns;

            invalidFieldBoundaries = new HashSet<FieldBoundary>();
        }

        #region Get/set fields
        /// <summary>
        /// Off-grid points are allowed if they are set to WATER.
        /// </summary>
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

        public void FillUndeterminedFieldsRow(int rowIndex, BimaruValueConstraint constraint)
        {
            BimaruValue valueToSet = constraint.GetRepresentativeValue();
            if (valueToSet == BimaruValue.UNDETERMINED ||
                numberOfUndeterminedFieldsPerRow[rowIndex] == 0)
            {
                return;
            }

            foreach (GridPoint p in PointsOfRow(rowIndex).Where( p => GetFieldValueNoCheck(p) == BimaruValue.UNDETERMINED))
            {
                this[p] = valueToSet;
            }
        }

        public void FillUndeterminedFieldsColumn(int columnIndex, BimaruValueConstraint constraint)
        {
            BimaruValue valueToSet = constraint.GetRepresentativeValue();
            if (valueToSet == BimaruValue.UNDETERMINED ||
                numberOfUndeterminedFieldsPerColumn[columnIndex] == 0)
            {
                return;
            }

            foreach (GridPoint p in PointsOfColumn(columnIndex).Where( p => GetFieldValueNoCheck(p) == BimaruValue.UNDETERMINED))
            {
                this[p] = valueToSet;
            }
        }

        protected override void OnAfterFieldValueSet(FieldValueChangedEventArgs<BimaruValue> e)
        {
            UpdateNumberOfFields(e);
            UpdateInvalidFieldBoundaries(e.Point);
            UpdateNumberOfShipsPerLength(e);
        }
        #endregion

        #region Field counts
        private int[] numberOfShipFieldsPerRow;

        public IReadOnlyList<int> NumberOfShipFieldsPerRow
            => Array.AsReadOnly(numberOfShipFieldsPerRow);


        private int[] numberOfShipFieldsPerColumn;

        public IReadOnlyList<int> NumberOfShipFieldsPerColumn
            => Array.AsReadOnly(numberOfShipFieldsPerColumn);


        private int[] numberOfUndeterminedFieldsPerRow;

        public IReadOnlyList<int> NumberOfUndeterminedFieldsPerRow
            => Array.AsReadOnly(numberOfUndeterminedFieldsPerRow);


        private int[] numberOfUndeterminedFieldsPerColumn;

        public IReadOnlyList<int> NumberOfUndeterminedFieldsPerColumn
            => Array.AsReadOnly(numberOfUndeterminedFieldsPerColumn);


        private int numberOfNotFullyDeterminedFields;

        public bool IsFullyDetermined
            => numberOfNotFullyDeterminedFields == 0;


        private void UpdateNumberOfFields(FieldValueChangedEventArgs<BimaruValue> e)
        {
            int change;
            var newValue = this[e.Point];

            change = GetChangeInNumberOfFields(e.OriginalValue.IsShip(), newValue.IsShip());
            numberOfShipFieldsPerRow[e.Point.RowIndex] += change;
            numberOfShipFieldsPerColumn[e.Point.ColumnIndex] += change;

            change = GetChangeInNumberOfFields(e.OriginalValue == BimaruValue.UNDETERMINED, newValue == BimaruValue.UNDETERMINED);
            numberOfUndeterminedFieldsPerRow[e.Point.RowIndex] += change;
            numberOfUndeterminedFieldsPerColumn[e.Point.ColumnIndex] += change;

            change = GetChangeInNumberOfFields(!e.OriginalValue.IsFullyDetermined(), !newValue.IsFullyDetermined());
            numberOfNotFullyDeterminedFields += change;
        }

        private int GetChangeInNumberOfFields(bool wasBefore, bool isAfter)
        {
            return (isAfter ? 1 : 0) - (wasBefore ? 1 : 0);
        }
        #endregion

        #region IsValid
        private HashSet<FieldBoundary> invalidFieldBoundaries;

        public bool IsValid
            => invalidFieldBoundaries.Count == 0;

        private void UpdateInvalidFieldBoundaries(GridPoint center)
        {
            BimaruValue centerValue = GetFieldValueNoCheck(center);
            foreach (Direction direction in Directions.AllDirections())
            {
                BimaruValue neighbourValue = this[center.GetNextPoint(direction)];
                FieldBoundary boundary = center.GetBoundary(direction);
                if (centerValue.IsCompatibleWith(direction, neighbourValue))
                {
                    invalidFieldBoundaries.Remove(boundary);
                }
                else
                {
                    invalidFieldBoundaries.Add(boundary);
                }
            }
        }
        #endregion

        #region Ship count
        private int[] numberOfShipsPerLength;

        public IReadOnlyList<int> NumberOfShipsPerLength
            => Array.AsReadOnly(numberOfShipsPerLength);

        private void UpdateNumberOfShipsPerLength(FieldValueChangedEventArgs<BimaruValue> e)
        {
            foreach(int shipLength in DetectShips(e.Point, e.OriginalValue))
            {
                numberOfShipsPerLength[shipLength]--;
            }

            BimaruValue newValue = GetFieldValueNoCheck(e.Point);
            foreach (int shipLength in DetectShips(e.Point, newValue))
            {
                numberOfShipsPerLength[shipLength]++;
            }
        }

        private IEnumerable<int> DetectShips(GridPoint point, BimaruValue valueAtPoint)
        {
            if (valueAtPoint == BimaruValue.SHIP_SINGLE)
            {
                yield return 1;
                yield break;
            }

            int? shipLength;

            // -1 due to double count of the cross point
            shipLength = GetShipPartLength(point, valueAtPoint, Direction.LEFT) +
                         GetShipPartLength(point, valueAtPoint, Direction.RIGHT) - 1;
            if (shipLength.HasValue)
            {
                yield return shipLength.Value;
            }

            // -1 due to double count of the cross point
            shipLength = GetShipPartLength(point, valueAtPoint, Direction.UP) +
                         GetShipPartLength(point, valueAtPoint, Direction.DOWN) - 1;
            if (shipLength.HasValue)
            {
                yield return shipLength.Value;
            }
        }

        private int? GetShipPartLength(GridPoint startPoint, BimaruValue startValue, Direction direction)
        {
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
        #endregion

        #region Cloning
        public void OverwriteWith(object source)
        {
            if (!(source is BimaruGrid sourceAsGrid))
            {
                throw new ArgumentException("Source is not a BimaruGrid.");
            }

            base.OverwriteWith(sourceAsGrid);

            numberOfNotFullyDeterminedFields = sourceAsGrid.numberOfNotFullyDeterminedFields;

            numberOfUndeterminedFieldsPerColumn = (int[])sourceAsGrid.numberOfUndeterminedFieldsPerColumn.Clone();
            numberOfUndeterminedFieldsPerRow = (int[])sourceAsGrid.numberOfUndeterminedFieldsPerRow.Clone();
            numberOfShipFieldsPerColumn = (int[])sourceAsGrid.numberOfShipFieldsPerColumn.Clone();
            numberOfShipFieldsPerRow = (int[])sourceAsGrid.numberOfShipFieldsPerRow.Clone();
            numberOfShipsPerLength = (int[])sourceAsGrid.numberOfShipsPerLength.Clone();

            invalidFieldBoundaries = new HashSet<FieldBoundary>(sourceAsGrid.invalidFieldBoundaries);
        }

        public virtual object Clone()
        {
            BimaruGrid clonedGrid = new BimaruGrid(NumberOfRows, NumberOfColumns);

            clonedGrid.OverwriteWith(this);

            return clonedGrid;
        }
        #endregion
    }
}
