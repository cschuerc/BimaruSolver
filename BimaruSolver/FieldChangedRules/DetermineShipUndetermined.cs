using BimaruInterfaces;
using Utility;

namespace BimaruSolver
{
    public class DetermineShipUndetermined : IFieldValueChangedRule
    {
        public void FieldValueChanged(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            DetermineFieldValue(game, e.Point);

            foreach (var direction in Directions.GetNonDiagonalDirections())
            {
                DetermineFieldValue(game, e.Point.GetNextPoint(direction));
            }
        }

        private void DetermineFieldValue(IGame game, GridPoint point)
        {
            if (game.Grid[point] == BimaruValue.SHIP_UNDETERMINED)
            {
                BimaruValue? newValue = GetShipUndeterminedFieldValue(game, point);
                if (newValue.HasValue)
                {
                    game.Grid[point] = newValue.Value;
                }
            }
        }

        private BimaruValue? GetShipUndeterminedFieldValue(IGame game, GridPoint pointShipUndetermined)
        {
            BimaruValue? newValue = null;

            Direction? shipDirection = FindNonDiagonalNeighbour(game, pointShipUndetermined, BimaruValueConstraint.SHIP);
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
            else if (AreAllNonDiagonalNeighboursWater(game, pointShipUndetermined))
            {
                newValue = BimaruValue.SHIP_SINGLE;
            }

            return newValue;
        }

        private Direction? FindNonDiagonalNeighbour(IGame game, GridPoint center, BimaruValueConstraint constraint)
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

        private bool AreAllNonDiagonalNeighboursWater(IGame game, GridPoint center)
        {
            foreach (var direction in Directions.GetNonDiagonalDirections())
            {
                var pointInDirection = center.GetNextPoint(direction);
                if (game.Grid[pointInDirection] != BimaruValue.WATER)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
