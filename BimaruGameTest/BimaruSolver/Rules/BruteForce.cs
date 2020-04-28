using System.Collections.Generic;
using Utility;
using System.Linq;
using BimaruInterfaces;
using System;

namespace BimaruSolver
{
    /// <summary>
    /// Brute force trial rule. Tries for a single UNDETERMINED or SHIP_UNDETERMINED
    /// field all possible consistent and determined values.
    /// </summary>
    internal class BruteForce : ITrialAndErrorRule
    {
        /// <inheritdoc/>
        public IEnumerable<FieldsToChange<BimaruValue>> GetCompleteChangeTrials(IGame game)
        {
            GridPoint? undetPoint = game.Grid.AllPoints().FirstOrDefault(p =>
                game.Grid.GetFieldValue(p) == BimaruValue.UNDETERMINED ||
                game.Grid.GetFieldValue(p) == BimaruValue.SHIP_UNDETERMINED);

            if (undetPoint == null)
            {
                yield break;
            }

            BimaruValue value = game.Grid.GetFieldValue(undetPoint.Value);

            Func<BimaruValue, bool> isDeterminedAndConsistent = (b =>
                b.IsFullyDetermined() &&
                (!value.IsShip() || b.IsShip()));

            foreach (BimaruValue newValue in BimaruValueExtensions.AllBimaruValues().Where(isDeterminedAndConsistent))
            {
                yield return new FieldsToChange<BimaruValue>(undetPoint.Value, newValue);
            }
        }
    }
}
