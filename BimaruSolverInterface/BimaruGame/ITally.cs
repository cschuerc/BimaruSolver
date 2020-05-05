using System.Collections.Generic;

namespace BimaruInterfaces
{
    /// <summary>
    /// Tally (total number of requested ship fields per row or column) of a Bimaru game
    /// </summary>
    public interface ITally: IEnumerable<int>
    {
        /// <summary>
        /// Length of the tally
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Total number of ship fields per row or column
        /// </summary>
        /// <param name="index"> Row or column index </param>
        /// <returns> Total number of ship fields at (row or column) index </returns>
        int this[int index] { get; set; }

        /// <summary>
        /// Sum over the row/column of the total number of ship fields
        /// </summary>
        int Sum { get; }
    }
}
