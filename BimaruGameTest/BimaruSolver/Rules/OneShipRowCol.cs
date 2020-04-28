using BimaruInterfaces;
using Utility;

namespace BimaruSolver
{
    /// <summary>
    /// Fills the field with a row and column tally of 1 each by SHIP_SINGLE.
    /// Note that this solver does not correctly solve Bimaru games in general.
    /// However, it is quite useful for testing.
    /// </summary>
    internal class OneShipRowCol : IFullGridRule
    {
        /// <inheritdoc/>
        public bool ShallBeAppliedOnce => false;

        /// <inheritdoc/>
        public void Solve(IGame game)
        {
            int rowIndex = game.RowTally.Length - 1;
            while (rowIndex >= 0 && game.RowTally[rowIndex] != 1)
            {
                rowIndex--;
            }

            int columnIndex = game.ColumnTally.Length - 1;
            while (columnIndex >= 0 && game.ColumnTally[columnIndex] != 1)
            {
                columnIndex--;
            }

            if (rowIndex >= 0 && columnIndex >= 0)
            {
                game.Grid.SetFieldValue(new GridPoint(rowIndex, columnIndex), BimaruValue.SHIP_SINGLE);
            }
        }
    }
}
