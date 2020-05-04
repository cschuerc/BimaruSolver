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
        private static ISolver SetupNonCountingSolver()
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

            var trialAndErrorRule = new TrialLongestMissingShip();

            return new Solver(fieldChangedRules, fullGridRules, trialAndErrorRule, true);
        }

        [TestMethod]
        public void TestAllGames()
        {
            var solver = SetupNonCountingSolver();
            var database = new Database(new BinaryFormatter());

            foreach (var databaseGame in database.GetAllGames(null))
            {
                Assert.IsFalse(databaseGame.Game.IsSolved);

                solver.Solve(databaseGame.Game);

                Assert.IsTrue(databaseGame.Game.IsSolved);
            }
        }
    }
}
