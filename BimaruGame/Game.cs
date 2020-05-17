using Bimaru.Interfaces;
using System;
using System.Linq;

namespace Bimaru.GameUtil
{
    [Serializable]
    public class Game : IGame
    {
        public Game(IGridTally targetNumberOfShipFieldsPerRow,
            IGridTally targetNumberOfShipFieldsPerColumn,
            IShipTarget targetNumberOfShipsPerLength,
            IBimaruGrid grid)
        {
            TargetNumberOfShipFieldsPerRow = targetNumberOfShipFieldsPerRow;
            TargetNumberOfShipFieldsPerColumn = targetNumberOfShipFieldsPerColumn;
            TargetNumberOfShipsPerLength = targetNumberOfShipsPerLength;
            Grid = grid;

            if (targetNumberOfShipFieldsPerRow.Length != grid.NumberOfRows ||
                targetNumberOfShipFieldsPerColumn.Length != grid.NumberOfColumns)
            {
                throw new ArgumentOutOfRangeException("The number of rows/columns does not agree between tallies and grid.");
            }
        }

        #region Target number of ship fields
        private IGridTally targetNumberOfShipFieldsPerRow;

        public IGridTally TargetNumberOfShipFieldsPerRow
        {
            get
            {
                return targetNumberOfShipFieldsPerRow;
            }

            private set
            {
                targetNumberOfShipFieldsPerRow = value ?? throw new ArgumentNullException("TargetNumberOfShipFieldsRow");
            }
        }

        private Satisfiability TargetShipFieldsRowSatisfiability
        {
            get
            {
                return TargetNumberOfShipFieldsPerRow.GetSatisfiability(
                    Grid.NumberOfShipFieldsPerRow,
                    Grid.NumberOfUndeterminedFieldsPerRow);
            }
        }


        private IGridTally targetNumberOfShipFieldsPerColumn;

        public IGridTally TargetNumberOfShipFieldsPerColumn
        {
            get
            {
                return targetNumberOfShipFieldsPerColumn;
            }

            private set
            {
                targetNumberOfShipFieldsPerColumn = value ?? throw new ArgumentNullException("TargetNumberOfShipFieldsColumn");
            }
        }

        private Satisfiability TargetShipFieldsColumnSatisfiability
        {
            get
            {
                return TargetNumberOfShipFieldsPerColumn.GetSatisfiability(
                    Grid.NumberOfShipFieldsPerColumn,
                    Grid.NumberOfUndeterminedFieldsPerColumn);
            }
        }
        #endregion

        #region Target number of ships
        private IShipTarget targetNumberOfShipsPerLength;

        public IShipTarget TargetNumberOfShipsPerLength
        {
            get
            {
                return targetNumberOfShipsPerLength;
            }

            private set
            {
                targetNumberOfShipsPerLength = value ?? throw new ArgumentNullException("TargetNumberOfShips");
            }
        }

        private Satisfiability TargetNumberOfShipsSatisfiability
        {
            get
            {
                return TargetNumberOfShipsPerLength.GetSatisfiability(Grid.NumberOfShipsPerLength);
            }
        }
        #endregion

        #region Grid
        private IBimaruGrid grid;

        public IBimaruGrid Grid
        {
            get
            {
                return grid;
            }

            private set
            {
                grid = value ?? throw new ArgumentNullException("Grid");
            }
        }

        public int NumberOfMissingShipFieldsPerRow(int index)
        {
            return targetNumberOfShipFieldsPerRow[index] - grid.NumberOfShipFieldsPerRow[index];
        }

        public int NumberOfMissingShipFieldsPerColumn(int index)
        {
            return targetNumberOfShipFieldsPerColumn[index] - grid.NumberOfShipFieldsPerColumn[index];
        }

        public int? LengthOfLongestMissingShip
        {
            get
            {
                return TargetNumberOfShipsPerLength.LengthOfLongestMissingShip(Grid.NumberOfShipsPerLength);
            }
        }
        #endregion

        #region Game properties
        public bool IsUnsolvable
        {
            get
            {
                return TargetNumberOfShipFieldsPerRow.Any(t => t > Grid.NumberOfColumns) ||
                    TargetNumberOfShipFieldsPerRow.Total != TargetNumberOfShipsPerLength.TotalShipFields ||
                    TargetNumberOfShipFieldsPerColumn.Any(t => t > Grid.NumberOfRows) ||
                    TargetNumberOfShipFieldsPerColumn.Total != TargetNumberOfShipsPerLength.TotalShipFields ||
                    TargetNumberOfShipsPerLength.LongestShipLength > Math.Max(Grid.NumberOfColumns, Grid.NumberOfRows);
            }
        }

        public bool IsValid
        {
            get
            {
                return !IsUnsolvable &&
                    Grid.IsValid &&
                    TargetShipFieldsRowSatisfiability != Satisfiability.VIOLATED &&
                    TargetShipFieldsColumnSatisfiability != Satisfiability.VIOLATED &&
                    TargetNumberOfShipsSatisfiability != Satisfiability.VIOLATED;
            }
        }

        public bool IsSolved
        {
            get
            {
                return !IsUnsolvable &&
                    Grid.IsValid &&
                    Grid.IsFullyDetermined &&
                    TargetShipFieldsRowSatisfiability == Satisfiability.SATISFIED &&
                    TargetShipFieldsColumnSatisfiability == Satisfiability.SATISFIED &&
                    TargetNumberOfShipsSatisfiability == Satisfiability.SATISFIED;
            }
        }
        #endregion
    }
}
