using BimaruInterfaces;
using Utility;

namespace BimaruSolver
{
    /// <summary>
    /// Determine SHIP_UNDETERMINED and the direction of SHIP_MIDDLE if possible
    /// </summary>
    public class DetermineShipFields : IFieldChangedRule
    {
        private Direction? FindNonDiagonalNeighbour(IGame game, GridPoint center, BimaruValueConstraint constraint)
        {
            foreach (var direction in DirectionExtensions.AllNonDiagonalDirections())
            {
                var pointInDirection = center.GetNextPoint(direction);
                if (constraint.IsSatisfiedBy(game.Grid.GetFieldValue(pointInDirection)))
                {
                    return direction;
                }
            }

            return null;
        }

        #region Ship Undetermined
        private bool AreAllNeighboursWater(IGame game, GridPoint center)
        {
            foreach (var direction in DirectionExtensions.AllNonDiagonalDirections())
            {
                var pointInDirection = center.GetNextPoint(direction);
                if (game.Grid.GetFieldValue(pointInDirection) != BimaruValue.WATER)
                {
                    return false;
                }
            }

            return true;
        }

        private BimaruValue? GetShipUndeterminedValue(IGame game, GridPoint shipUndeterminedpoint)
        {
            BimaruValue? newValue = null;

            Direction? shipDirection = FindNonDiagonalNeighbour(game, shipUndeterminedpoint, BimaruValueConstraint.SHIP);
            if (shipDirection.HasValue)
            {
                var oppositePoint = shipUndeterminedpoint.GetNextPoint(shipDirection.Value.GetOpposite());
                var oppositeValue = game.Grid.GetFieldValue(oppositePoint);

                if (oppositeValue == BimaruValue.WATER)
                {
                    newValue = shipDirection.Value.GetFirstShipValue();
                }
                else if (oppositeValue.IsShip())
                {
                    newValue = BimaruValue.SHIP_MIDDLE;
                }
            }
            else if (AreAllNeighboursWater(game, shipUndeterminedpoint))
            {
                newValue = BimaruValue.SHIP_SINGLE;
            }

            return newValue;
        }

        private void DetermineShipUndetermined(IGame game, GridPoint shipUndetermined)
        {
            BimaruValue? newValue = GetShipUndeterminedValue(game, shipUndetermined);
            if (newValue.HasValue)
            {
                game.Grid.SetFieldValue(shipUndetermined, newValue.Value);
            }
        }
        #endregion

        #region Ship Middle
        private DirectionType? GetShipDirection(IGame game, GridPoint shipMiddlePoint)
        {
            Direction? shipDirection = FindNonDiagonalNeighbour(game, shipMiddlePoint, BimaruValueConstraint.SHIP);
            if (shipDirection.HasValue)
            {
                return shipDirection.Value.GetDirectionType();
            }

            Direction? waterDirection = FindNonDiagonalNeighbour(game, shipMiddlePoint, BimaruValueConstraint.WATER);
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

        private void SetShipMiddleNeighbours(IGame game, GridPoint shipMiddlePoint, DirectionType shipMiddleDirection)
        {
            foreach (var direction in DirectionExtensions.AllNonDiagonalDirections())
            {
                var constraint = direction.GetDirectionType() == shipMiddleDirection ?
                    BimaruValueConstraint.SHIP :
                    BimaruValueConstraint.WATER;

                // Skip set if constraint already satisfied
                var pointInDirection = shipMiddlePoint.GetNextPoint(direction);
                if (!constraint.IsSatisfiedBy(game.Grid.GetFieldValue(pointInDirection)))
                {
                    BimaruValue valueToSet = constraint.GetRepresentativeValue();
                    game.Grid.SetFieldValue(pointInDirection, valueToSet);
                }
            }
        }

        private void DetermineShipMiddle(IGame game, GridPoint shipMiddlePoint)
        {
            DirectionType? shipDirection = GetShipDirection(game, shipMiddlePoint);
            if (shipDirection.HasValue)
            {
                SetShipMiddleNeighbours(game, shipMiddlePoint, shipDirection.Value);
            }
        }
        #endregion

        private void DetermineShip(IGame game, GridPoint point)
        {
            BimaruValue value = game.Grid.GetFieldValue(point);
            if (value == BimaruValue.SHIP_UNDETERMINED)
            {
                DetermineShipUndetermined(game, point);
            }
            else if (value == BimaruValue.SHIP_MIDDLE)
            {
                DetermineShipMiddle(game, point);
            }
        }

        /// <inheritdoc/>
        public void FieldValueChanged(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            DetermineShip(game, e.Point);

            foreach (var direction in DirectionExtensions.AllNonDiagonalDirections())
            {
                DetermineShip(game, e.Point.GetNextPoint(direction));
            }
        }
    }
}
