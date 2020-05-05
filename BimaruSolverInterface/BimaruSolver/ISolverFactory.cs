namespace BimaruInterfaces
{
    /// <summary>
    /// Factory to generate Bimaru solvers
    /// </summary>
    public interface ISolverFactory
    {
        /// <summary>
        /// Generates a Bimaru solver
        /// </summary>
        /// <param name="shallCountSolutions"> True, if the number of solutions shall be counted </param>
        /// <returns></returns>
        ISolver GenerateSolver(bool shallCountSolutions);
    }
}
