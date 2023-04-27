using System.Linq;
using Bimaru.Interface;
using Bimaru.Interface.Game;
using Bimaru.Interface.Solver;
using Utility;

namespace Bimaru.Solver.CombinedRules
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
    public class FillRowOrColumnWithWater : IFieldValueChangedRule, ISolverRule
    {
        private static bool AreTargetShipFieldsSetInRow(IBimaruGame game, int rowIndex)
        {
            return game.NumberOfMissingShipFieldsPerRow(rowIndex) == 0;
        }

        private static bool AreTargetShipFieldsSetInColumn(IBimaruGame game, int columnIndex)
        {
            return game.NumberOfMissingShipFieldsPerColumn(columnIndex) == 0;
        }

        public void FieldValueChanged(IBimaruGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            if (AreTargetShipFieldsSetInRow(game, e.Point.RowIndex))
            {
                game.Grid.FillUndeterminedFieldsRow(e.Point.RowIndex, BimaruValueConstraint.WATER);
            }

            if (AreTargetShipFieldsSetInColumn(game, e.Point.ColumnIndex))
            {
                game.Grid.FillUndeterminedFieldsColumn(e.Point.ColumnIndex, BimaruValueConstraint.WATER);
            }
        }

        #region Full grid rule
        public bool ShallBeAppliedOnce => true;

        public void Solve(IBimaruGame game)
        {
            foreach (var rowIndex in Enumerable.Range(0, game.Grid.NumberOfRows).Where(i => AreTargetShipFieldsSetInRow(game, i)))
            {
                game.Grid.FillUndeterminedFieldsRow(rowIndex, BimaruValueConstraint.WATER);
            }

            foreach (var columnIndex in Enumerable.Range(0, game.Grid.NumberOfColumns).Where(i => AreTargetShipFieldsSetInColumn(game, i)))
            {
                game.Grid.FillUndeterminedFieldsColumn(columnIndex, BimaruValueConstraint.WATER);
            }
        }
        #endregion
    }
}
