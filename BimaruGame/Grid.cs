using Bimaru.General;
using System;
using System.Collections.Generic;

namespace Bimaru
{
    /// <summary>
    /// Bimaru grid implementation
    /// </summary>
    public class Grid : GridBase<FieldValues>, IGrid
    {
        /// <summary>
        /// Instantiates a bimaru grid with corresponding number of rows/columns
        /// </summary>
        /// <param name="numRows"> Positive number of rows </param>
        /// <param name="numColumns"> Positive number of columns </param>
        public Grid(int numRows, int numColumns)
            : base(numRows, numColumns, FieldValues.UNDETERMINED)
        {
            _numEmptyFieldsRow = new int[numRows].InitValues(numColumns);
            _numShipFieldsRow = new int[numRows].InitValues(0);

            _numEmptyFieldsColumn = new int[numColumns].InitValues(numRows);
            _numShipFieldsColumn = new int[numColumns].InitValues(0);

            int maxShipLength = System.Math.Max(numRows, numColumns);
            _numShipsOfLength = new int[maxShipLength + 1].InitValues(0); // +1 due to ship length of zero

            _numNotFullyDeterminedFields = numRows * numColumns;

            _incompatibleBoundaries = new HashSet<IFieldBoundary>();
        }

        /// <inheritdoc/>
        public override FieldValues GetFieldValue(IGridPoint point)
        {
            if (!IsPointInGrid(point))
            {
                return FieldValues.WATER;
            }

            return GetFieldValueNoCheck(point);
        }

        /// <inheritdoc/>
        public override object Clone()
        {
            Grid clonedGrid = (Grid)base.Clone();

            clonedGrid._numEmptyFieldsColumn = (int[])this._numEmptyFieldsColumn.Clone();
            clonedGrid._numEmptyFieldsRow = (int[])this._numEmptyFieldsRow.Clone();
            clonedGrid._numShipFieldsColumn = (int[])this._numShipFieldsColumn.Clone();
            clonedGrid._numShipFieldsRow = (int[])this._numShipFieldsRow.Clone();
            clonedGrid._numShipsOfLength = (int[])this._numShipsOfLength.Clone();

            clonedGrid._incompatibleBoundaries = new HashSet<IFieldBoundary>(_incompatibleBoundaries);

            return clonedGrid;
        }

        
        private int[] _numShipFieldsRow;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumShipFieldsRow
            => System.Array.AsReadOnly(_numShipFieldsRow);


        private int[] _numShipFieldsColumn;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumShipFieldsColumn
            => System.Array.AsReadOnly(_numShipFieldsColumn);


        private int[] _numEmptyFieldsRow;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumEmptyFieldsRow
            => System.Array.AsReadOnly(_numEmptyFieldsRow);


        private int[] _numEmptyFieldsColumn;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumEmptyFieldsColumn
            => System.Array.AsReadOnly(_numEmptyFieldsColumn);


        private int[] _numShipsOfLength;

        /// <inheritdoc/>
        public IReadOnlyList<int> GetNumShips
            => System.Array.AsReadOnly(_numShipsOfLength);


        private HashSet<IFieldBoundary> _incompatibleBoundaries;

        /// <inheritdoc/>
        public bool IsValid => _incompatibleBoundaries.Count == 0;


        private int _numNotFullyDeterminedFields;

        /// <inheritdoc/>
        public bool IsFullyDetermined => (_numNotFullyDeterminedFields == 0);

        /// <summary>
        /// Length of a ship part starting at the given point and extending into the given direction
        /// </summary>
        /// <param name="startPoint"> Starting point </param>
        /// <param name="direction"> Direction in which the ship is detected</param>
        /// <returns> Length of the ship part </returns>
        private int? GetShipPartLength(IGridPoint startPoint, Directions direction)
        {
            if (direction.IsDiagonal())
            {
                return null;
            }

            int shipLength = 0;

            IGridPoint currentPoint = startPoint;
            if (GetFieldValue(currentPoint) == direction.GetStartFieldValue())
            {
                shipLength++;
                currentPoint = currentPoint.GetNextPoint(direction);
            }
            
            while (GetFieldValue(currentPoint) == FieldValues.SHIP_MIDDLE)
            {
                shipLength++;
                currentPoint = currentPoint.GetNextPoint(direction);
            }

            if (GetFieldValue(currentPoint) == direction.GetEndFieldValue())
            {
                shipLength++;
                return shipLength;
            }

            return null;
        }

        /// <summary>
        /// Get the length of a complete ship that crosses the point
        /// </summary>
        /// <param name="crossPoint"> Point to cross </param>
        /// <returns> Ship length of a complete ship crossing the point</returns>
        private int? GetShipLength(IGridPoint crossPoint)
        {
            if (GetFieldValue(crossPoint) == FieldValues.SHIP_SINGLE)
            {
                return 1;
            }

            int? shipLength = GetShipPartLength(crossPoint, Directions.LEFT) +
                              GetShipPartLength(crossPoint, Directions.RIGHT) - 1;

            if (shipLength  == null)
            {
                shipLength = GetShipPartLength(crossPoint, Directions.UP) +
                             GetShipPartLength(crossPoint, Directions.DOWN) - 1;
            }

            return shipLength;
        }

        private void UpdateInvalidBoundaries(IGridPoint center)
        {
            FieldValues centerValue = GetFieldValueNoCheck(center);
            foreach (Directions direction in Enum.GetValues(typeof(Directions)))
            {
                IFieldBoundary boundary = center.GetBoundary(direction);
                bool isCompatibleOld = !_incompatibleBoundaries.Contains(boundary);

                bool isCompatibleNew =
                    centerValue.IsCompatibleWith(direction, GetFieldValue(center.GetNextPoint(direction)));

                if (!isCompatibleOld && isCompatibleNew)
                {
                    _incompatibleBoundaries.Remove(boundary);
                }
                else if (isCompatibleOld && !isCompatibleNew)
                {
                    _incompatibleBoundaries.Add(boundary);
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnBeforeFieldValueSet(IGridPoint point)
        {
            var value = GetFieldValue(point);
            if (value.IsShip())
            {
                _numShipFieldsRow[point.RowIndex]--;
                _numShipFieldsColumn[point.ColumnIndex]--;
            }
            else if (value == FieldValues.UNDETERMINED)
            {
                _numEmptyFieldsRow[point.RowIndex]--;
                _numEmptyFieldsColumn[point.ColumnIndex]--;
            }

            if (!value.IsFullyDetermined())
            {
                _numNotFullyDeterminedFields--;
            }

            int? shipLength = GetShipLength(point);
            if (shipLength != null)
            {
                _numShipsOfLength[shipLength.Value]--;
            }
        }

        /// <inheritdoc/>
        protected override void OnAfterFieldValueSet(IGridPoint point)
        {
            var value = GetFieldValue(point);
            if (value.IsShip())
            {
                _numShipFieldsRow[point.RowIndex]++;
                _numShipFieldsColumn[point.ColumnIndex]++;
            }
            else if (value == FieldValues.UNDETERMINED)
            {
                _numEmptyFieldsRow[point.RowIndex]++;
                _numEmptyFieldsColumn[point.ColumnIndex]++;
            }

            if (!value.IsFullyDetermined())
            {
                _numNotFullyDeterminedFields++;
            }

            UpdateInvalidBoundaries(point);

            int? shipLength = GetShipLength(point);
            if (shipLength != null)
            {
                _numShipsOfLength[shipLength.Value]++;
            }
        }
    }
}
