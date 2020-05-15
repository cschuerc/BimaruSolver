using BimaruInterfaces;
using Utility;

namespace BimaruSolver
{
    /// <summary>
    /// Determines all neighbours of SHIP_MIDDLE if possible.
    /// </summary>
    public class DetermineShipMiddleNeighbours : IFieldValueChangedRule
    {
        public void FieldValueChanged(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            DetermineNeighbours(game, e.Point);

            foreach (var direction in Directions.GetNonDiagonalDirections())
            {
                DetermineNeighbours(game, e.Point.GetNextPoint(direction));
            }
        }

        private void DetermineNeighbours(IGame game, GridPoint point)
        {
            if (game.Grid[point] == BimaruValue.SHIP_MIDDLE)
            {
                DirectionType? shipDirection = GetShipMiddleDirection(game, point);
                if (shipDirection.HasValue)
                {
                    SetShipMiddleNeighbours(game, point, shipDirection.Value);
                }
            }
        }

        private DirectionType? GetShipMiddleDirection(IGame game, GridPoint pointShipMiddle)
        {
            Direction? shipDirection = FindNonDiagonalNeighbour(game, pointShipMiddle, BimaruValueConstraint.SHIP);
            if (shipDirection.HasValue)
            {
                return shipDirection.Value.GetDirectionType();
            }

            Direction? waterDirection = FindNonDiagonalNeighbour(game, pointShipMiddle, BimaruValueConstraint.WATER);
            if (waterDirection.HasValue)
            {
                DirectionType type = waterDirection.Value.GetDirectionType();
                if (type == DirectionType.COLUMN)
                {
                    return DirectionType.ROW;
                }
                else if (type == DirectionType.ROW)
                {
                    return DirectionType.COLUMN;
                }
            }

            return null;
        }

        private void SetShipMiddleNeighbours(IGame game, GridPoint pointShipMiddle, DirectionType directionShipMiddle)
        {
            foreach (var direction in Directions.GetAllDirections())
            {
                var constraint = direction.GetDirectionType() == directionShipMiddle ?
                    BimaruValueConstraint.SHIP :
                    BimaruValueConstraint.WATER;

                // Skip set if constraint already satisfied
                var pointInDirection = pointShipMiddle.GetNextPoint(direction);
                if (!constraint.IsSatisfiedBy(game.Grid[pointInDirection]))
                {
                    BimaruValue valueToSet = constraint.GetRepresentativeValue();
                    game.Grid[pointInDirection] = valueToSet;
                }
            }
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
    }
}
