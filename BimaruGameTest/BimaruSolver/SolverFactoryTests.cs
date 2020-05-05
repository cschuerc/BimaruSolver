using BimaruDatabase;
using BimaruGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization.Formatters.Binary;

namespace BimaruSolver
{
    [TestClass]
    public class SolverFactoryTests
    {
        [TestMethod]
        public void TestFactory()
        {
            var solverNonCounting = (new SolverFactory()).GenerateSolver(false);
            var game = (new GameFactory()).GenerateGameTwoSolutions();

            Assert.IsFalse(game.IsSolved);
            int numSolutions = solverNonCounting.Solve(game);
            Assert.IsTrue(game.IsSolved);
            Assert.AreEqual(1, numSolutions);

            var solverCounting = (new SolverFactory()).GenerateSolver(true);
            game = (new GameFactory()).GenerateGameTwoSolutions();

            Assert.IsFalse(game.IsSolved);
            numSolutions = solverCounting.Solve(game);
            Assert.IsTrue(game.IsSolved);
            Assert.AreEqual(2, numSolutions);
        }

        [TestMethod]
        public void TestAllGamesNonCounting()
        {
            var solver = (new SolverFactory()).GenerateSolver(false);
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
            var solver = (new SolverFactory()).GenerateSolver(true);
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
