using Bimaru.Interface.Game;
using Bimaru.Interface.Utility;

namespace Bimaru.Solver
{
    public class ShipLocation
    {
        public ShipLocation(GridPoint startPoint, Direction direction, int shipLength)
        {
            var shipFields = BimaruValues.FieldValuesOfShip(direction, shipLength);
            Changes = new FieldsToChange<BimaruValue>(startPoint, direction, shipFields);
        }

        public FieldsToChange<BimaruValue> Changes
        {
            get;
        }

        public bool IsCompatibleButNotEqualIn(IBimaruGame game)
        {
            var isEqual = true;

            foreach (var c in Changes)
            {
                var currentValue = game.Grid[c.Point];

                if (!currentValue.IsCompatibleChangeTo(c.NewValue))
                {
                    return false;
                }

                isEqual = isEqual && (currentValue == c.NewValue);
            }

            return !isEqual;
        }
    }
}
