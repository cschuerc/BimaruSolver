namespace BimaruInterfaces
{
    /// <summary>
    /// Settings of the number of requested ships per length
    /// </summary>
    public interface IShipSettings
    {
        /// <summary>
        /// Number of requested ships of given length
        /// </summary>
        /// <param name="length"> Ship length </param>
        /// <returns> Number of requested ships of given length </returns>
        int this[int length] { get; set; }

        /// <summary>
        /// Length of the longest requested ship. Returns 0 if no ship is requested.
        /// </summary>
        int LongestShipLength { get; }

        /// <summary>
        /// Total number of requested ship fields in the grid
        /// </summary>
        int NumShipFields { get; }
    }
}
