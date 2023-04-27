using System.Linq;
using Bimaru.Database;
using Xunit;

namespace Bimaru.Tests.Database
{
    public class DatabaseFactoryTests
    {
        [Fact]
        public void TestDatabaseIsNotNull()
        {
            Assert.NotNull(GameDatabaseFactory.GetDatabase());
        }

        [Fact]
        public void TestGamesInDatabase()
        {
            var database = GameDatabaseFactory.GetDatabase();

            Assert.True(database.GetAllGames(null).Any());
        }
    }
}
