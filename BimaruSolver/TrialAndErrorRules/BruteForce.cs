using Bimaru.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Bimaru.SolverUtil
{
    /// <summary>
    /// Tries for a single UNDETERMINED or SHIP_UNDETERMINED
    /// field all possible compatible and determined values.
    /// </summary>
    public class BruteForce : ITrialAndErrorRule
    {
        /// <summary>
        /// Every trial contradicts any other trial.
        /// Hence they are disjoint.
        /// </summary>
        public bool AreTrialsDisjoint => true;

        /// <summary>
        /// Every set of trials contains all possibilities
        /// for one field. Hence, they are complete.
        /// </summary>
        public bool AreTrialsComplete => true;

        public IEnumerable<FieldsToChange<BimaruValue>> GetChangeTrials(IGame game)
        {
            var notFullyDetPoint = GetNotFullyDeterminedPoint(game);
            if (notFullyDetPoint == null)
            {
                yield break;
            }

            var value = game.Grid[notFullyDetPoint.Value];

            bool isDeterminedAndCompatible(BimaruValue newValue) =>
                newValue.IsFullyDetermined() &&
                value.IsCompatibleChangeTo(newValue);

            foreach (BimaruValue newValue in BimaruValues.AllBimaruValues().Where(isDeterminedAndCompatible))
            {
                yield return new FieldsToChange<BimaruValue>(notFullyDetPoint.Value, newValue);
            }
        }

        private GridPoint? GetNotFullyDeterminedPoint(IGame game)
        {
            foreach (var p in game.Grid.AllPoints().Where(p => !game.Grid[p].IsFullyDetermined()))
            {
                return p;
            }

            return null;
        }
    }
}
