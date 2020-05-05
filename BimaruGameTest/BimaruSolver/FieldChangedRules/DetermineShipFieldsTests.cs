using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Utility;

namespace BimaruSolver
{
    [TestClass]
    public class DetermineShipFieldsTests
    {
        [TestMethod]
        public void TestDetShipSingle()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);

            var p11 = new GridPoint(1, 1);
            using (var subscriber = new ChangedRuleSubscriber(game, new DetermineShipFields()))
            {
                game.Grid[p11] = BimaruValue.SHIP_UNDETERMINED;

                game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;
                game.Grid[new GridPoint(1, 0)] = BimaruValue.WATER;
                game.Grid[new GridPoint(1, 2)] = BimaruValue.WATER;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.WATER;
            }

            // ?W?
            // WSW
            // ?W?

            Assert.AreEqual(BimaruValue.SHIP_SINGLE, game.Grid[p11]);
        }

        [TestMethod]
        public void TestDetShipMiddle()
        {
            var p11 = new GridPoint(1, 1);

            var game = (new GameFactory()).GenerateEmptyGame(3, 3);

            using (var subscriber = new ChangedRuleSubscriber(game, new DetermineShipFields()))
            {
                game.Grid[p11] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_UNDETERMINED;
            }

            // ???
            // SSS
            // ???

            Assert.AreEqual(BimaruValue.SHIP_MIDDLE, game.Grid[p11]);


            game = (new GameFactory()).GenerateEmptyGame(3, 3);

            using (var subscriber = new ChangedRuleSubscriber(game, new DetermineShipFields()))
            {
                game.Grid[p11] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(0, 1)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.SHIP_UNDETERMINED;
            }

            // ?S?
            // ?S?
            // ?S?

            Assert.AreEqual(BimaruValue.SHIP_MIDDLE, game.Grid[p11]);
        }

        [TestMethod]
        public void TestDetContShips()
        {
            var p11 = new GridPoint(1, 1);

            var game = (new GameFactory()).GenerateEmptyGame(3, 3);

            using (var subscriber = new ChangedRuleSubscriber(game, new DetermineShipFields()))
            {
                game.Grid[p11] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(1, 0)] = BimaruValue.WATER;
                game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_UNDETERMINED;
            }

            // ???
            // WSS
            // ???

            Assert.AreEqual(BimaruValue.SHIP_CONT_RIGHT, game.Grid[p11]);


            game = (new GameFactory()).GenerateEmptyGame(3, 3);

            using (var subscriber = new ChangedRuleSubscriber(game, new DetermineShipFields()))
            {
                game.Grid[p11] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(1, 2)] = BimaruValue.WATER;
            }

            // ???
            // SSW
            // ???

            Assert.AreEqual(BimaruValue.SHIP_CONT_LEFT, game.Grid[p11]);


            game = (new GameFactory()).GenerateEmptyGame(3, 3);

            using (var subscriber = new ChangedRuleSubscriber(game, new DetermineShipFields()))
            {
                game.Grid[p11] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.SHIP_UNDETERMINED;
            }

            // ?S?
            // ?S?
            // ?W?

            Assert.AreEqual(BimaruValue.SHIP_CONT_UP, game.Grid[p11]);


            game = (new GameFactory()).GenerateEmptyGame(3, 3);

            using (var subscriber = new ChangedRuleSubscriber(game, new DetermineShipFields()))
            {
                game.Grid[p11] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(0, 1)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.WATER;
            }

            // ?W?
            // ?S?
            // ?S?

            Assert.AreEqual(BimaruValue.SHIP_CONT_DOWN, game.Grid[p11]);
        }

        [TestMethod]
        public void TestShipMiddleNeighbours()
        {
            var p11 = new GridPoint(1, 1);

            var game = (new GameFactory()).GenerateEmptyGame(3, 3);

            using (var subscriber = new ChangedRuleSubscriber(game, new DetermineShipFields()))
            {
                game.Grid[p11] = BimaruValue.SHIP_MIDDLE;
            }

            // ???
            // ?S?
            // ???

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 2)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(2, 1)]);


            game = (new GameFactory()).GenerateEmptyGame(3, 3);

            using (var subscriber = new ChangedRuleSubscriber(game, new DetermineShipFields()))
            {
                game.Grid[p11] = BimaruValue.SHIP_MIDDLE;
                game.Grid[new GridPoint(0, 1)] = BimaruValue.UNDETERMINED;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.SHIP_CONT_DOWN;
            }

            // ?S?
            // ?S?
            // ???

            Assert.AreEqual(BimaruValue.SHIP_CONT_UP, game.Grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(1, 2)]);
            Assert.AreEqual(BimaruValue.SHIP_CONT_DOWN, game.Grid[new GridPoint(2, 1)]);


            game = (new GameFactory()).GenerateEmptyGame(3, 3);

            using (var subscriber = new ChangedRuleSubscriber(game, new DetermineShipFields()))
            {
                game.Grid[p11] = BimaruValue.SHIP_MIDDLE;
                game.Grid[new GridPoint(0, 1)] = BimaruValue.UNDETERMINED;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.WATER;
            }

            // ?W?
            // ?S?
            // ???

            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.SHIP_CONT_RIGHT, game.Grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.SHIP_CONT_LEFT, game.Grid[new GridPoint(1, 2)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(2, 1)]);
        }

        [TestMethod]
        public void TestSeveralShips()
        {
            var game = (new GameFactory()).GenerateEmptyGame(5, 5);

            using (var subscriber = new ChangedRuleSubscriber(game, new DetermineShipFields()))
            {
                game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(0, 1)] = BimaruValue.SHIP_MIDDLE;
                game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(0, 3)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(0, 4)] = BimaruValue.WATER;

                game.Grid[new GridPoint(1, 2)] = BimaruValue.WATER;
                game.Grid[new GridPoint(2, 2)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(3, 2)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(4, 2)] = BimaruValue.WATER;

                game.Grid[new GridPoint(4, 0)] = BimaruValue.SHIP_UNDETERMINED;

                game.Grid[new GridPoint(2, 4)] = BimaruValue.WATER;
                game.Grid[new GridPoint(3, 3)] = BimaruValue.WATER;
                game.Grid[new GridPoint(3, 4)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(4, 4)] = BimaruValue.WATER;
            }

            // S? ?? WW ?? WW
            // ?? ?? S? WW S?
            // ?? ?? S? ?? WW
            // ?? ?? WW ?? ??
            // S? SM S? S? WW

            Assert.AreEqual(BimaruValue.SHIP_CONT_RIGHT, game.Grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.SHIP_MIDDLE, game.Grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(1, 1)]);
            Assert.AreEqual(BimaruValue.SHIP_MIDDLE, game.Grid[new GridPoint(0, 2)]);
            Assert.AreEqual(BimaruValue.SHIP_CONT_LEFT, game.Grid[new GridPoint(0, 3)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(0, 4)]);

            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(1, 2)]);
            Assert.AreEqual(BimaruValue.SHIP_CONT_UP, game.Grid[new GridPoint(2, 2)]);
            Assert.AreEqual(BimaruValue.SHIP_CONT_DOWN, game.Grid[new GridPoint(3, 2)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(4, 2)]);

            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid[new GridPoint(4, 0)]);

            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(2, 4)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(3, 3)]);
            Assert.AreEqual(BimaruValue.SHIP_SINGLE, game.Grid[new GridPoint(3, 4)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(4, 4)]);

            int numUndet = game.Grid.AllPoints().Where(p => game.Grid[p] == BimaruValue.UNDETERMINED).Count();

            Assert.AreEqual(10, numUndet);
        }
    }
}
