using Bimaru.DatabaseUtil;
using Bimaru.Interfaces;
using Xunit;

namespace Bimaru.Test
{
    public class GameMetaInfoTests
    {
        [Fact]
        public void TestEqualitySameObject()
        {
            var game = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);

            Assert.Equal(game, game);
        }

        [Fact]
        public void TestEqualitySameData()
        {
            var game0 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game1 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);

            Assert.Equal(game0, game1);
        }

        [Fact]
        public void TestEqualityDifferentId()
        {
            var game0 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game1 = new GameMetaInfo(1, GameSize.LARGE, GameDifficulty.EASY);

            Assert.NotEqual(game0, game1);
        }

        [Fact]
        public void TestEqualityDifferentSize()
        {
            var game0 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game1 = new GameMetaInfo(0, GameSize.MIDDLE, GameDifficulty.EASY);

            Assert.NotEqual(game0, game1);
        }

        [Fact]
        public void TestEqualityDifferentDifficulty()
        {
            var game0 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game1 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.HARD);

            Assert.NotEqual(game0, game1);
        }
    }
}
