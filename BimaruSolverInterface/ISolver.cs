namespace BimaruInterfaces
{
    /// <summary>
    /// Bimaru solver
    /// </summary>
    public interface ISolver
    {
        /// <summary>
        /// Tries to solve the Bimaru game.
        /// If it succeeds, the solution is on top of the grid stack right above the pre-solved game.
        /// If it fails, only the pre-solved game is on the grid stack.
        /// </summary>
        /// <param name="game"> Bimaru game to solve </param>
        void Solve(IGame game);
    }
}
