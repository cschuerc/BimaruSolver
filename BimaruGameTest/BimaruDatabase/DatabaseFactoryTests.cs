using System.Linq;
using Bimaru.DatabaseUtil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bimaru.Test
{
    [TestClass]
    public class DatabaseFactoryTests
    {
        [TestMethod]
        public void TestDatabaseIsNotNull()
        {
            Assert.IsNotNull(DatabaseFactory.GetDatabase());
        }

        [TestMethod]
        public void TestGamesInDatabase()
        {
            var database = DatabaseFactory.GetDatabase();

            Assert.IsTrue(database.GetAllGames(null).Any());
        }
    }
}
