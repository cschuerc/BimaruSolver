namespace Utility
{
    /// <summary>
    /// A point on a two-dimensional integer grid
    /// </summary>
    public interface IGridPoint
    {
        /// <summary>
        /// Get the row index of the point
        /// </summary>
        /// <returns> Row index of the point </returns>
        int RowIndex
        {
            get;
        }

        /// <summary>
        /// Get the column index of the point
        /// </summary>
        /// <returns> Column index of the point </returns>
        int ColumnIndex
        {
            get;
        }

        /// <summary>
        /// Get the next integer grid point in the specified direction
        /// </summary>
        /// <param name="direction"> Direction </param>
        /// <returns> The next point in the specified direction </returns>
        IGridPoint GetNextPoint(Directions direction);

        /// <summary>
        /// Get the boundary when going from the point in the direction.
        /// </summary>
        /// <param name="direction"> Direction to go </param>
        /// <returns> Field boundary from this point in the direction </returns>
        IFieldBoundary GetBoundary(Directions direction);
    }
}
