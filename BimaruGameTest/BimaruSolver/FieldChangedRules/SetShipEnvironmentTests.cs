using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace BimaruSolver
{
    [TestClass]
    public class SetShipEnvironmentTests
    {
        [TestMethod]
        public void TestWater()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            var p11 = new GridPoint(1, 1);
            using (var subscriber = new ChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[p11] = BimaruValue.WATER;
            }

            Assert.AreEqual(BimaruValue.WATER, game.Grid[p11]);

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(0, 2)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(2, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(2, 2)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 2)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(2, 1)]);
        }

        [TestMethod]
        public void TestSingleShip()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            game.TargetNumberOfShipFieldsPerRow[1] = 1;
            game.TargetNumberOfShipFieldsPerColumn[1] = 1;
            game.TargetNumberOfShipsPerLength[1] = 1;

            var p11 = new GridPoint(1, 1);
            using (var subscriber = new ChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[p11] = BimaruValue.SHIP_SINGLE;
            }

            // 1xBATTLESHIP
            //   010
            //   ---
            // 0|???
            // 1|?S?
            // 0|???

            Assert.IsTrue(game.IsSolved);
        }

        [TestMethod]
        public void TestMiddleShip()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);

            var p11 = new GridPoint(1, 1);
            using (var subscriber = new ChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[p11] = BimaruValue.SHIP_MIDDLE;
            }

            Assert.AreEqual(BimaruValue.SHIP_MIDDLE, game.Grid[p11]);

            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(0, 2)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(2, 0)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(2, 2)]);

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 2)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(2, 1)]);
        }

        [TestMethod]
        public void TestShipUndet()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);

            var p11 = new GridPoint(1, 1);
            using (var subscriber = new ChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[p11] = BimaruValue.SHIP_UNDETERMINED;
            }

            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid[p11]);

            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(0, 2)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(2, 0)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(2, 2)]);

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 2)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(2, 1)]);
        }

        [TestMethod]
        public void TestShipCont()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            var p11 = new GridPoint(1, 1);
            using (var subscriber = new ChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[p11] = BimaruValue.SHIP_CONT_RIGHT;
            }

            Assert.AreEqual(BimaruValue.SHIP_CONT_RIGHT, game.Grid[p11]);

            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(0, 2)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(2, 0)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(2, 2)]);

            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(2, 1)]);

            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid[new GridPoint(1, 2)]);
        }

        [TestMethod]
        public void TestShipContOverwrite()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            game.TargetNumberOfShipFieldsPerRow[1] = 2;
            game.TargetNumberOfShipFieldsPerColumn[1] = 1;
            game.TargetNumberOfShipFieldsPerColumn[2] = 1;
            game.TargetNumberOfShipsPerLength[2] = 1;

            var p11 = new GridPoint(1, 1);
            var p12 = new GridPoint(1, 2);
            using (var subscriber = new ChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[p11] = BimaruValue.SHIP_CONT_RIGHT;
                game.Grid[p12] = BimaruValue.SHIP_CONT_LEFT;
            }

            // 1xDESTROYER
            //   011
            //   ---
            // 0|???
            // 2|?SS
            // 0|???

            Assert.IsTrue(game.IsSolved);
        }
    }
}
