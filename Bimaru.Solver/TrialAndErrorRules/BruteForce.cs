﻿using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface.Game;
using Bimaru.Interface.Solver;
using Bimaru.Interface.Utility;

namespace Bimaru.Solver.TrialAndErrorRules
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

        public IEnumerable<FieldsToChange<BimaruValue>> GetChangeTrials(IBimaruGame game)
        {
            var notFullyDetPoint = GetNotFullyDeterminedPoint(game);
            if (notFullyDetPoint == null)
            {
                yield break;
            }

            var value = game.Grid[notFullyDetPoint.Value];

            bool IsDeterminedAndCompatible(BimaruValue newValue) =>
                newValue.IsFullyDetermined() &&
                value.IsCompatibleChangeTo(newValue);

            foreach (var newValue in BimaruValues.AllBimaruValues().Where(IsDeterminedAndCompatible))
            {
                yield return new FieldsToChange<BimaruValue>(notFullyDetPoint.Value, newValue);
            }
        }

        private static GridPoint? GetNotFullyDeterminedPoint(IBimaruGame game)
        {
            return game
                .Grid
                .AllPoints()
                .Where(p => !game.Grid[p].IsFullyDetermined())
                .Cast<GridPoint?>()
                .FirstOrDefault();
        }
    }
}
