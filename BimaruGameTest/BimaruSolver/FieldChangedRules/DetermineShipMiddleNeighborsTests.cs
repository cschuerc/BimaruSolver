using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Bimaru.SolverUtil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace Bimaru.Test
{
    [TestClass]
    public class DetermineShipMiddleNeighborsTests
    {
        [TestMethod]
        public void TestShipMiddleNeighborsUnknown()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new DetermineShipMiddleNeighbors()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_MIDDLE;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.SHIP_MIDDLE, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED }
                });
        }

        [TestMethod]
        public void TestShipMiddleNeighborsWater()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new DetermineShipMiddleNeighbors()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_MIDDLE;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.WATER;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER },
                    { BimaruValue.SHIP_UNDETERMINED, BimaruValue.SHIP_MIDDLE, BimaruValue.SHIP_UNDETERMINED },
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER }
                });
        }

        [TestMethod]
        public void TestShipMiddleNeighborsShip()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new DetermineShipMiddleNeighbors()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_MIDDLE;
                game.Grid[new GridPoint(2, 1)] = BimaruValue.SHIP_UNDETERMINED;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.WATER, BimaruValue.SHIP_UNDETERMINED, BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.SHIP_MIDDLE      , BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.SHIP_UNDETERMINED, BimaruValue.WATER }
                });
        }
    }
}
