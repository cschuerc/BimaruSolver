using System.Collections.Generic;
using Bimaru.Interfaces;
using Bimaru.Solver.CombinedRules;
using Bimaru.Solver.FieldChangedRules;
using Bimaru.Solver.TrialAndErrorRules;
using Utility;

namespace Bimaru.Solver
{
    public class SolverFactory : ISolverFactory
    {
        private static ITrialAndErrorRule GetTrialRule(bool shallCountSolutions)
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

        public IBimaruSolver GenerateSolver(bool shallCountSolutions)
        {
            var fieldChangedRules = new List<IFieldValueChangedRule>()
            {   new SetShipEnvironment(),
                new FillRowOrColumnWithWater(),
                new FillRowOrColumnWithShips(),
                new DetermineShipUndetermined(),
                new DetermineShipMiddleNeighbors()
            };

            var fullGridRules = new List<ISolverRule>()
            {   new FillRowOrColumnWithWater(),
                new FillRowOrColumnWithShips()
            };

            var trialRule = GetTrialRule(shallCountSolutions);

            return new BimaruSolver(
                fieldChangedRules,
                fullGridRules,
                trialRule,
                new Backup<IBimaruGrid>(),
                shallCountSolutions);
        }
    }
}
