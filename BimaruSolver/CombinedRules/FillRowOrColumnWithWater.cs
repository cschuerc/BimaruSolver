using BimaruInterfaces;
using System.Linq;
using Utility;

namespace BimaruSolver
{
    /// <summary>
    /// Fills all UNDETERMINED fields of a row or a column with WATER
    /// if the row or column tally is already satisfied.
    /// 
    /// A FieldChangedRule as well as a FullGridRule exist, because a FieldChangedRule
    /// alone might miss the case where a tally contained a 0. However, a FullGridRule
    /// is much less efficient than a FieldChangedRule and hence the FullGridRule is
    /// used once and thereafter only the FieldChangedRule.
    /// </summary>
    public class FillRowOrColumnWithWater : IFieldChangedRule, IFullGridRule
    {
        private static bool AreAllShipsUsedInRow(IGame game, int rowIndex)
        {
            return game.MissingShipFieldsRow(rowIndex) == 0;
        }

        private static bool AreAllShipsUsedInColumn(IGame game, int columnIndex)
        {
            return game.MissingShipFieldsColumn(columnIndex) == 0;
        }

        /// <inheritdoc/>
        public void FieldValueChanged(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            if (AreAllShipsUsedInRow(game, e.Point.RowIndex))
            {
                game.Grid.FillUndeterminedFieldsRow(e.Point.RowIndex, BimaruValueConstraint.WATER);
            }

            if (AreAllShipsUsedInColumn(game, e.Point.ColumnIndex))
            {
                game.Grid.FillUndeterminedFieldsColumn(e.Point.ColumnIndex, BimaruValueConstraint.WATER);
            }
        }

        #region Full grid rule
        /// <inheritdoc/>
        public bool ShallBeAppliedOnce => true;

        /// <inheritdoc/>
        public void Solve(IGame game)
        {
            foreach (int rowIndex in Enumerable.Range(0, game.Grid.NumRows).Where(i => AreAllShipsUsedInRow(game, i)))
            {
                game.Grid.FillUndeterminedFieldsRow(rowIndex, BimaruValueConstraint.WATER);
            }

            foreach (int columnIndex in Enumerable.Range(0, game.Grid.NumColumns).Where(i => AreAllShipsUsedInColumn(game, i)))
            {
                game.Grid.FillUndeterminedFieldsColumn(columnIndex, BimaruValueConstraint.WATER);
            }
        }
        #endregion
    }
}
