using Bimaru.Interface;
using Bimaru.Interface.Game;
using Bimaru.Interface.Solver;

namespace Bimaru.Solver.FieldChangedRules
{
    /// <summary>
    /// Sets the surroundings of a ship field.
    /// For example, a SHIP_SINGLE is surrounded by WATER.
    /// </summary>
    public class SetShipEnvironment : IFieldValueChangedRule
    {
        public void FieldValueChanged(IBimaruGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            var newValue = game.Grid[e.Point];
            foreach (var direction in Directions.GetAllDirections())
            {
                var constraintInDirection = newValue.GetConstraint(direction);

                var pointInDirection = e.Point.GetNextPoint(direction);
                var valueInDirection = game.Grid[pointInDirection];

                // Skip set if constraint already satisfied
                if (constraintInDirection.IsSatisfiedBy(valueInDirection))
                {
                    continue;
                }

                var valueToSet = constraintInDirection.GetRepresentativeValue();
                game.Grid[pointInDirection] = valueToSet;
            }
        }
    }
}
