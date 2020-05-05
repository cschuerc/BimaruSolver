namespace BimaruInterfaces
{
    /// <summary>
    /// Rule to further solve a Bimaru by looking at a large part of the grid.
    /// </summary>
    public interface IFullGridRule
    {
        /// <summary>
        /// If true, then the rule should only be applied once.
        /// Calling it then several times will not hurt but is useless.
        /// </summary>
        bool ShallBeAppliedOnce { get; }

        /// <summary>
        /// Augments the solution by determining some unambiguous field values.
        /// </summary>
        /// <param name="game"> Bimaru game </param>
        void Solve(IGame game);
    }
}
