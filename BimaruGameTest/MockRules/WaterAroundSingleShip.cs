using BimaruInterfaces;
using BimaruSolver;
using System;
using Utility;

namespace BimaruTest
{
    internal class WaterAroundSingleShip : IFieldChangedRule
    {
        public void FieldValueChanged(IGame game, FieldValueChangedEventArgs<FieldValues> e)
        {
            if (game.Grid.GetFieldValue(e.Point) == FieldValues.SHIP_SINGLE)
            {
                foreach (Directions direction in Enum.GetValues(typeof(Directions)))
                {
                    game.Grid.SetFieldValue(e.Point.GetNextPoint(direction), FieldValues.WATER);
                }
            }
        }
    }
}
