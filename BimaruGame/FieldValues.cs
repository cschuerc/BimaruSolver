using System.Collections.Generic;

namespace Bimaru
{
    /// <summary>
    /// Enumerates all possible field values (empty, water, ship, ...) of a Bimaru game.
    /// </summary>
    public enum FieldValues
    {
        /// <summary>
        /// Undetermined fields
        /// </summary>
        UNDETERMINED,

        /// <summary>
        /// Water field
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

    /// <summary>
    /// Extensions concerning the BimaruFieldValue class
    /// </summary>
    public static class BimaruFieldValueExtensions
    {
        private static HashSet<FieldValues> _isNotDetermined =
            new HashSet<FieldValues>()
            {   FieldValues.UNDETERMINED,
                FieldValues.SHIP_UNDETERMINED};

        /// <summary>
        /// Whether the field value is fully determined.
        /// </summary>
        /// <param name="value"> Field value </param>
        /// <returns> True, if the field value is fully determined. </returns>
        public static bool IsFullyDetermined(this FieldValues value)
        {
            return !_isNotDetermined.Contains(value); 
        }

        private static readonly Dictionary<Directions, FieldValues> _startFieldValue =
            new Dictionary<Directions, FieldValues>()
                {
                    { Directions.DOWN, FieldValues.SHIP_CONT_DOWN },
                    { Directions.LEFT, FieldValues.SHIP_CONT_LEFT },
                    { Directions.RIGHT, FieldValues.SHIP_CONT_RIGHT },
                    { Directions.UP, FieldValues.SHIP_CONT_UP }
                };

        /// <summary>
        /// Get the first Bimaru field value of a ship of length at least two that continues in the specified direction
        /// </summary>
        /// <param name="value"> Direction in which the ship continues </param>
        /// <returns> Starting field value of a ship looking in the specified direction </returns>
        public static FieldValues GetStartFieldValue(this Directions value)
        {
            return _startFieldValue[value];
        }

        /// <summary>
        /// Get the last Bimaru field value of a ship of length at least two that continues in the specified direction
        /// </summary>
        /// <param name="value"> Direction in which the ship continues </param>
        /// <returns> End field value of a ship looking in the specified direction </returns>
        public static FieldValues GetEndFieldValue(this Directions value)
        {
            return _startFieldValue[value.GetOpposite()];
        }
    }
}
