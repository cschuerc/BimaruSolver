using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BimaruDatabase
{
    [TestClass]
    public class DatabaseGameTests
    {
        [TestMethod]
        public void TestNullArguments()
        {
            var metaInfo = new GameMetaInfo(0, GameSize.LARGE, GameDifficulty.EASY);
            var game = new GameFactory().GenerateEmptyGame(1, 1);

            Assert.ThrowsException<ArgumentNullException>(
                () => new DatabaseGame(null, game));

            Assert.ThrowsException<ArgumentNullException>(
                () => new DatabaseGame(metaInfo, null));

            new DatabaseGame(metaInfo, game);
        }
    }
}
