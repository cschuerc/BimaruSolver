using BimaruInterfaces;
using BimaruSolver;
using System.Collections.Generic;
using Utility;

namespace BimaruTest
{
    internal class BruteForce : ITrialAndErrorRule
    {
        private static IEnumerable<FieldToChange> CreateSingleChangeList(IGridPoint point, FieldValues newValue)
        {
            return new List<FieldToChange>() { new FieldToChange(point, newValue) };
        }

        private static IGridPoint GetUndeterminedField(IGame game)
        {
            for (int rowIndex = 0; rowIndex < game.Grid.NumRows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < game.Grid.NumColumns; columnIndex++)
                {
                    IGridPoint point = new GridPoint(rowIndex, columnIndex);
                    FieldValues value = game.Grid.GetFieldValue(point);
                    if (value == FieldValues.UNDETERMINED)
                    {
                        return new GridPoint(rowIndex, columnIndex);
                    }
                }
            }

            return null;
        }

        public IEnumerable<IEnumerable<FieldToChange>> GetCompleteChangeTrials(IGame game)
        {
            var allValuesSingleField = new List<IEnumerable<FieldToChange>>();

            IGridPoint point = GetUndeterminedField(game);
            if (point != null)
            {
                allValuesSingleField.Add(CreateSingleChangeList(point, FieldValues.SHIP_CONT_DOWN));
                allValuesSingleField.Add(CreateSingleChangeList(point, FieldValues.SHIP_CONT_LEFT));
                allValuesSingleField.Add(CreateSingleChangeList(point, FieldValues.SHIP_CONT_RIGHT));
                allValuesSingleField.Add(CreateSingleChangeList(point, FieldValues.SHIP_CONT_UP));
                allValuesSingleField.Add(CreateSingleChangeList(point, FieldValues.SHIP_MIDDLE));
                allValuesSingleField.Add(CreateSingleChangeList(point, FieldValues.SHIP_SINGLE));
                allValuesSingleField.Add(CreateSingleChangeList(point, FieldValues.WATER));
            }

            return allValuesSingleField;
        }
    }
}
