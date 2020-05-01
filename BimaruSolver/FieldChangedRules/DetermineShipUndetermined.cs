using BimaruInterfaces;
using Utility;

namespace BimaruSolver
{
    /// <summary>
    /// Determine SHIP_UNDETERMINED if possible
    /// </summary>
    public class DetermineShipUndetermined : IFieldChangedRule
    {
        private Direction? FindNeighbourShip(IGame game, GridPoint center)
        {
            foreach (var direction in DirectionExtensions.AllNonDiagonalDirections())
            {
                var pointInDirection = center.GetNextPoint(direction);
                if (game.Grid.GetFieldValue(pointInDirection).IsShip())
                {
                    return direction;
                }
            }

            return null;
        }

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

        private BimaruValue? GetNewShipValue(IGame game, GridPoint point)
        {
            BimaruValue? newValue = null;

            Direction? shipDirection = FindNeighbourShip(game, point);
            if (shipDirection.HasValue)
            {
                var oppositePoint = point.GetNextPoint(shipDirection.Value.GetOpposite());
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
            else if (AreAllNeighboursWater(game, point))
            {
                newValue = BimaruValue.SHIP_SINGLE;
            }

            return newValue;
        }

        private void DetermineShip(IGame game, GridPoint point)
        {
            if (game.Grid.GetFieldValue(point) != BimaruValue.SHIP_UNDETERMINED)
            {
                return;
            }

            BimaruValue? newValue = GetNewShipValue(game, point);

            if (newValue.HasValue)
            {
                game.Grid.SetFieldValue(point, newValue.Value);
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
