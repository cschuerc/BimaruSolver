using Bimaru.Interfaces;
using Bimaru.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bimaru.DatabaseUtil
{
    [TestClass]
    public class GameSourceFromResourcesTests
    {
        [TestMethod]
        public void TestNoSerializer()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new GameSourceFromResources(null));
        }

        [TestMethod]
        public void TestCorrectMetaInfo()
        {
            var serializer = new BinaryFormatter();
            var gameSource = new GameSourceFromResources(serializer);

            var expectedGames = GetAllGames(serializer);

            Assert.AreEqual(expectedGames.Count(), gameSource.GetMetaInfoOfAllGames().Count());

            foreach (var actualMetaInfo in gameSource.GetMetaInfoOfAllGames())
            {
                var expectedGame = expectedGames.FirstOrDefault(g => g.MetaInfo.ID == actualMetaInfo.ID);

                Assert.IsNotNull(expectedGame);
                Assert.AreEqual(expectedGame.MetaInfo, actualMetaInfo);
            }
        }

        private static IEnumerable<IGameWithMetaInfo> GetAllGames(IFormatter serializer)
        {
            var games = new Dictionary<int, IGameWithMetaInfo>();

            var databaseAssembly = Assembly.GetAssembly(typeof(GameSourceFromResources));
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

        [TestMethod]
        public void TestLoadGameIdInRange()
        {
            var serializer = new BinaryFormatter();
            var gameSource = new GameSourceFromResources(serializer);

            foreach (var expectedGame in GetAllGames(serializer))
            {
                var game = gameSource.GetGame(expectedGame.MetaInfo.ID);
                game.AssertEqual(expectedGame);
            }
        }

        [TestMethod]
        public void TestLoadGameIdOutOfRange()
        {
            var gameSource = new GameSourceFromResources(new BinaryFormatter());

            int maxId = gameSource.GetMetaInfoOfAllGames().Select(m => m.ID).Max();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => gameSource.GetGame(maxId + 1));
        }
    }
}
