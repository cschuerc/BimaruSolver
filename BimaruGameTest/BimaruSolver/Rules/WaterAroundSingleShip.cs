using BimaruInterfaces;
using Utility;

namespace BimaruSolver
{
    /// <summary>
    /// Sets WATER all around a single ship.
    /// </summary>
    internal class WaterAroundSingleShip : IFieldChangedRule
    {
        /// <inheritdoc/>
        public void FieldValueChanged(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            if (game.Grid.GetFieldValue(e.Point) != BimaruValue.SHIP_SINGLE)
            {
                return;
            }

            foreach (Direction direction in DirectionExtensions.AllDirections())
            {
                game.Grid.SetFieldValue(e.Point.GetNextPoint(direction), BimaruValue.WATER);
            }
        }
    }
}
