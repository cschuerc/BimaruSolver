using System.Linq;
using Bimaru.Database;
using webapi.Mappers;
using webapi.Models;
using Xunit;

namespace Bimaru.Tests.Database;

public class SeedGameDbTests
{
    [Fact]
    public void TestAllSeededGamesHaveUniqueSolution()
    {
        var solver = SolverFactoryForTesting.GenerateSolver(true);
        var entityMapper = GameMapperFactoryForTesting.Generate();

        var games = SeedGameDb.GetGameEntities()
            .Select(entityMapper.Map<GameDto>)
            .Select(GameDtoGameMapper.Map);

        foreach (var game in games)
        {
            Assert.False(game.IsSolved);

            var numSolutions = solver.Solve(game);

            Assert.True(game.IsSolved);
            Assert.Equal(1, numSolutions);
        }
    }
}