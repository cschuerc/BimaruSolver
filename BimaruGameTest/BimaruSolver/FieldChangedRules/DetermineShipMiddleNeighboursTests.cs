using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Bimaru.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace Bimaru.SolverUtil
{
    [TestClass]
    public class DetermineShipMiddleNeighboursTests
    {
        [TestMethod]
        public void TestShipMiddleNeighboursUnknown()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (var subscriber = new FieldValueChangedRuleSubscriber(game, new DetermineShipMiddleNeighbours()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_MIDDLE;
            }

            game.Grid.AssertEqual(
                new BimaruValue[3, 3]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.SHIP_MIDDLE, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED }
                });
        }

        [TestMethod]
        public void TestShipMiddleNeighboursWater()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (var subscriber = new FieldValueChangedRuleSubscriber(game, new DetermineShipMiddleNeighbours()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_MIDDLE;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.WATER;
            }

            game.Grid.AssertEqual(
                new BimaruValue[3, 3]
                {
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER },
                    { BimaruValue.SHIP_UNDETERMINED, BimaruValue.SHIP_MIDDLE, BimaruValue.SHIP_UNDETERMINED },
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER }
                });
        }

        [TestMethod]
        public void TestShipMiddleNeighboursShip()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (var subscriber = new FieldValueChangedRuleSubscriber(game, new DetermineShipMiddleNeighbours()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_MIDDLE;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.SHIP_UNDETERMINED;
            }

            game.Grid.AssertEqual(
                new BimaruValue[3, 3]
                {
                    { BimaruValue.WATER, BimaruValue.SHIP_UNDETERMINED, BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.SHIP_MIDDLE      , BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.SHIP_UNDETERMINED, BimaruValue.WATER }
                });
        }
    }
}
