using Bimaru.Interface.Game;
using Bimaru.Interface.Utility;

namespace Bimaru.Interface.Solver
{
    public interface ITrialAndErrorRule
    {
        /// <summary>
        /// Get a set of change trials.
        /// 
        /// An example is when there are exactly N different possible
        /// locations for a single BATTLESHIP. Then this method would
        /// enumerate each of these N possibilities. Each of these N
        /// trials consists of four changes where each represents one
        /// piece of the BATTLESHIP.
        /// 
        /// </summary>
        IEnumerable<FieldsToChange<BimaruValue>> GetChangeTrials(IBimaruGame game);

        /// <summary>
        /// Disjoint means that each solution is a
        /// descendant of at most one change trial.
        /// </summary>
        bool AreTrialsDisjoint
        {
            get;
        }

        /// <summary>
        /// Complete means that every solution is a
        /// descendant of at least one change trial.
        /// </summary>
        bool AreTrialsComplete
        {
            get;
        }
    }
}
