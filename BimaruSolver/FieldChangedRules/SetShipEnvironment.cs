using Bimaru.Interfaces;
using Utility;

namespace Bimaru.SolverUtil
{
    /// <summary>
    /// Sets the surroundings of a ship field.
    /// For example, a SHIP_SINGLE is surrounded by WATER.
    /// </summary>
    public class SetShipEnvironment : IFieldValueChangedRule
    {
        public void FieldValueChanged(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            BimaruValue newValue = game.Grid[e.Point];
            foreach (Direction direction in Directions.GetAllDirections())
            {
                BimaruValueConstraint constraintInDirection = newValue.GetConstraint(direction);

                GridPoint pointInDirection = e.Point.GetNextPoint(direction);
                BimaruValue valueInDirection = game.Grid[pointInDirection];

                // Skip set if constraint already satisfied
                if (!constraintInDirection.IsSatisfiedBy(valueInDirection))
                {
                    BimaruValue valueToSet = constraintInDirection.GetRepresentativeValue();
                    game.Grid[pointInDirection] = valueToSet;
                }
            }
        }
    }
}
