using BimaruInterfaces;
using BimaruSolver;
using Utility;

namespace BimaruTest
{
    internal class OneShipRowCol : IFullGridRule
    {
        public void Solve(IGame game)
        {
            int rowIndex = 0;
            while (rowIndex < game.RowTally.Length && game.RowTally[rowIndex] != 1)
            {
                rowIndex++;
            }

            int columnIndex = 0;
            while (columnIndex < game.ColumnTally.Length && game.ColumnTally[columnIndex] != 1)
            {
                columnIndex++;
            }

            if (rowIndex < game.RowTally.Length && columnIndex < game.ColumnTally.Length)
            {
                game.Grid.SetFieldValue(new GridPoint(rowIndex, columnIndex), FieldValues.SHIP_SINGLE);
            }
        }
    }
}
