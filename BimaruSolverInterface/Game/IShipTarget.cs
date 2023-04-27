using System.Collections.Generic;

namespace Bimaru.Interfaces
{
    /// <summary>
    /// Target number of ships per length
    /// </summary>
    public interface IShipTarget
    {
        /// <summary>
        /// Target number of ships of given length
        /// </summary>
        int this[int shipLength]
        {
            get;
            set;
        }

        /// <summary>
        /// Length of the longest requested ship. Null if no ship is requested.
        /// </summary>
        int? LongestShipLength
        {
            get;
        }

        /// <summary>
        /// Total target number of ship fields.
        /// </summary>
        int TotalShipFields
        {
            get;
        }

        /// <summary>
        /// How the target number of ships is satisfied.
        /// </summary>
        Satisfiability GetSatisfiability(IReadOnlyList<int> numberOfShipsPerLength);

        /// <summary>
        /// Null if no ship is missing.
        /// </summary>
        int? LengthOfLongestMissingShip(IReadOnlyList<int> numberOfShipsPerLength);
    }
}
