﻿using System.Collections.Generic;
using Utility;
using System.Linq;
using BimaruInterfaces;

namespace BimaruSolver
{
    /// <summary>
    /// Brute force trial rule. Tries for a single UNDETERMINED
    /// or SHIP_UNDETERMINED field all possible compatible and
    /// determined values.
    /// 
    /// This trial and error rule is complete and disjoint.
    /// Hence, it can be used to count the number of solutions.
    /// </summary>
    public class BruteForce : ITrialAndErrorRule
    {
        private GridPoint? GetNotFullyDeterminedGridPoint(IGame game)
        {
            foreach (var p in game.Grid.AllPoints().Where(p => !game.Grid[p].IsFullyDetermined()))
            {
                return p;
            }

            return null;
        }

        /// <inheritdoc/>
        public bool AreTrialsDisjoint => true;

        /// <inheritdoc/>
        public IEnumerable<FieldsToChange<BimaruValue>> GetCompleteChangeTrials(IGame game)
        {
            var notFullyDetPoint = GetNotFullyDeterminedGridPoint(game);

            if (notFullyDetPoint == null)
            {
                yield break;
            }

            var value = game.Grid[notFullyDetPoint.Value];

            bool isDeterminedAndCompatible(BimaruValue newValue) =>
                newValue.IsFullyDetermined() &&
                value.IsCompatibleChange(newValue);

            foreach (BimaruValue newValue in BimaruValueExtensions.AllBimaruValues().Where(isDeterminedAndCompatible))
            {
                yield return new FieldsToChange<BimaruValue>(notFullyDetPoint.Value, newValue);
            }
        }
    }
}
