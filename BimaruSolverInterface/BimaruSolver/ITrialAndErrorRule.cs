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
        /// is a descendant of at least one trial change.
        /// 
        /// An example is when there are exactly N different possible
        /// locations for a single BATTLESHIP. Then this method would
        /// enumerate each of these N possibilities exactly once. Each
        /// of these change trials consists of four elements where each
        /// represents one piece of the BATTLESHIP.
        /// 
        /// Note that this method has to give always at least one set of
        /// non-trivial changes unless the Bimaru grid is fully determined.
        /// 
        /// For counting the number of solutions, it is also necessary that
        /// the trial changes are disjoint. Disjoint means that each solution is a
        /// descendant of exactly one trial change.
        /// 
        /// </summary>
        /// <param name="game"> Bimaru game </param>
        /// <returns> Enumerator for a complete set of change trials. </returns>
        IEnumerable<FieldsToChange<BimaruValue>> GetCompleteChangeTrials(IGame game);
    }
}
