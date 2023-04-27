using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Bimaru.Interface
{
    public enum BimaruValue
    {
        /// <summary>
        /// Undetermined (empty) fields
        /// </summary>
        UNDETERMINED,

        /// <summary>
        /// Water fields
        /// </summary>
        WATER,

        /// <summary>
        /// Ship field that continues leftwards but in no other direction
        /// </summary>
        SHIP_CONT_LEFT,

        /// <summary>
        /// Ship field that continues upwards but in no other direction
        /// </summary>
        SHIP_CONT_UP,

        /// <summary>
        /// Ship field that continues rightwards but in no other direction
        /// </summary>
        SHIP_CONT_RIGHT,

        /// <summary>
        /// Ship field that continues downwards but in no other direction
        /// </summary>
        SHIP_CONT_DOWN,

        /// <summary>
        /// Ship field that continues in two opposite directions (either up/down or left/right)
        /// </summary>
        SHIP_MIDDLE,

        /// <summary>
        /// Ship field that continues in no direction (single ship)
        /// </summary>
        SHIP_SINGLE,

        /// <summary>
        /// Ship field without information on where it continues/stops
        /// </summary>
        SHIP_UNDETERMINED
    }

    public static class BimaruValues
    {
        /// <summary>
        /// All Bimaru values in arbitrary order.
        /// </summary>
        public static IEnumerable<BimaruValue> AllBimaruValues()
        {
            return Enum.GetValues(typeof(BimaruValue)).Cast<BimaruValue>();
        }


        private static readonly HashSet<BimaruValue> isNotFullyDetermined =
            new HashSet<BimaruValue>()
            {
                BimaruValue.UNDETERMINED,
                BimaruValue.SHIP_UNDETERMINED
            };

        /// <summary>
        /// All Bimaru values except UNDETERMINED and SHIP_UNDETERMINED
        /// are fully determined.
        /// </summary>
        public static bool IsFullyDetermined(this BimaruValue value)
        {
            return !isNotFullyDetermined.Contains(value);
        }

        private static readonly Dictionary<Direction, BimaruValue> startBimaruValue =
            new Dictionary<Direction, BimaruValue>()
            {
                { Direction.DOWN, BimaruValue.SHIP_CONT_DOWN },
                { Direction.LEFT, BimaruValue.SHIP_CONT_LEFT },
                { Direction.RIGHT, BimaruValue.SHIP_CONT_RIGHT },
                { Direction.UP, BimaruValue.SHIP_CONT_UP }
            };

        /// <summary>
        /// First Bimaru value of a ship of length at least two heading in the specified direction.
        /// </summary>
        public static BimaruValue GetFirstShipValue(this Direction direction)
        {
            return startBimaruValue[direction];
        }

        /// <summary>
        /// Last Bimaru value of a ship of length at least two heading in the specified direction.
        /// </summary>
        public static BimaruValue GetLastShipValue(this Direction direction)
        {
            return startBimaruValue[direction.GetOpposite()];
        }

        /// <summary>
        /// True, if a new field value does not contradict the old value.
        /// For example, the new value WATER would be incompatible with
        /// SHIP_UNDETERMINED. However, the new value SHIP_SINGLE would
        /// be compatible with SHIP_UNDETERMINED, as it further specifies it.
        /// </summary>
        public static bool IsCompatibleChangeTo(this BimaruValue oldValue, BimaruValue newValue)
        {
            return oldValue == newValue ||
                oldValue == BimaruValue.UNDETERMINED ||
                (oldValue == BimaruValue.SHIP_UNDETERMINED && newValue.IsShip());
        }

        /// <summary>
        /// Enumerable over the ship field values of a ship of given length in a given direction.
        /// </summary>
        public static IEnumerable<BimaruValue> FieldValuesOfShip(Direction shipDirection, int shipLength)
        {
            if (shipLength < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(shipLength));
            }

            return FieldValuesOfShipNoCheck(shipDirection, shipLength);
        }

        private static IEnumerable<BimaruValue> FieldValuesOfShipNoCheck(Direction shipDirection, int shipLength)
        {
            if (shipLength == 1)
            {
                yield return BimaruValue.SHIP_SINGLE;
                yield break;
            }

            shipLength--;
            yield return shipDirection.GetFirstShipValue();

            while (shipLength > 1)
            {
                shipLength--;
                yield return BimaruValue.SHIP_MIDDLE;
            }

            yield return shipDirection.GetLastShipValue();
        }

        private static readonly Dictionary<BimaruValue, BimaruValueConstraint> constraintOfValue =
            new Dictionary<BimaruValue, BimaruValueConstraint>()
            {
                { BimaruValue.SHIP_CONT_DOWN, BimaruValueConstraint.SHIP },
                { BimaruValue.SHIP_CONT_LEFT, BimaruValueConstraint.SHIP },
                { BimaruValue.SHIP_CONT_RIGHT, BimaruValueConstraint.SHIP },
                { BimaruValue.SHIP_CONT_UP, BimaruValueConstraint.SHIP },
                { BimaruValue.SHIP_MIDDLE, BimaruValueConstraint.SHIP },
                { BimaruValue.SHIP_SINGLE, BimaruValueConstraint.SHIP },
                { BimaruValue.SHIP_UNDETERMINED, BimaruValueConstraint.SHIP },
                { BimaruValue.UNDETERMINED, BimaruValueConstraint.NO },
                { BimaruValue.WATER, BimaruValueConstraint.WATER }
            };

        /// <summary>
        /// True, if the field is any kind of ship.
        /// </summary>
        public static bool IsShip(this BimaruValue value)
        {
            return value.GetConstraint() == BimaruValueConstraint.SHIP;
        }

        /// <summary>
        /// Most specific constraint that the field value satisfies.
        /// </summary>
        public static BimaruValueConstraint GetConstraint(this BimaruValue value)
        {
            return constraintOfValue[value];
        }

        /// <summary>
        /// Constraint that a field value gives to the neighbor field in the given direction.
        /// </summary>
        public static BimaruValueConstraint GetConstraint(this BimaruValue value, Direction direction)
        {
            var constraint = BimaruValueConstraint.NO;

            if (value.IsShip())
            {
                if (direction.GetDirectionType() == DirectionType.DIAGONAL)
                {
                    constraint = BimaruValueConstraint.WATER;
                }
                else if (value == direction.GetFirstShipValue())
                {
                    constraint = BimaruValueConstraint.SHIP;
                }
                else if (value != BimaruValue.SHIP_MIDDLE &&
                    value != BimaruValue.SHIP_UNDETERMINED)
                {
                    constraint = BimaruValueConstraint.WATER;
                }
            }

            return constraint;
        }

        /// <summary>
        /// True, if the given value and its neighbor value in the given direction are compatible.
        /// </summary>
        public static bool IsCompatibleWith(this BimaruValue value, Direction direction, BimaruValue neighborValue)
        {
            var constraintToNeighbor = value.GetConstraint(direction);
            var constraintToThis = neighborValue.GetConstraint(direction.GetOpposite());
            return constraintToNeighbor.DoesAllow(neighborValue) && constraintToThis.DoesAllow(value);
        }
    }
}
