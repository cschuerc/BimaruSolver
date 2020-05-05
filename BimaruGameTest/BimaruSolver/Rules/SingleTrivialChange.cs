using BimaruInterfaces;
using System.Collections.Generic;
using Utility;

namespace BimaruSolver
{
    /// <summary>
    /// A trial and error rule that produces a single trial
    /// with a single but trivial change.
    /// </summary>
    internal class SingleTrivialChange : ITrialAndErrorRule
    {
        /// <inheritdoc/>
        public bool AreTrialsDisjoint => false;

        /// <inheritdoc/>
        public IEnumerable<FieldsToChange<BimaruValue>> GetCompleteChangeTrials(IGame game)
        {
            var p = new GridPoint(0, 0);
            BimaruValue oldValue = game.Grid[p];
            yield return new FieldsToChange<BimaruValue>(p, oldValue);
        }
    }
}
