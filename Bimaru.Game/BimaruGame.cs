using System;
using System.Linq;
using Bimaru.Interface.Game;
using Bimaru.Interface.Utility;

namespace Bimaru.Game
{
    [Serializable]
    public class BimaruGame : IBimaruGame
    {
        public BimaruGame(IGridTally targetNumberOfShipFieldsPerRow,
            IGridTally targetNumberOfShipFieldsPerColumn,
            IShipTarget targetNumberOfShipsPerLength,
            IBimaruGrid grid)
        {
            TargetNumberOfShipFieldsPerRow = targetNumberOfShipFieldsPerRow;
            TargetNumberOfShipFieldsPerColumn = targetNumberOfShipFieldsPerColumn;
            TargetNumberOfShipsPerLength = targetNumberOfShipsPerLength;
            Grid = grid;

            if (targetNumberOfShipFieldsPerRow.Length != grid.NumberOfRows)
            {
                throw new ArgumentOutOfRangeException(nameof(targetNumberOfShipFieldsPerRow),
                    targetNumberOfShipFieldsPerRow,
                    "The number of rows does not agree between tallies and grid.");
            }

            if (targetNumberOfShipFieldsPerColumn.Length != grid.NumberOfColumns)
            {
                throw new ArgumentOutOfRangeException(nameof(targetNumberOfShipFieldsPerColumn),
                    targetNumberOfShipFieldsPerColumn,
                    "The number of columns does not agree between tallies and grid.");
            }
        }

        #region Target number of ship fields
        private IGridTally targetNumberOfShipFieldsPerRow;

        public IGridTally TargetNumberOfShipFieldsPerRow
        {
            get => targetNumberOfShipFieldsPerRow;

            private set => targetNumberOfShipFieldsPerRow = value ?? throw new ArgumentNullException(nameof(value));
        }

        private Satisfiability TargetShipFieldsRowSatisfiability =>
            TargetNumberOfShipFieldsPerRow.GetSatisfiability(
                Grid.NumberOfShipFieldsPerRow,
                Grid.NumberOfUndeterminedFieldsPerRow);


        private IGridTally targetNumberOfShipFieldsPerColumn;

        public IGridTally TargetNumberOfShipFieldsPerColumn
        {
            get => targetNumberOfShipFieldsPerColumn;

            private set => targetNumberOfShipFieldsPerColumn = value ?? throw new ArgumentNullException(nameof(value));
        }

        private Satisfiability TargetShipFieldsColumnSatisfiability =>
            TargetNumberOfShipFieldsPerColumn.GetSatisfiability(
                Grid.NumberOfShipFieldsPerColumn,
                Grid.NumberOfUndeterminedFieldsPerColumn);

        #endregion

        #region Target number of ships
        private IShipTarget targetNumberOfShipsPerLength;

        public IShipTarget TargetNumberOfShipsPerLength
        {
            get => targetNumberOfShipsPerLength;
            
            private set => targetNumberOfShipsPerLength = value ?? throw new ArgumentNullException(nameof(value));
        }

        private Satisfiability TargetNumberOfShipsSatisfiability => TargetNumberOfShipsPerLength.GetSatisfiability(Grid.NumberOfShipsPerLength);

        #endregion

        #region Grid
        private IBimaruGrid grid;

        public IBimaruGrid Grid
        {
            get => grid;

            private set => grid = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int NumberOfMissingShipFieldsPerRow(int rowIndex)
        {
            return targetNumberOfShipFieldsPerRow[rowIndex] - grid.NumberOfShipFieldsPerRow[rowIndex];
        }

        public int NumberOfMissingShipFieldsPerColumn(int columnIndex)
        {
            return targetNumberOfShipFieldsPerColumn[columnIndex] - grid.NumberOfShipFieldsPerColumn[columnIndex];
        }

        public int? LengthOfLongestMissingShip => TargetNumberOfShipsPerLength.LengthOfLongestMissingShip(Grid.NumberOfShipsPerLength);

        #endregion

        #region BimaruGame properties
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

        public bool IsValid =>
            !IsUnsolvable &&
            Grid.IsValid &&
            TargetShipFieldsRowSatisfiability != Satisfiability.VIOLATED &&
            TargetShipFieldsColumnSatisfiability != Satisfiability.VIOLATED &&
            TargetNumberOfShipsSatisfiability != Satisfiability.VIOLATED;

        public bool IsSolved =>
            !IsUnsolvable &&
            Grid.IsValid &&
            Grid.IsFullyDetermined &&
            TargetShipFieldsRowSatisfiability == Satisfiability.SATISFIED &&
            TargetShipFieldsColumnSatisfiability == Satisfiability.SATISFIED &&
            TargetNumberOfShipsSatisfiability == Satisfiability.SATISFIED;

        #endregion
    }
}
