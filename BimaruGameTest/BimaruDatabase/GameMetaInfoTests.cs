using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BimaruDatabase
{
    [TestClass]
    public class GameMetaInfoTests
    {
        [TestMethod]
        public void TestEquality()
        {
            var game0 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game1 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);

            Assert.AreEqual(game0, game0);
            Assert.AreEqual(game0, game1);

            var game2 = new GameMetaInfo(1, GameSize.LARGE, GameDifficulty.EASY);

            Assert.AreNotEqual(game0, game2);

            var game3 = new GameMetaInfo(0, GameSize.MIDDLE, GameDifficulty.EASY);

            Assert.AreNotEqual(game0, game3);

            var game4 = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.HARD);

            Assert.AreNotEqual(game0, game4);
        }
    }
}
