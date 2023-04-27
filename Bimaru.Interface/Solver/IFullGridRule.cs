using Bimaru.Interface.Game;

namespace Bimaru.Interface.Solver
{
    public interface ISolverRule
    {
        /// <summary>
        /// If true, then the rule should only be applied once.
        /// Calling it then several times will not hurt but is useless.
        /// </summary>
        bool ShallBeAppliedOnce
        {
            get;
        }

        /// <summary>
        /// Improves the solution by determining some unambiguous field values.
        /// </summary>
        void Solve(IBimaruGame game);
    }
}
