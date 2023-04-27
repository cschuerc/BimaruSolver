using System;
using System.Collections.Generic;
using System.Linq;
using Bimaru.Database;
using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Xunit;

namespace Bimaru.Test
{
    public class DatabaseTests
    {
        [Fact]
        public void TestGetAllGames()
        {
            var stubGames = GetStubGames();
            var gameSource = new GameSourceStub(stubGames);
            var db = new GameDatabase(gameSource);

            foreach (var filter in filtersToTest)
            {
                AssertEqualGames(stubGames.Where(g => filter == null || filter(g.MetaInfo)).ToList(), db.GetAllGames(filter).ToList());
            }
        }

        private static IReadOnlyCollection<IGameWithMetaInfo> GetStubGames()
        {
            var stubGames = new List<IGameWithMetaInfo>();

            var metaInfo = new GameMetaInfo(0, GameSize.SMALL, GameDifficulty.EASY);
            var game = new GameFactory().GenerateEmptyGame(1, 1);
            game.Grid[new Utility.GridPoint(0, 0)] = BimaruValue.SHIP_CONT_DOWN;
            stubGames.Add(new GameWithMetaInfo(metaInfo, game));

            metaInfo = new GameMetaInfo(1, GameSize.SMALL, GameDifficulty.MIDDLE);
            game = new GameFactory().GenerateEmptyGame(1, 1);
            game.Grid[new Utility.GridPoint(0, 0)] = BimaruValue.SHIP_CONT_LEFT;
            stubGames.Add(new GameWithMetaInfo(metaInfo, game));

            metaInfo = new GameMetaInfo(2, GameSize.MIDDLE, GameDifficulty.EASY);
            game = new GameFactory().GenerateEmptyGame(1, 1);
            game.Grid[new Utility.GridPoint(0, 0)] = BimaruValue.SHIP_CONT_LEFT;
            stubGames.Add(new GameWithMetaInfo(metaInfo, game));

            metaInfo = new GameMetaInfo(3, GameSize.MIDDLE, GameDifficulty.HARD);
            game = new GameFactory().GenerateEmptyGame(1, 1);
            game.Grid[new Utility.GridPoint(0, 0)] = BimaruValue.SHIP_CONT_LEFT;
            stubGames.Add(new GameWithMetaInfo(metaInfo, game));

            return stubGames;
        }

        private class GameSourceStub : IGameSource
        {
            public GameSourceStub(IEnumerable<IGameWithMetaInfo> games)
            {
                Games = new List<IGameWithMetaInfo>(games);
            }

            private List<IGameWithMetaInfo> Games { get; }

            public IEnumerable<IGameMetaInfo> GetMetaInfoOfAllGames()
            {
                return Games.Select(g => g.MetaInfo);
            }

            public IGameWithMetaInfo GetGame(int id)
            {
                return Games.FirstOrDefault(g => g.MetaInfo.Id == id);
            }
        }

        private static readonly List<Func<IGameMetaInfo, bool>> filtersToTest = new()
        {
            null,
            _ => true,
            _ => false,
            m => m.Size == GameSize.LARGE,
            m => m.Size != GameSize.LARGE,
            m => m.Difficulty == GameDifficulty.EASY,
            m => m.Difficulty != GameDifficulty.EASY,
            m => m.Size == GameSize.MIDDLE && m.Difficulty == GameDifficulty.MIDDLE,
        };

        private static void AssertEqualGames(IReadOnlyCollection<IGameWithMetaInfo> expectedGames, IReadOnlyCollection<IGameWithMetaInfo> actualGames)
        {
            Assert.Equal(expectedGames.Count, actualGames.Count);

            foreach (var expectedGame in expectedGames)
            {
                var match = actualGames.First(g => g.MetaInfo.Id == expectedGame.MetaInfo.Id);

                expectedGame.AssertEqual(match);
            }
        }

        [Fact]
        public void TestGetRandomGame()
        {
            const int numTrials = 100;
            var stubGames = GetStubGames();
            var gameSource = new GameSourceStub(stubGames);
            var db = new GameDatabase(gameSource);

            foreach (var filter in filtersToTest)
            {
                foreach (var _ in Enumerable.Range(0, numTrials))
                {
                    AssertContainsGame(stubGames.Where(g => filter == null || filter(g.MetaInfo)).ToList(), db.GetRandomGame(filter));
                }
            }
        }

        private static void AssertContainsGame(IReadOnlyCollection<IGameWithMetaInfo> expectedGames, IGameWithMetaInfo actualGame)
        {
            if (!expectedGames.Any() && actualGame == null)
            {
                return;
            }

            Assert.NotNull(actualGame);

            var expectedGame = expectedGames.First(g => g.MetaInfo.Id == actualGame.MetaInfo.Id);

            expectedGame.AssertEqual(actualGame);
        }

        [Fact]
        public void TestGetSpecificGame()
        {
            var stubGames = GetStubGames();
            var gameSource = new GameSourceStub(stubGames);
            var db = new GameDatabase(gameSource);

            var maxId = 0;
            foreach (var game in stubGames)
            {
                game.AssertEqual(db.GetSpecificGame(game.MetaInfo.Id));
                maxId = Math.Max(maxId, game.MetaInfo.Id);
            }

            db.GetSpecificGame(maxId + 1).AssertEqual(null);
        }
    }
}
