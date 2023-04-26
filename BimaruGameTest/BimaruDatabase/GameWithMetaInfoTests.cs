using System;
using Bimaru.DatabaseUtil;
using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bimaru.Test
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

            var _ = new GameWithMetaInfo(metaInfo, game);
        }
    }
}
