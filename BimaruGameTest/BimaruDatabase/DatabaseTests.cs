using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Bimaru.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bimaru.DatabaseUtil
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void TestGetAllGames()
        {
            var stubGames = GetStubGames();
            var gameSource = new GameSourceStub(stubGames);
            var db = new Database(gameSource);

            foreach (var filter in filtersToTest)
            {
                AssertEqualGames(stubGames.Where(g => filter == null || filter(g.MetaInfo)), db.GetAllGames(filter));
            }
        }

        private static IEnumerable<IGameWithMetaInfo> GetStubGames()
        {
            GameMetaInfo metaInfo;
            IGame game;

            metaInfo = new GameMetaInfo(0, GameSize.SMALL, GameDifficulty.EASY);
            game = new GameFactory().GenerateEmptyGame(1, 1);
            game.Grid[new Utility.GridPoint(0, 0)] = BimaruValue.SHIP_CONT_DOWN;
            yield return new GameWithMetaInfo(metaInfo, game);

            metaInfo = new GameMetaInfo(1, GameSize.SMALL, GameDifficulty.MIDDLE);
            game = new GameFactory().GenerateEmptyGame(1, 1);
            game.Grid[new Utility.GridPoint(0, 0)] = BimaruValue.SHIP_CONT_LEFT;
            yield return new GameWithMetaInfo(metaInfo, game);

            metaInfo = new GameMetaInfo(2, GameSize.MIDDLE, GameDifficulty.EASY);
            game = new GameFactory().GenerateEmptyGame(1, 1);
            game.Grid[new Utility.GridPoint(0, 0)] = BimaruValue.SHIP_CONT_LEFT;
            yield return new GameWithMetaInfo(metaInfo, game);

            metaInfo = new GameMetaInfo(3, GameSize.MIDDLE, GameDifficulty.HARD);
            game = new GameFactory().GenerateEmptyGame(1, 1);
            game.Grid[new Utility.GridPoint(0, 0)] = BimaruValue.SHIP_CONT_LEFT;
            yield return new GameWithMetaInfo(metaInfo, game);
        }

        private class GameSourceStub : IGameSource
        {
            public GameSourceStub(IEnumerable<IGameWithMetaInfo> games)
            {
                Games = new List<IGameWithMetaInfo>(games);
            }

            private readonly List<IGameWithMetaInfo> Games;

            public IEnumerable<IGameMetaInfo> GetMetaInfoOfAllGames()
            {
                return Games.Select(g => g.MetaInfo);
            }

            public IGameWithMetaInfo GetGame(int ID)
            {
                return Games.FirstOrDefault(g => g.MetaInfo.ID == ID);
            }
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
            var stubGames = GetStubGames();
            var gameSource = new GameSourceStub(stubGames);
            var db = new Database(gameSource);

            foreach (var filter in filtersToTest)
            {
                foreach (int i in Enumerable.Range(0, numTrials))
                {
                    AssertContainsGame(stubGames.Where(g => filter == null || filter(g.MetaInfo)), db.GetRandomGame(filter));
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
            var stubGames = GetStubGames();
            var gameSource = new GameSourceStub(stubGames);
            var db = new Database(gameSource);

            int maxId = 0;
            foreach (var game in stubGames)
            {
                game.AssertEqual(db.GetSpecificGame(game.MetaInfo.ID));
                maxId = Math.Max(maxId, game.MetaInfo.ID);
            }

            db.GetSpecificGame(maxId + 1).AssertEqual(null);
        }
    }
}
