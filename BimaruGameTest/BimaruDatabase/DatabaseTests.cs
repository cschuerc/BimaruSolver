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
    public class DatabaseTests
    {
        [TestMethod]
        public void TestNoSerializer()
        {
            BinaryFormatter serializer = new BinaryFormatter();

            Assert.ThrowsException<ArgumentNullException>(
                () => new Database(null));

            new Database(serializer);
        }

        private static void CheckEquality(IDatabaseGame gameExp, IDatabaseGame gameActual)
        {
            if (gameExp == null && gameActual == null)
            {
                return;
            }

            Assert.IsTrue(gameExp != null && gameActual != null);

            Assert.AreEqual(gameExp.MetaInfo, gameActual.MetaInfo);

            Assert.IsTrue(gameExp.Game.RowTally.SequenceEqual(gameActual.Game.RowTally));
            Assert.IsTrue(gameExp.Game.ColumnTally.SequenceEqual(gameActual.Game.ColumnTally));

            Assert.AreEqual(gameExp.Game.ShipSettings.LongestShipLength, gameActual.Game.ShipSettings.LongestShipLength);

            foreach (int shipLength in Enumerable.Range(1, gameExp.Game.ShipSettings.LongestShipLength))
            {
                Assert.AreEqual(gameExp.Game.ShipSettings[shipLength], gameActual.Game.ShipSettings[shipLength]);
            }

            foreach (var p in gameExp.Game.Grid.AllPoints())
            {
                Assert.AreEqual(gameExp.Game.Grid[p], gameActual.Game.Grid[p]);
            }

            // More checks are possible
        }

        private static void CheckEqualGames(IEnumerable<IDatabaseGame> gamesExp, IEnumerable<IDatabaseGame> gamesActual)
        {
            Assert.AreEqual(gamesExp.Count(), gamesActual.Count());

            foreach (var game in gamesExp)
            {
                var match = gamesActual.First(g => g.MetaInfo.ID == game.MetaInfo.ID);

                CheckEquality(game, match);
            }
        }

        private static void CheckContainsGame(IEnumerable<IDatabaseGame> gamesExp, IDatabaseGame gameActual)
        {
            if (gamesExp.Count() == 0 && gameActual == null)
            {
                return;
            }

            Assert.IsNotNull(gameActual);

            var match = gamesExp.First(g => g.MetaInfo.ID == gameActual.MetaInfo.ID);

            CheckEquality(match, gameActual);
        }

        private static IEnumerable<IDatabaseGame> GetAllGames(IFormatter serializer)
        {
            var games = new Dictionary<int, IDatabaseGame>();

            var databaseAssembly = Assembly.GetAssembly(typeof(Database));
            foreach (string resourceName in databaseAssembly.GetManifestResourceNames())
            {
                using (Stream s = databaseAssembly.GetManifestResourceStream(resourceName))
                {
                    var game = (IDatabaseGame)serializer.Deserialize(s);
                    games.Add(game.MetaInfo.ID, game); // Check no duplicate IDs
                }
            }

            return games.Values;
        }

        private static readonly List<Func<IGameMetaInfo, bool>> _filters = new List<Func<IGameMetaInfo, bool>>()
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

            var db = new Database(serializer);
            var allGames = GetAllGames(serializer);

            foreach (var filter in _filters)
            {
                CheckEqualGames(allGames.Where(g => filter == null || filter(g.MetaInfo)), db.GetAllGames(filter));
            }
        }

        [TestMethod]
        public void TestGetRandomGame()
        {
            int numTrials = 100;
            BinaryFormatter serializer = new BinaryFormatter();

            var db = new Database(serializer);
            var allGames = GetAllGames(serializer);

            foreach (var filter in _filters)
            {
                foreach (int i in Enumerable.Range(0, numTrials))
                {
                    CheckContainsGame(allGames.Where(g => filter == null || filter(g.MetaInfo)), db.GetRandomGame(filter));
                }
            }
        }

        [TestMethod]
        public void TestGetSpecificGame()
        {
            BinaryFormatter serializer = new BinaryFormatter();

            var db = new Database(serializer);
            var allGames = GetAllGames(serializer);

            int maxId = 0;
            foreach (var game in allGames)
            {
                CheckEquality(game, db.GetSpecificGame(game.MetaInfo.ID));
                maxId = Math.Max(maxId, game.MetaInfo.ID);
            }

            CheckEquality(null, db.GetSpecificGame(maxId + 1));
        }
    }
}
