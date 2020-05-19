using Bimaru.DatabaseUtil;
using Bimaru.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bimaru.SolverUtil
{
    [TestClass]
    public class SolverFactoryTests
    {
        [TestMethod]
        public void TestTwoSolutionGameNonCounting()
        {
            var solverNonCounting = new SolverFactory().GenerateSolver(false);
            var game = new GameFactoryForTesting().GenerateGameTwoSolutions();

            Assert.IsFalse(game.IsSolved);
            int numSolutions = solverNonCounting.Solve(game);
            Assert.IsTrue(game.IsSolved);
            Assert.AreEqual(1, numSolutions);
        }

        [TestMethod]
        public void TestTwoSolutionGameCounting()
        {
            var solverCounting = new SolverFactory().GenerateSolver(true);
            var game = new GameFactoryForTesting().GenerateGameTwoSolutions();

            Assert.IsFalse(game.IsSolved);
            int numSolutions = solverCounting.Solve(game);
            Assert.IsTrue(game.IsSolved);
            Assert.AreEqual(2, numSolutions);
        }

        [TestMethod]
        public void TestAllGamesNonCounting()
        {
            var solver = new SolverFactory().GenerateSolver(false);
            var database = new DatabaseFactory().GetDatabase();

            foreach (var databaseGame in database.GetAllGames(null))
            {
                Assert.IsFalse(databaseGame.Game.IsSolved);

                int numSolutions = solver.Solve(databaseGame.Game);

                Assert.IsTrue(databaseGame.Game.IsSolved);
                Assert.AreEqual(1, numSolutions);
            }
        }

        [TestMethod]
        public void TestAllGamesCounting()
        {
            var solver = new SolverFactory().GenerateSolver(true);
            var database = new DatabaseFactory().GetDatabase();

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
