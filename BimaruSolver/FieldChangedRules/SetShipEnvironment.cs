using BimaruInterfaces;
using Utility;

namespace BimaruSolver
{
    /// <summary>
    /// Sets the surrounding of a changed field if possible.
    /// For example, a SHIP_SINGLE is surrounded by WATER.
    /// </summary>
    public class SetShipEnvironment : IFieldChangedRule
    {
        /// <inheritdoc/>
        public void FieldValueChanged(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            BimaruValue newValue = game.Grid.GetFieldValue(e.Point);
            foreach (Direction direction in DirectionExtensions.AllDirections())
            {
                BimaruValueConstraint constraintInDirection = newValue.GetConstraint(direction);

                GridPoint pointInDirection = e.Point.GetNextPoint(direction);
                BimaruValue valueInDirection = game.Grid.GetFieldValue(pointInDirection);

                // Skip set if constraint already satisfied
                if (!constraintInDirection.IsSatisfiedBy(valueInDirection))
                {
                    BimaruValue valueToSet = constraintInDirection.GetRepresentativeValue();
                    game.Grid.SetFieldValue(pointInDirection, valueToSet);
                }
            }
        }
    }
}
