using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BimaruDatabase
{
    [TestClass]
    public class ResourceDatabaseTests
    {
        [TestMethod]
        public void TestNoSerializer()
        {
            BinaryFormatter serializer = new BinaryFormatter();

            Assert.ThrowsException<ArgumentNullException>(
                () => new ResourceDatabase(null));
        }

        private static readonly List<Func<IGameMetaInfo, bool>> filtersToTest = new List<Func<IGameMetaInfo, bool>>()
        {
            null,
            m => true,
            m => false,
            m => m.Size == GameSize.LARGE,
            m => m.Size != GameSize.LARGE,
            m => m.Difficulty == GameDifficulty.EASY,
            m => m.Difficulty != GameDifficulty.EASY,
            m => m.Size == GameSize.MIDDLE && m.Difficulty == GameDifficulty.MIDDLE,
        };

        [TestMethod]
        public void TestGetAllGames()
        {
            BinaryFormatter serializer = new BinaryFormatter();

            var db = new ResourceDatabase(serializer);
            var allGames = GetAllGames(serializer);

            foreach (var filter in filtersToTest)
            {
                AssertEqualGames(allGames.Where(g => filter == null || filter(g.MetaInfo)), db.GetAllGames(filter));
            }
        }

        private static IEnumerable<IGameWithMetaInfo> GetAllGames(IFormatter serializer)
        {
            var games = new Dictionary<int, IGameWithMetaInfo>();

            var databaseAssembly = Assembly.GetAssembly(typeof(ResourceDatabase));
            foreach (string resourceName in databaseAssembly.GetManifestResourceNames())
            {
                using (Stream s = databaseAssembly.GetManifestResourceStream(resourceName))
                {
                    var game = (IGameWithMetaInfo)serializer.Deserialize(s);
                    games.Add(game.MetaInfo.ID, game); // Check no duplicate IDs
                }
            }

            return games.Values;
        }

        private static void AssertEqualGames(IEnumerable<IGameWithMetaInfo> expectedGames, IEnumerable<IGameWithMetaInfo> actualGames)
        {
            Assert.AreEqual(expectedGames.Count(), actualGames.Count());

            foreach (var expectedGame in expectedGames)
            {
                var match = actualGames.First(g => g.MetaInfo.ID == expectedGame.MetaInfo.ID);

                expectedGame.AssertEqual(match);
            }
        }

        [TestMethod]
        public void TestGetRandomGame()
        {
            int numTrials = 100;
            BinaryFormatter serializer = new BinaryFormatter();

            var db = new ResourceDatabase(serializer);
            var allGames = GetAllGames(serializer);

            foreach (var filter in filtersToTest)
            {
                foreach (int i in Enumerable.Range(0, numTrials))
                {
                    AssertContainsGame(allGames.Where(g => filter == null || filter(g.MetaInfo)), db.GetRandomGame(filter));
                }
            }
        }

        private static void AssertContainsGame(IEnumerable<IGameWithMetaInfo> expectedGames, IGameWithMetaInfo actualGame)
        {
            if (expectedGames.Count() == 0 && actualGame == null)
            {
                return;
            }

            Assert.IsNotNull(actualGame);

            var expectedGame = expectedGames.First(g => g.MetaInfo.ID == actualGame.MetaInfo.ID);

            expectedGame.AssertEqual(actualGame);
        }

        [TestMethod]
        public void TestGetSpecificGame()
        {
            BinaryFormatter serializer = new BinaryFormatter();

            var db = new ResourceDatabase(serializer);
            var allGames = GetAllGames(serializer);

            int maxId = 0;
            foreach (var game in allGames)
            {
                game.AssertEqual(db.GetSpecificGame(game.MetaInfo.ID));
                maxId = Math.Max(maxId, game.MetaInfo.ID);
            }

            db.GetSpecificGame(maxId + 1).AssertEqual(null);
        }
    }
}
