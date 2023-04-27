using Bimaru.Database;
using Bimaru.Interface.Database;
using Bimaru.Solver;
using Xunit;

namespace Bimaru.Tests.Solver
{
    public class SolverFactoryTests
    {
        [Fact]
        public void TestTwoSolutionGameNonCounting()
        {
            var solverNonCounting = new SolverFactory().GenerateSolver(false);
            var game = GameFactoryForTesting.GenerateGameTwoSolutions();

            Assert.False(game.IsSolved);
            var numSolutions = solverNonCounting.Solve(game);
            Assert.True(game.IsSolved);
            Assert.Equal(1, numSolutions);
        }

        [Fact]
        public void TestTwoSolutionGameCounting()
        {
            var solverCounting = new SolverFactory().GenerateSolver(true);
            var game = GameFactoryForTesting.GenerateGameTwoSolutions();

            Assert.False(game.IsSolved);
            var numSolutions = solverCounting.Solve(game);
            Assert.True(game.IsSolved);
            Assert.Equal(2, numSolutions);
        }

        [Fact]
        public void TestAllGamesNonCounting()
        {
            var solver = new SolverFactory().GenerateSolver(false);
            var database = GetDatabase();

            foreach (var databaseGame in database.GetAllGames(null))
            {
                Assert.False(databaseGame.Game.IsSolved);

                var numSolutions = solver.Solve(databaseGame.Game);

                Assert.True(databaseGame.Game.IsSolved);
                Assert.Equal(1, numSolutions);
            }
        }

        [Fact]
        public void TestAllGamesCounting()
        {
            var solver = new SolverFactory().GenerateSolver(true);
            var database = GetDatabase();

            foreach (var databaseGame in database.GetAllGames(null))
            {
                Assert.False(databaseGame.Game.IsSolved);

                var numSolutions = solver.Solve(databaseGame.Game);

                Assert.True(databaseGame.Game.IsSolved);
                Assert.Equal(1, numSolutions);
            }
        }

        private static IGameDatabase GetDatabase()
        {
            var gameSource = new GameSourceFromResources();
            return new GameDatabase(gameSource);
        }
    }
}
