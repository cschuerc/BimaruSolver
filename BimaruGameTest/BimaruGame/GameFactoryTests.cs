using System.Linq;
using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bimaru.Test
{
    [TestClass]
    public class GameFactoryTests
    {
        [TestMethod]
        public void TestGenerateEmptyGameRowTally()
        {
            var game = new GameFactory().GenerateEmptyGame(3, 4);

            Assert.IsTrue(game.TargetNumberOfShipFieldsPerRow.SequenceEqual(new[] { 0, 0, 0 }));
        }

        [TestMethod]
        public void TestGenerateEmptyGameColumnTally()
        {
            var game = new GameFactory().GenerateEmptyGame(3, 4);

            Assert.IsTrue(game.TargetNumberOfShipFieldsPerColumn.SequenceEqual(new[] { 0, 0, 0, 0 }));
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
