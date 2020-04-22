using BimaruInterfaces;
using System.Collections.Generic;

namespace BimaruSolver
{
    /// <summary>
    /// Trial and error rule to solve a Bimaru
    /// </summary>
    public interface ITrialAndErrorRule
    {
        /// <summary>
        /// Get a complete and disjoint set of change trials.
        /// 
        /// Complete means that every solution to the grid
        /// is a descendant of one trial change.
        /// 
        /// Disjoint means that no two trial changes are compatible
        /// with each other.
        /// 
        /// An example is when there are exactly N different possible
        /// locations for a single BATTLESHIP. Then this method would
        /// enumerate each of these N possibilities exactly once. Each
        /// of these change trials consists of four FieldToChange elements
        /// where each represents one piece of the BATTLESHIP.
        /// 
        /// </summary>
        /// <param name="game"> Bimaru game </param>
        /// <returns> Enumerator for a complete and disjoint set of change trials. </returns>
        IEnumerable<IEnumerable<FieldToChange>> GetCompleteChangeTrials(IGame game);
    }
}
