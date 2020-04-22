using BimaruInterfaces;

namespace BimaruSolver
{
    /// <summary>
    /// Rule to further solve a Bimaru by looking at a large part of the grid.
    /// </summary>
    public interface IFullGridRule
    {
        /// <summary>
        /// Augments the solution by determining some unambiguous field values.
        /// </summary>
        /// <param name="game"> Bimaru game </param>
        void Solve(IGame game);
    }
}
