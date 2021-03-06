using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Bimaru.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace Bimaru.SolverUtil
{
    [TestClass]
    public class DetermineShipUndeterminedTests
    {
        [TestMethod]
        public void TestShipSingle()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (var subscriber = new FieldValueChangedRuleSubscriber(game, new DetermineShipUndetermined()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_UNDETERMINED;

                game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;
                game.Grid[new GridPoint(1, 0)] = BimaruValue.WATER;
                game.Grid[new GridPoint(1, 2)] = BimaruValue.WATER;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.WATER;
            }

            game.Grid.AssertEqual(
                new BimaruValue[3, 3]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.WATER, BimaruValue.UNDETERMINED },
                    { BimaruValue.WATER, BimaruValue.SHIP_SINGLE, BimaruValue.WATER },
                    { BimaruValue.UNDETERMINED, BimaruValue.WATER, BimaruValue.UNDETERMINED }
                });
        }

        [TestMethod]
        public void TestShipSingleOrderReversed()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (var subscriber = new FieldValueChangedRuleSubscriber(game, new DetermineShipUndetermined()))
            {
                game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;
                game.Grid[new GridPoint(1, 0)] = BimaruValue.WATER;
                game.Grid[new GridPoint(1, 2)] = BimaruValue.WATER;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.WATER;

                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_UNDETERMINED;
            }

            game.Grid.AssertEqual(
                new BimaruValue[3, 3]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.WATER, BimaruValue.UNDETERMINED },
                    { BimaruValue.WATER, BimaruValue.SHIP_SINGLE, BimaruValue.WATER },
                    { BimaruValue.UNDETERMINED, BimaruValue.WATER, BimaruValue.UNDETERMINED }
                });
        }

        [TestMethod]
        public void TestShipMiddleHorizontal()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (var subscriber = new FieldValueChangedRuleSubscriber(game, new DetermineShipUndetermined()))
            {
                game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_UNDETERMINED;
            }

            game.Grid.AssertEqual(
                new BimaruValue[3, 3]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                    { BimaruValue.SHIP_CONT_RIGHT, BimaruValue.SHIP_MIDDLE, BimaruValue.SHIP_CONT_LEFT },
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED }
                });
        }

        [TestMethod]
        public void TestShipMiddleVertical()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (var subscriber = new FieldValueChangedRuleSubscriber(game, new DetermineShipUndetermined()))
            {
                game.Grid[new GridPoint(0, 1)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.SHIP_UNDETERMINED;
            }

            game.Grid.AssertEqual(
                new BimaruValue[3, 3]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.SHIP_CONT_UP, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.SHIP_MIDDLE, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.SHIP_CONT_DOWN, BimaruValue.UNDETERMINED }
                });
        }

        [TestMethod]
        public void TestShipContRight()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (var subscriber = new FieldValueChangedRuleSubscriber(game, new DetermineShipUndetermined()))
            {
                game.Grid[new GridPoint(1, 0)] = BimaruValue.WATER;
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_UNDETERMINED;
            }

            game.Grid.AssertEqual(
                new BimaruValue[3, 3]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                    { BimaruValue.WATER, BimaruValue.SHIP_CONT_RIGHT, BimaruValue.SHIP_CONT_LEFT },
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED }
                });
        }
        public void TestShipContLeft()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (var subscriber = new FieldValueChangedRuleSubscriber(game, new DetermineShipUndetermined()))
            {
                game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(1, 2)] = BimaruValue.WATER;
            }

            game.Grid.AssertEqual(
                new BimaruValue[3, 3]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                    { BimaruValue.SHIP_CONT_RIGHT, BimaruValue.SHIP_CONT_LEFT, BimaruValue.WATER },
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED }
                });
        }

        public void TestShipContUp()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (var subscriber = new FieldValueChangedRuleSubscriber(game, new DetermineShipUndetermined()))
            {
                game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.SHIP_UNDETERMINED;
            }

            game.Grid.AssertEqual(
                new BimaruValue[3, 3]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.WATER, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.SHIP_CONT_UP, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.SHIP_CONT_DOWN, BimaruValue.UNDETERMINED }
                });
        }

        public void TestShipContDown()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (var subscriber = new FieldValueChangedRuleSubscriber(game, new DetermineShipUndetermined()))
            {
                game.Grid[new GridPoint(0, 1)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_UNDETERMINED;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.WATER;
            }

            game.Grid.AssertEqual(
                new BimaruValue[3, 3]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.SHIP_CONT_UP, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.SHIP_CONT_DOWN, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.WATER, BimaruValue.UNDETERMINED }
                });
        }

        [TestMethod]
        public void TestSeveralShips()
        {
            var game = (new GameFactory()).GenerateEmptyGame(5, 5);

            using (var subscriber = new FieldValueChangedRuleSubscriber(game, new DetermineShipUndetermined()))
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

            game.Grid.AssertEqual(
                new BimaruValue[5, 5]
                {
                    { BimaruValue.SHIP_CONT_RIGHT  , BimaruValue.SHIP_MIDDLE , BimaruValue.SHIP_MIDDLE   , BimaruValue.SHIP_CONT_LEFT, BimaruValue.WATER        },
                    { BimaruValue.UNDETERMINED     , BimaruValue.UNDETERMINED, BimaruValue.WATER         , BimaruValue.UNDETERMINED  , BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED     , BimaruValue.UNDETERMINED, BimaruValue.SHIP_CONT_UP  , BimaruValue.UNDETERMINED  , BimaruValue.WATER },
                    { BimaruValue.UNDETERMINED     , BimaruValue.UNDETERMINED, BimaruValue.SHIP_CONT_DOWN, BimaruValue.WATER         , BimaruValue.SHIP_SINGLE  },
                    { BimaruValue.SHIP_UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.WATER         , BimaruValue.UNDETERMINED  , BimaruValue.WATER        },
                });
        }
    }
}
