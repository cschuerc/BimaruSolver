namespace BimaruInterfaces
{
    public interface ISolverFactory
    {
        /// <param name="shallCountSolutions"> if false, then the solver stops after having found the first solution. </param>
        ISolver GenerateSolver(bool shallCountSolutions);
    }
}
