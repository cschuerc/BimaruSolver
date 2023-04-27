using Bimaru.Interfaces;
using Utility;

namespace Bimaru.Solver.FieldChangedRules
{
    /// <summary>
    /// Determines all neighbors of SHIP_MIDDLE if possible.
    /// </summary>
    public class DetermineShipMiddleNeighbors : IFieldValueChangedRule
    {
        public void FieldValueChanged(IBimaruGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            DetermineNeighbors(game, e.Point);

            foreach (var direction in Directions.GetNonDiagonalDirections())
            {
                DetermineNeighbors(game, e.Point.GetNextPoint(direction));
            }
        }

        private static void DetermineNeighbors(IBimaruGame game, GridPoint point)
        {
            if (game.Grid[point] != BimaruValue.SHIP_MIDDLE)
            {
                return;
            }

            var shipDirection = GetShipMiddleDirection(game, point);
            if (shipDirection.HasValue)
            {
                SetShipMiddleNeighbors(game, point, shipDirection.Value);
            }
        }

        private static DirectionType? GetShipMiddleDirection(IBimaruGame game, GridPoint pointShipMiddle)
        {
            var shipDirection = FindNonDiagonalNeighbor(game, pointShipMiddle, BimaruValueConstraint.SHIP);
            if (shipDirection.HasValue)
            {
                return shipDirection.Value.GetDirectionType();
            }

            var waterDirection = FindNonDiagonalNeighbor(game, pointShipMiddle, BimaruValueConstraint.WATER);
            if (!waterDirection.HasValue)
            {
                return null;
            }

            var type = waterDirection.Value.GetDirectionType();

            switch (type)
            {
                case DirectionType.COLUMN:
                    return DirectionType.ROW;
                case DirectionType.ROW:
                    return DirectionType.COLUMN;
                default:
                    return null;
            }
        }

        private static void SetShipMiddleNeighbors(IBimaruGame game, GridPoint pointShipMiddle, DirectionType directionShipMiddle)
        {
            foreach (var direction in Directions.GetAllDirections())
            {
                var constraint = direction.GetDirectionType() == directionShipMiddle ?
                    BimaruValueConstraint.SHIP :
                    BimaruValueConstraint.WATER;

                // Skip set if constraint already satisfied
                var pointInDirection = pointShipMiddle.GetNextPoint(direction);
                if (constraint.IsSatisfiedBy(game.Grid[pointInDirection]))
                {
                    continue;
                }

                var valueToSet = constraint.GetRepresentativeValue();
                game.Grid[pointInDirection] = valueToSet;
            }
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
    }
}
