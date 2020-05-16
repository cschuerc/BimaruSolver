using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bimaru.DatabaseUtil
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
