using System.Collections.Generic;
using Utility;

namespace BimaruInterfaces
{
    /// <summary>
    /// Different types of field values
    /// </summary>
    public enum FieldValueTypes
    {
        /// <summary>
        /// Still undetermined field value.
        /// </summary>
        UNDETERMINED,

        /// <summary>
        /// Any kind of ship field.
        /// </summary>
        SHIP,

        /// <summary>
        /// Determined but no ship. Only WATER.
        /// </summary>
        NO_SHIP
    }

    /// <summary>
    /// Extensions to the ship boundaries
    /// </summary>
    public static class FieldValueTypesExtensions
    {
        private static readonly Dictionary<FieldValues, FieldValueTypes> _type =
            new Dictionary<FieldValues, FieldValueTypes>()
            {
                { FieldValues.SHIP_CONT_DOWN, FieldValueTypes.SHIP },
                { FieldValues.SHIP_CONT_LEFT, FieldValueTypes.SHIP },
                { FieldValues.SHIP_CONT_RIGHT, FieldValueTypes.SHIP },
                { FieldValues.SHIP_CONT_UP, FieldValueTypes.SHIP },
                { FieldValues.SHIP_MIDDLE, FieldValueTypes.SHIP },
                { FieldValues.SHIP_SINGLE, FieldValueTypes.SHIP },
                { FieldValues.SHIP_UNDETERMINED, FieldValueTypes.SHIP },
                { FieldValues.UNDETERMINED, FieldValueTypes.UNDETERMINED },
                { FieldValues.WATER, FieldValueTypes.NO_SHIP }
            };

        /// <summary>
        /// Get the type of the field value
        /// </summary>
        /// <param name="value"> Field value </param>
        /// <returns> Type of the field value </returns>
        public static FieldValueTypes GetFieldValueType(this FieldValues value)
        {
            return _type[value];
        }

        /// <summary>
        /// Whether the field is any kind of ship or not
        /// </summary>
        /// <param name="value"> Field value </param>
        /// <returns>True, if the field value is a kind of ship </returns>
        public static bool IsShip(this FieldValues value)
        {
            return value.GetFieldValueType() == FieldValueTypes.SHIP;
        }

        /// <summary>
        /// Get the type of constraint that a field gives to the neighbour field in the given direction.
        /// </summary>
        /// <param name="value"> Value of the field </param>
        /// <param name="direction"> Direction of the neighbour </param>
        /// <returns> Constraint that a field gives to the neighbour field in the given direction. </returns>
        private static FieldValueTypes GetConstraint(this FieldValues value, Directions direction)
        {
            FieldValueTypes constraint = FieldValueTypes.UNDETERMINED;

            if (value.IsShip())
            {
                if (direction.IsDiagonal())
                {
                    constraint = FieldValueTypes.NO_SHIP;
                }
                else if (value == direction.GetStartFieldValue())
                {
                    constraint = FieldValueTypes.SHIP;
                }
                else if (value != FieldValues.SHIP_MIDDLE &&
                    value != FieldValues.SHIP_UNDETERMINED)
                {
                    constraint = FieldValueTypes.NO_SHIP;
                }
            }

            return constraint;
        }

        /// <summary>
        /// Whether the constraint allows the field value.
        /// </summary>
        /// <param name="value"> Value to check </param>
        /// <param name="constraint"> Constraint </param>
        /// <returns> True, if the constraint is compatible with the field value </returns>
        private static bool IsAllowed(this FieldValues value, FieldValueTypes constraint)
        {
            FieldValueTypes valueType = value.GetFieldValueType();

            return valueType == FieldValueTypes.UNDETERMINED ||
                constraint == FieldValueTypes.UNDETERMINED ||
                valueType == constraint;
        }

        /// <summary>
        /// Whether the given value and its neighbouring other value in the given direction are compatible.
        /// </summary>
        /// <param name="value"> Field value </param>
        /// <param name="direction"> Direction of the neighbour from the field value </param>
        /// <param name="neighbourValue"> Neighbour value </param>
        /// <returns> True, if the value and the neighbour value in the given direction are compatible. </returns>
        public static bool IsCompatibleWith(this FieldValues value, Directions direction, FieldValues neighbourValue)
        {
            FieldValueTypes constraintToOther = value.GetConstraint(direction);
            FieldValueTypes constraintToThis = neighbourValue.GetConstraint(direction.GetOpposite());
            return neighbourValue.IsAllowed(constraintToOther) && value.IsAllowed(constraintToThis);
        }
    }
}