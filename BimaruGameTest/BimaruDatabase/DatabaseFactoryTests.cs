using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bimaru.DatabaseUtil
{
    [TestClass]
    public class DatabaseFactoryTests
    {
        [TestMethod]
        public void TestDatabaseIsNotNull()
        {
            var factory = new DatabaseFactory();

            Assert.IsNotNull(factory.GetDatabase());
        }

        [TestMethod]
        public void TestGamesInDatabase()
        {
            var factory = new DatabaseFactory();
            var database = factory.GetDatabase();

            Assert.IsTrue(database.GetAllGames(null).Count() > 0);
        }
    }
}
