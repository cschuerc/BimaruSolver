namespace BimaruInterfaces
{
    public interface ISolverFactory
    {
        ISolver GenerateSolver(bool shallCountSolutions);
    }
}
