using System;
using System.Linq;
using Bimaru.Database;
using Bimaru.Game;
using Bimaru.Interface.Database;
using Bimaru.Interface.Utility;
using Xunit;

namespace Bimaru.Tests.Database
{
    public class GameSourceFromResourcesTests
    {
        [Fact]
        public void TestOneGameComplete()
        {
            var gameSource = new GameSourceFromResources();

            var actualGameWithMetaInfo = gameSource.GetGame(24);

            var expectedGameWithMetaInfo = new GameWithMetaInfo(
                new GameMetaInfo(24, GameSize.LARGE, GameDifficulty.HARD),
                new BimaruGame(
                    new GridTally(10)
                    {
                        [0] = 2, [1] = 1, [2] = 4, [3] = 2, [4] = 1, [5] = 2, [6] = 2, [7] = 1, [8] = 4, [9] = 1,
                    },
                    new GridTally(10)
                    {
                        [0] = 3, [1] = 2, [2] = 2, [3] = 2, [4] = 2, [5] = 1, [6] = 1, [7] = 3, [8] = 3, [9] = 1,
                    },
                    new ShipTarget()
                    {
                        [1] = 4, [2] = 3, [3] = 2, [4] = 1,
                    },
                    new BimaruGrid(10, 10)
                    {
                        [new GridPoint(0, 3)] = BimaruValue.SHIP_CONT_RIGHT,
                        [new GridPoint(0, 6)] = BimaruValue.WATER,
                        [new GridPoint(3, 6)] = BimaruValue.WATER,
                        [new GridPoint(7, 0)] = BimaruValue.SHIP_CONT_DOWN,
                        [new GridPoint(8, 9)] = BimaruValue.SHIP_SINGLE,
                    }
                )
            );

            actualGameWithMetaInfo.AssertEqual(expectedGameWithMetaInfo);
        }

        [Fact]
        public void TestLoadGameIdInRange()
        {
            var gameSource = new GameSourceFromResources();

            var maxId = gameSource.GetMetaInfoOfAllGames().Select(m => m.Id).Max();
            Assert.Equal(24, maxId);

            foreach (var id in Enumerable.Range(1, maxId))
            {
                var game = gameSource.GetGame(id);
                Assert.NotNull(game);
                Assert.Equal(id, game.MetaInfo.Id);
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
