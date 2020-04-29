namespace BimaruInterfaces
{
    /// <summary>
    /// Bimaru solver
    /// </summary>
    public interface ISolver
    {
        /// <summary>
        /// Computes the number of solutions of the Bimaru game.
        /// 
        /// If at least one solution is found, an arbitrary solution
        /// is on top of the grid stack right above the pre-solved game.
        /// 
        /// If no solution is found, the grid is left unchanged.
        /// </summary>
        /// <param name="game"> Bimaru game to solve </param>
        /// <returns> Number of solutions. </returns>
        int Solve(IGame game);
    }
}
