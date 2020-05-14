using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BimaruDatabase
{
    [TestClass]
    public class GameWithMetaInfoTests
    {
        [TestMethod]
        public void TestNullArguments()
        {
            var metaInfo = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game = new GameFactory().GenerateEmptyGame(1, 1);

            Assert.ThrowsException<ArgumentNullException>(
                () => new GameWithMetaInfo(null, game));

            Assert.ThrowsException<ArgumentNullException>(
                () => new GameWithMetaInfo(metaInfo, null));

            new GameWithMetaInfo(metaInfo, game);
        }
    }
}
