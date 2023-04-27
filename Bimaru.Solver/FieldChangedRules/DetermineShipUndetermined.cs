using System.Linq;
using Bimaru.Interface.Game;
using Bimaru.Interface.Solver;
using Bimaru.Interface.Utility;

namespace Bimaru.Solver.FieldChangedRules
{
    public class DetermineShipUndetermined : IFieldValueChangedRule
    {
        public void FieldValueChanged(IBimaruGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            DetermineFieldValue(game, e.Point);

            foreach (var direction in Directions.GetNonDiagonalDirections())
            {
                DetermineFieldValue(game, e.Point.GetNextPoint(direction));
            }
        }

        private static void DetermineFieldValue(IBimaruGame game, GridPoint point)
        {
            if (game.Grid[point] == BimaruValue.SHIP_UNDETERMINED)
            {
                var newValue = GetShipUndeterminedFieldValue(game, point);
                if (newValue.HasValue)
                {
                    game.Grid[point] = newValue.Value;
                }
            }
        }

        private static BimaruValue? GetShipUndeterminedFieldValue(IBimaruGame game, GridPoint pointShipUndetermined)
        {
            BimaruValue? newValue = null;

            var shipDirection = FindNonDiagonalNeighbor(game, pointShipUndetermined, BimaruValueConstraint.SHIP);
            if (shipDirection.HasValue)
            {
                var oppositePoint = pointShipUndetermined.GetNextPoint(shipDirection.Value.GetOpposite());
                var valueOppositePoint = game.Grid[oppositePoint];

                if (valueOppositePoint == BimaruValue.WATER)
                {
                    newValue = shipDirection.Value.GetFirstShipValue();
                }
                else if (valueOppositePoint.IsShip())
                {
                    newValue = BimaruValue.SHIP_MIDDLE;
                }
            }
            else if (AreAllNonDiagonalNeighborsWater(game, pointShipUndetermined))
            {
                newValue = BimaruValue.SHIP_SINGLE;
            }

            return newValue;
        }

        private static Direction? FindNonDiagonalNeighbor(IBimaruGame game, GridPoint center, BimaruValueConstraint constraint)
        {
            foreach (var direction in Directions.GetNonDiagonalDirections())
            {
                var pointInDirection = center.GetNextPoint(direction);
                if (constraint.IsSatisfiedBy(game.Grid[pointInDirection]))
                {
                    return direction;
                }
            }

            return null;
        }

        private static bool AreAllNonDiagonalNeighborsWater(IBimaruGame game, GridPoint center)
        {
            return Directions.GetNonDiagonalDirections().Select(center.GetNextPoint).All(pointInDirection => game.Grid[pointInDirection] == BimaruValue.WATER);
        }
    }
}
