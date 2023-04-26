namespace Bimaru.Interfaces
{
    public interface ISolverFactory
    {
        /// <summary>
        /// Generates a new solver instance to solve Bimaru games.
        /// </summary>
        /// <param name="shallCountSolutions">if false, then the solver stops after having found the first solution.</param>
        /// <returns></returns>
        ISolver GenerateSolver(bool shallCountSolutions);
    }
}
