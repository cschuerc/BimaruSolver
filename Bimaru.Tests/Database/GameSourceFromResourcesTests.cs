using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Bimaru.Database;
using Bimaru.Interface.Database;
using Xunit;

namespace Bimaru.Tests.Database
{
    public class GameSourceFromResourcesTests
    {
        [Fact]
        public void TestCorrectMetaInfo()
        {
            var gameSource = new GameSourceFromResources();

            var expectedGames = GetAllGames();

            Assert.Equal(expectedGames.Count, gameSource.GetMetaInfoOfAllGames().Count());

            foreach (var actualMetaInfo in gameSource.GetMetaInfoOfAllGames())
            {
                var expectedGame = expectedGames.FirstOrDefault(g => g.MetaInfo.Id == actualMetaInfo.Id);

                Assert.NotNull(expectedGame);
                Assert.Equal(expectedGame.MetaInfo, actualMetaInfo);
            }
        }

        private static IReadOnlyCollection<IGameWithMetaInfo> GetAllGames()
        {
            var databaseAssembly = Assembly.GetAssembly(typeof(GameSourceFromResources));
            if (databaseAssembly == null)
            {
                return Array.Empty<IGameWithMetaInfo>();
            }

            var games = new Dictionary<int, IGameWithMetaInfo>();

            foreach (var resourceName in databaseAssembly.GetManifestResourceNames())
            {
                using var s = databaseAssembly.GetManifestResourceStream(resourceName);
                if (s == null)
                {
                    continue;
                }

                var game = JsonSerializer.Deserialize<IGameWithMetaInfo>(s);
                games.Add(game.MetaInfo.Id, game); // Check no duplicate IDs
            }

            return games.Values;
        }

        [Fact]
        public void TestLoadGameIdInRange()
        {
            var gameSource = new GameSourceFromResources();

            foreach (var expectedGame in GetAllGames())
            {
                var game = gameSource.GetGame(expectedGame.MetaInfo.Id);
                game.AssertEqual(expectedGame);
            }
        }

        [Fact]
        public void TestLoadGameIdOutOfRange()
        {
            var gameSource = new GameSourceFromResources();

            var maxId = gameSource.GetMetaInfoOfAllGames().Select(m => m.Id).Max();

            Assert.Throws<ArgumentOutOfRangeException>(() => gameSource.GetGame(maxId + 1));
        }
    }
}
