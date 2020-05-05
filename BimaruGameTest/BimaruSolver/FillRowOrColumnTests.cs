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
            int numRows = 2;
            int numColumns = 3;

            var game = (new GameFactory()).GenerateEmptyGame(numRows, numColumns);
            game.RowTally[0] = 2;
            game.RowTally[1] = 1;
            game.ColumnTally[0] = 1;
            game.ColumnTally[2] = 2;
            game.Grid.SetFieldValue(new GridPoint(0, 2), BimaruValue.SHIP_CONT_RIGHT);

            //   102
            //   ---
            // 1|???
            // 2|??S

            var shipsFillRule = new FillRowOrColumnWithShips();
            shipsFillRule.Solve(game);

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.SHIP_CONT_RIGHT, game.Grid.GetFieldValue(new GridPoint(0, 2)));

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 2)));

            var waterFillRule = new FillRowOrColumnWithWater();
            waterFillRule.Solve(game);

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.SHIP_CONT_RIGHT, game.Grid.GetFieldValue(new GridPoint(0, 2)));

            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 2)));
        }

        [TestMethod]
        public void TestFieldChangedRules()
        {
            int numRows = 2;
            int numColumns = 3;

            var game = (new GameFactory()).GenerateEmptyGame(numRows, numColumns);
            game.RowTally[0] = 2;
            game.RowTally[1] = 1;
            game.ColumnTally[0] = 1;
            game.ColumnTally[2] = 2;

            //   102
            //   ---
            // 1|???
            // 2|???

            var waterFillRule = new FillRowOrColumnWithWater();

            var p00 = new GridPoint(0, 0);
            game.Grid.SetFieldValue(p00, BimaruValue.SHIP_UNDETERMINED);
            waterFillRule.FieldValueChanged(game, new FieldValueChangedEventArgs<BimaruValue>(p00, BimaruValue.UNDETERMINED));

            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 2)));

            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 2)));

            var shipsFillRule = new FillRowOrColumnWithShips();

            var p12 = new GridPoint(1, 2);
            game.Grid.SetFieldValue(p12, BimaruValue.SHIP_UNDETERMINED);
            shipsFillRule.FieldValueChanged(game, new FieldValueChangedEventArgs<BimaruValue>(p12, BimaruValue.UNDETERMINED));

            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 2)));

            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 2)));
        }
    }
}
