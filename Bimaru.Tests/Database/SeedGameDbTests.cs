using System.Linq;
using AutoMapper;
using Bimaru.Database.DbContexts;
using Bimaru.Interface.Game;
using webapi.Models;
using webapi.Profiles;
using Xunit;

namespace Bimaru.Tests.Database;

public class SeedGameDbTests
{
    [Fact]
    public void TestAllSeededGamesHaveUniqueSolution()
    {
        var solver = SolverFactoryForTesting.GenerateSolver(true);
        var mapper = GetMapper();

        foreach (var game in SeedGameDb.GetGameEntities().Select(g => mapper.Map<GameDto>(g)).Select(g => mapper.Map<IBimaruGame>(g)))
        {
            Assert.False(game.IsSolved);

            var numSolutions = solver.Solve(game);

            Assert.True(game.IsSolved);
            Assert.Equal(1, numSolutions);
        }
    }

    private static IMapper GetMapper()
    {
        var config = new MapperConfiguration(cfg
            => cfg.AddProfile(new GameProfile()));

        return config.CreateMapper();
    }
}