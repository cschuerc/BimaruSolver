using BimaruInterfaces;
using System.Collections.Generic;

namespace BimaruSolver
{
    /// <summary>
    /// Creates efficient Bimaru solvers
    /// </summary>
    public class SolverFactory : ISolverFactory
    {
        private ITrialAndErrorRule GetTrialRule(bool shallCountSolutions)
        {
            ITrialAndErrorRule trialRule;

            if (shallCountSolutions)
            {
                trialRule = new OneMissingShipOrWater(new BruteForce());
            }
            else
            {
                trialRule = new LongestMissingShip();
            }

            return trialRule;
        }

        /// <inheritdoc/>
        public ISolver GenerateSolver(bool shallCountSolutions)
        {
            var fieldChangedRules = new List<IFieldChangedRule>()
            {   new SetShipEnvironment(),
                new FillRowOrColumnWithWater(),
                new FillRowOrColumnWithShips(),
                new DetermineShipFields()
            };

            var fullGridRules = new List<IFullGridRule>()
            {   new FillRowOrColumnWithWater(),
                new FillRowOrColumnWithShips()
            };

            var trialRule = GetTrialRule(shallCountSolutions);

            return new Solver(fieldChangedRules, fullGridRules, trialRule, shallCountSolutions);
        }
    }
}
