using System.Collections.Generic;
using Utility;

namespace BimaruInterfaces
{
    /// <summary>
    /// Trial and error rule to solve a Bimaru
    /// </summary>
    public interface ITrialAndErrorRule
    {
        /// <summary>
        /// Get a complete set of change trials.
        /// 
        /// Complete means that every solution to the grid
        /// is a descendant of at least one trial.
        /// 
        /// An example is when there are exactly N different possible
        /// locations for a single BATTLESHIP. Then this method would
        /// enumerate each of these N possibilities. Each of these N
        /// trials consists of four changes where each represents one
        /// piece of the BATTLESHIP.
        /// 
        /// For counting the number of solutions, it is also necessary
        /// that the trials are disjoint. Disjoint means that each
        /// solution is a descendant of at most one trial change.
        /// 
        /// Hence, if a set of trials is complete and disjoint, then
        /// each solution is a descendant of exactly one trial change.
        /// Note that this does not forbid trials without a descendant
        /// solution.
        /// 
        /// </summary>
        /// <param name="game"> Bimaru game </param>
        /// <returns> Enumerator for a complete set of change trials. </returns>
        IEnumerable<FieldsToChange<BimaruValue>> GetCompleteChangeTrials(IGame game);

        /// <summary>
        /// True, if the trials are always disjoint. If true, this
        /// trial and error rule can be used to count the number of
        /// solutions.
        /// </summary>
        bool AreTrialsDisjoint { get; }
    }
}
