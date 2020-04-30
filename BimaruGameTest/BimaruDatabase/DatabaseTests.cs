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

            // More checks are possible
        }

        private static void CheckEqualGames(IEnumerable<IDatabaseGame> gamesExp, IEnumerable<IDatabaseGame> gamesActual)
        {
            Assert.AreEqual(gamesExp.Count(), gamesActual.Count());

            foreach (var game in gamesExp)
            {
                var match = gamesActual.First(g => g.MetaInfo.ID == game.MetaInfo.ID);

                Assert.AreNotEqual(null, match);
                CheckEquality(game, match);
            }
        }

        private static void CheckContainsGame(IEnumerable<IDatabaseGame> gamesExp, IDatabaseGame gamesActual)
        {
            if (gamesExp.Count() == 0 && gamesActual == null)
            {
                return;
            }

            Assert.AreNotEqual(null, gamesActual);

            var match = gamesExp.First(g => g.MetaInfo.ID == gamesActual.MetaInfo.ID);

            Assert.AreNotEqual(null, match);
            CheckEquality(match, gamesActual);
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

            foreach (var isOkay in _filters)
            {
                CheckEqualGames(allGames.Where(g => isOkay == null || isOkay(g.MetaInfo)), db.GetAllGames(isOkay));
            }
        }

        [TestMethod]
        public void TestGetRandomGame()
        {
            BinaryFormatter serializer = new BinaryFormatter();

            var db = new Database(serializer);
            var allGames = GetAllGames(serializer);

            foreach (var isOkay in _filters)
            {
                foreach (int i in Enumerable.Range(0, 100))
                {
                    CheckContainsGame(allGames.Where(g => isOkay == null || isOkay(g.MetaInfo)), db.GetRandomGame(isOkay));
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
