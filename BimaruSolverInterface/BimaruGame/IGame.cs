namespace BimaruInterfaces
{
    public interface IGame
    {
        IGridTally TargetNumberOfShipFieldsPerRow
        {
            get;
        }

        IGridTally TargetNumberOfShipFieldsPerColumn
        {
            get;
        }

        IShipTarget TargetNumberOfShipsPerLength
        {
            get;
        }

        IBimaruGrid Grid
        {
            get;
        }

        int NumberOfMissingShipFieldsPerRow(int rowIndex);

        int NumberOfMissingShipFieldsPerColumn(int columnIndex);

        /// <summary>
        /// True, if the current Bimaru game is unsolvable.
        /// This means that no solution exists independently of the grid field values.
        /// False does not mean that the game is solvable.
        /// </summary>
        bool IsUnsolvable
        {
            get;
        }

        /// <summary>
        /// True, if the current game is valid.
        /// This means that it is not unsolvable and
        /// that all basic checks allow a solution to exist.
        /// </summary>
        bool IsValid
        {
            get;
        }

        bool IsSolved
        {
            get;
        }
    }
}
