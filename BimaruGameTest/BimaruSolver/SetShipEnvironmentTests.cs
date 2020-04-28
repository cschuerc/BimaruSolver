using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace BimaruSolver
{
    [TestClass]
    public class SetShipEnvironmentTests
    {
        private static Game SetupGame(int numRows, int numColumns)
        {
            var rowTally = new Tally(numRows);
            Tally columnTally = new Tally(numColumns);
            ShipSettings settings = new ShipSettings();
            Grid initialGrid = new Grid(numRows, numColumns);
            RollbackGrid grid = new RollbackGrid(initialGrid);

            return new Game(rowTally, columnTally, settings, grid);
        }

        [TestMethod]
        public void TestWater()
        {
            int numRows = 3;
            int numColumns = 3;

            var game = SetupGame(numRows, numColumns);
            var p11 = new GridPoint(1, 1);
            game.Grid.SetFieldValue(p11, BimaruValue.WATER);

            var rule = new SetShipEnvironment();
            rule.FieldValueChanged(game, new FieldValueChangedEventArgs<BimaruValue>(p11, BimaruValue.UNDETERMINED));

            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(p11));

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(2, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(2, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(2, 1)));
        }

        [TestMethod]
        public void TestSingleShip()
        {
            int numRows = 3;
            int numColumns = 3;

            var game = SetupGame(numRows, numColumns);

            game.RowTally[1] = 1;
            game.ColumnTally[1] = 1;
            game.ShipSettings[1] = 1;
            var p11 = new GridPoint(1, 1);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_SINGLE);

            // 1xBATTLESHIP
            //   010
            //   ---
            // 0|???
            // 1|?S?
            // 0|???

            var rule = new SetShipEnvironment();
            rule.FieldValueChanged(game, new FieldValueChangedEventArgs<BimaruValue>(p11, BimaruValue.UNDETERMINED));

            Assert.IsTrue(game.IsSolved);
        }

        [TestMethod]
        public void TestMiddleShip()
        {
            int numRows = 3;
            int numColumns = 3;

            var game = SetupGame(numRows, numColumns);

            var p11 = new GridPoint(1, 1);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_MIDDLE);


            var rule = new SetShipEnvironment();
            rule.FieldValueChanged(game, new FieldValueChangedEventArgs<BimaruValue>(p11, BimaruValue.UNDETERMINED));

            Assert.IsFalse(game.IsSolved);

            Assert.AreEqual(BimaruValue.SHIP_MIDDLE, game.Grid.GetFieldValue(p11));

            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(2, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(2, 2)));

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(2, 1)));
        }

        [TestMethod]
        public void TestShipUndet()
        {
            int numRows = 3;
            int numColumns = 3;

            var game = SetupGame(numRows, numColumns);

            var p11 = new GridPoint(1, 1);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_UNDETERMINED);

            var rule = new SetShipEnvironment();
            rule.FieldValueChanged(game, new FieldValueChangedEventArgs<BimaruValue>(p11, BimaruValue.UNDETERMINED));

            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid.GetFieldValue(p11));

            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(2, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(2, 2)));

            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(2, 1)));
        }

        [TestMethod]
        public void TestShipCont()
        {
            int numRows = 3;
            int numColumns = 3;

            var game = SetupGame(numRows, numColumns);
            var p11 = new GridPoint(1, 1);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_CONT_RIGHT);

            var rule = new SetShipEnvironment();
            rule.FieldValueChanged(game, new FieldValueChangedEventArgs<BimaruValue>(p11, BimaruValue.UNDETERMINED));

            Assert.AreEqual(BimaruValue.SHIP_CONT_RIGHT, game.Grid.GetFieldValue(p11));

            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(2, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(2, 2)));

            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(2, 1)));

            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 2)));
        }

        [TestMethod]
        public void TestShipContOverwrite()
        {
            int numRows = 3;
            int numColumns = 3;

            var game = SetupGame(numRows, numColumns);

            game.RowTally[1] = 2;
            game.ColumnTally[1] = 1;
            game.ColumnTally[2] = 1;
            game.ShipSettings[2] = 1;
            var p11 = new GridPoint(1, 1);
            game.Grid.SetFieldValue(p11, BimaruValue.SHIP_CONT_RIGHT);
            var p12 = new GridPoint(1, 2);
            game.Grid.SetFieldValue(p12, BimaruValue.SHIP_CONT_LEFT);

            // 1xDESTROYER
            //   011
            //   ---
            // 0|???
            // 2|?SS
            // 0|???

            var rule = new SetShipEnvironment();
            rule.FieldValueChanged(game, new FieldValueChangedEventArgs<BimaruValue>(p11, BimaruValue.UNDETERMINED));
            rule.FieldValueChanged(game, new FieldValueChangedEventArgs<BimaruValue>(p12, BimaruValue.UNDETERMINED));

            Assert.IsTrue(game.IsSolved);
        }
    }
}
