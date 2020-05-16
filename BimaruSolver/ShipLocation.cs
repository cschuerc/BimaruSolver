using Bimaru.Interfaces;
using Utility;

namespace Bimaru.SolverUtil
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
            private set;
        }

        public bool IsCompatibleButNotEqualIn(IGame game)
        {
            bool isEqual = true;

            foreach (var c in Changes)
            {
                BimaruValue currentValue = game.Grid[c.Point];

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
