using Bimaru.Interfaces;
using System.Linq;
using Utility;

namespace Bimaru.SolverUtil
{
    /// <summary>
    /// Fills all UNDETERMINED fields of a row or a column with SHIP_UNDETERMINED
    /// if their number equals the number of missing ships in the row or column.
    /// 
    /// A FieldChangedRule as well as a FullGridRule exist, because a FieldChangedRule
    /// alone might miss the case where a whole row or column is filled with only ships.
    /// However, a FullGridRule is much less efficient than a FieldChangedRule and hence
    /// the FullGridRule is used once and thereafter only the FieldChangedRule.
    /// </summary>
    public class FillRowOrColumnWithShips : IFieldValueChangedRule, ISolverRule
    {
        private static bool AreOnlyShipFieldsMissingRow(IBimaruGame game, int rowIndex)
        {
            return game.Grid.NumberOfUndeterminedFieldsPerRow[rowIndex] == game.NumberOfMissingShipFieldsPerRow(rowIndex);
        }

        private static bool AreOnlyShipFieldsMissingColumn(IBimaruGame game, int columnIndex)
        {
            return game.Grid.NumberOfUndeterminedFieldsPerColumn[columnIndex] == game.NumberOfMissingShipFieldsPerColumn(columnIndex);
        }

        public void FieldValueChanged(IBimaruGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            if (AreOnlyShipFieldsMissingRow(game, e.Point.RowIndex))
            {
                game.Grid.FillUndeterminedFieldsRow(e.Point.RowIndex, BimaruValueConstraint.SHIP);
            }

            if (AreOnlyShipFieldsMissingColumn(game, e.Point.ColumnIndex))
            {
                game.Grid.FillUndeterminedFieldsColumn(e.Point.ColumnIndex, BimaruValueConstraint.SHIP);
            }
        }

        #region Full grid rule
        public bool ShallBeAppliedOnce => true;

        public void Solve(IBimaruGame game)
        {
            foreach (var rowIndex in Enumerable.Range(0, game.Grid.NumberOfRows).Where(i => AreOnlyShipFieldsMissingRow(game, i)))
            {
                game.Grid.FillUndeterminedFieldsRow(rowIndex, BimaruValueConstraint.SHIP);
            }

            foreach (var columnIndex in Enumerable.Range(0, game.Grid.NumberOfColumns).Where(i => AreOnlyShipFieldsMissingColumn(game, i)))
            {
                game.Grid.FillUndeterminedFieldsColumn(columnIndex, BimaruValueConstraint.SHIP);
            }
        }
        #endregion
    }
}
