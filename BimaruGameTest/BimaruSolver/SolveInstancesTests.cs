using BimaruDatabase;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace BimaruSolver
{
    [TestClass]
    public class SolveInstancesTests
    {
        private static Solver SetupSolver(ITrialAndErrorRule trialRule, bool shallCountSolutions)
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

            var trialAndErrorRule = new LongestMissingShip();

            return new Solver(fieldChangedRules, fullGridRules, trialRule, shallCountSolutions);
        }

        [TestMethod]
        public void TestAllGamesNonCounting()
        {
            var solver = SetupSolver(new LongestMissingShip(), false);
            var database = new Database(new BinaryFormatter());

            foreach (var databaseGame in database.GetAllGames(null))
            {
                Assert.IsFalse(databaseGame.Game.IsSolved);

                solver.Solve(databaseGame.Game);

                Assert.IsTrue(databaseGame.Game.IsSolved);
            }
        }

        [TestMethod]
        public void TestAllGamesCounting()
        {
            var solver = SetupSolver(new OneMissingShipOrWater(new BruteForce()), true);
            var database = new Database(new BinaryFormatter());

            foreach (var databaseGame in database.GetAllGames(null))
            {
                Assert.IsFalse(databaseGame.Game.IsSolved);

                int numSolutions = solver.Solve(databaseGame.Game);

                Assert.IsTrue(databaseGame.Game.IsSolved);
                Assert.AreEqual(1, numSolutions);
            }
        }
    }
}
