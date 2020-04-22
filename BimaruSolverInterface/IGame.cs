namespace BimaruInterfaces
{
    /// <summary>
    /// Bimaru game
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Row tally
        /// </summary>
        ITally RowTally
        {
            get;
        }

        /// <summary>
        /// Column tally
        /// </summary>
        ITally ColumnTally
        {
            get;
        }

        /// <summary>
        /// Ship settings (requested number of ships)
        /// </summary>
        IShipSettings Settings
        {
            get;
        }

        /// <summary>
        /// Bimaru grid that is supposed to satisfy the tallies and ship settings
        /// </summary>
        IRollbackGrid Grid
        {
            get;
        }

        /// <summary>
        /// Number of missing ship fields in the row of given index.
        /// Might be negative if more than the requested ship fields are used.
        /// </summary>
        /// <param name="index"> Row index </param>
        /// <returns> Number of missing ship fields in the given row. </returns>
        int MissingShipFieldsRow(int index);

        /// <summary>
        /// Number of missing ship fields in the column of given index.
        /// Might be negative if more than the requested ship fields are used.
        /// </summary>
        /// <param name="index"> Column index </param>
        /// <returns> Number of missing ship fields in the given column. </returns>
        int MissingShipFieldsColumn(int index);

        /// <summary>
        /// True, if the current Bimaru game is unsolvable.
        /// This means that no solution exists independently of the grid field values.
        /// False does not mean the game is solvable.
        /// </summary>
        bool IsUnsolvable
        {
            get;
        }

        /// <summary>
        /// True, if the current Bimaru given is valid.
        /// This means that it is not unsolvable and that all basic checks allow a solution to exist.
        /// </summary>
        bool IsValid
        {
            get;
        }

        /// <summary>
        /// True, if the game is solved.
        /// </summary>
        bool IsSolved
        {
            get;
        }
    }
}
