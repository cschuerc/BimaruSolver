using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace BimaruGame
{
    [TestClass]
    public class GameFactoryTests
    {
        [TestMethod]
        public void TestGenerateEmptyGameRowTally()
        {
            var game = new GameFactory().GenerateEmptyGame(3, 4);

            Assert.IsTrue(game.TargetNumberOfShipFieldsPerRow.SequenceEqual(new int[3] { 0, 0, 0 }));
        }

        [TestMethod]
        public void TestGenerateEmptyGameColumnTally()
        {
            var game = new GameFactory().GenerateEmptyGame(3, 4);

            Assert.IsTrue(game.TargetNumberOfShipFieldsPerColumn.SequenceEqual(new int[4] { 0, 0, 0, 0 }));
        }

        [TestMethod]
        public void TestGenerateEmptyGameShipTarget()
        {
            var game = new GameFactory().GenerateEmptyGame(3, 4);

            Assert.IsNull(game.TargetNumberOfShipsPerLength.LongestShipLength);
        }

        [TestMethod]
        public void TestGenerateEmptyGameGrid()
        {
            var game = new GameFactory().GenerateEmptyGame(3, 4);

            foreach (var p in game.Grid.AllPoints())
            {
                Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[p]);
            }
        }
    }
}
