namespace BimaruInterfaces
{
    /// <summary>
    /// Bimaru game factory
    /// </summary>
    public interface IGameFactory
    {
        /// <summary>
        /// Generates an empty Bimaru game with the specified dimensions.
        /// </summary>
        /// <param name="numRows"> Number of rows </param>
        /// <param name="numColumns"> Number of columns </param>
        /// <returns></returns>
        IGame GenerateEmptyGame(int numRows, int numColumns);
    }
}
