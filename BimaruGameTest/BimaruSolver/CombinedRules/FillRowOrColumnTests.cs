using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace BimaruSolver
{
    [TestClass]
    public class FillRowOrColumnTests
    {
        [TestMethod]
        public void TestFullGridRules()
        {
            var game = (new GameFactory()).GenerateEmptyGame(2, 3);
            game.RowTally[0] = 2;
            game.RowTally[1] = 1;
            game.ColumnTally[0] = 1;
            game.ColumnTally[2] = 2;
            game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_CONT_RIGHT;

            //   102
            //   ---
            // 1|???
            // 2|??S

            var shipsFillRule = new FillRowOrColumnWithShips();
            shipsFillRule.Solve(game);

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.SHIP_CONT_RIGHT, game.Grid[new GridPoint(0, 2)]);

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 1)]);
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid[new GridPoint(1, 2)]);

            var waterFillRule = new FillRowOrColumnWithWater();
            waterFillRule.Solve(game);

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.SHIP_CONT_RIGHT, game.Grid[new GridPoint(0, 2)]);

            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(1, 1)]);
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid[new GridPoint(1, 2)]);
        }

        [TestMethod]
        public void TestFieldChangedRules()
        {
            var game = (new GameFactory()).GenerateEmptyGame(2, 3);
            game.RowTally[0] = 2;
            game.RowTally[1] = 1;
            game.ColumnTally[0] = 1;
            game.ColumnTally[2] = 2;

            //   102
            //   ---
            // 1|???
            // 2|???

            var p00 = new GridPoint(0, 0);
            using (var subscriber = new ChangedRuleSubscriber(game, new FillRowOrColumnWithWater()))
            {
                game.Grid[p00] = BimaruValue.SHIP_UNDETERMINED;
            }

            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(0, 2)]);

            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 1)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 2)]);


            var p12 = new GridPoint(1, 2);
            using (var subscriber = new ChangedRuleSubscriber(game, new FillRowOrColumnWithShips()))
            {
                game.Grid[p12] = BimaruValue.SHIP_UNDETERMINED;
            }

            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid[new GridPoint(0, 2)]);

            Assert.AreEqual(BimaruValue.WATER, game.Grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid[new GridPoint(1, 1)]);
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid[new GridPoint(1, 2)]);
        }
    }
}
