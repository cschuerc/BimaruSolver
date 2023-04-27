using System.Linq;
using Bimaru.DatabaseUtil;
using Xunit;

namespace Bimaru.Test
{
    public class DatabaseFactoryTests
    {
        [Fact]
        public void TestDatabaseIsNotNull()
        {
            Assert.NotNull(DatabaseFactory.GetDatabase());
        }

        [Fact]
        public void TestGamesInDatabase()
        {
            var database = DatabaseFactory.GetDatabase();

            Assert.True(database.GetAllGames(null).Any());
        }
    }
}
