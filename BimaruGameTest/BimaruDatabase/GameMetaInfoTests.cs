using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BimaruDatabase
{
    [TestClass]
    public class GameMetaInfoTests
    {
        [TestMethod]
        public void TestEqualitySameObject()
        {
            var game = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);

            Assert.AreEqual(game, game);
        }

        [TestMethod]
        public void TestEqualitySameData()
        {
            var game0 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game1 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);

            Assert.AreEqual(game0, game1);
        }

        [TestMethod]
        public void TestEqualityDifferentId()
        {
            var game0 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game1 = new GameMetaInfo(1, GameSize.LARGE, GameDifficulty.EASY);

            Assert.AreNotEqual(game0, game1);
        }

        [TestMethod]
        public void TestEqualityDifferentSize()
        {
            var game0 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game1 = new GameMetaInfo(0, GameSize.MIDDLE, GameDifficulty.EASY);

            Assert.AreNotEqual(game0, game1);
        }

        [TestMethod]
        public void TestEqualityDifferentDifficulty()
        {
            var game0 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game1 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.HARD);

            Assert.AreNotEqual(game0, game1);
        }
    }
}
