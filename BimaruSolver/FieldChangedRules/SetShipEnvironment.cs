using BimaruInterfaces;
using Utility;

namespace BimaruSolver
{
    /// <summary>
    /// Sets the surrounding of a changed field if possible.
    /// For example, a SHIP_SINGLE is surrounded by WATER.
    /// </summary>
    public class SetShipEnvironment : IFieldValueChangedRule
    {
        /// <inheritdoc/>
        public void FieldValueChanged(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            BimaruValue newValue = game.Grid[e.Point];
            foreach (Direction direction in Directions.AllDirections())
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
