using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Utility;

namespace BimaruTest
{
    [TestClass]
    public class GameTests
    {
        private Game SetupGame(int numRows, int numColumns)
        {
            var rowTally = new Tally(numRows);
            Tally columnTally = new Tally(numColumns);
            ShipSettings settings = new ShipSettings();
            Grid initialGrid = new Grid(numRows, numColumns);
            RollbackGrid grid = new RollbackGrid(initialGrid);

            return new Game(rowTally, columnTally, settings, grid);
        }

        [TestMethod]
        public void TestNullArguments()
        {
            int numRows = 4;
            int numColumns = 3;

            var rowTally = new Tally(numRows);
            Tally columnTally = new Tally(numColumns);
            ShipSettings settings = new ShipSettings();
            Grid initialGrid = new Grid(numRows, numColumns);
            RollbackGrid grid = new RollbackGrid(initialGrid);

            Assert.ThrowsException<ArgumentNullException>(() => new Game(null, columnTally, settings, grid));
            Assert.ThrowsException<ArgumentNullException>(() => new Game(rowTally, null, settings, grid));
            Assert.ThrowsException<ArgumentNullException>(() => new Game(rowTally, columnTally, null, grid));
            Assert.ThrowsException<ArgumentNullException>(() => new Game(rowTally, columnTally, settings, null));
        }

        [TestMethod]
        public void TestTallyGridMismatch()
        {
            int numRows = 4;
            int numColumns = 3;

            var rowTally = new Tally(numRows);
            Tally columnTally = new Tally(numColumns);
            ShipSettings settings = new ShipSettings();
            Grid initialGrid;
            RollbackGrid grid;

            initialGrid = new Grid(numRows + 1, numColumns);
            grid = new RollbackGrid(initialGrid);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Game(rowTally, columnTally, settings, grid));


            initialGrid = new Grid(numRows, numColumns + 1);
            grid = new RollbackGrid(initialGrid);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Game(rowTally, columnTally, settings, grid));


            initialGrid = new Grid(1, numColumns);
            grid = new RollbackGrid(initialGrid);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Game(rowTally, columnTally, settings, grid));


            initialGrid = new Grid(numRows, 1);
            grid = new RollbackGrid(initialGrid);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Game(rowTally, columnTally, settings, grid));
        }

        [TestMethod]
        public void TestUnsolvability()
        {
            int numRows = 4;
            int numColumns = 3;

            Game game = SetupGame(numRows, numColumns);

            Assert.IsFalse(game.IsUnsolvable);

            game.RowTally[0] = 1;
            game.RowTally[1] = 1;
            game.RowTally[3] = 1;

            Assert.IsTrue(game.IsUnsolvable);

            game.ColumnTally[0] = 3;

            Assert.IsTrue(game.IsUnsolvable);

            game.Settings[1] = 1;

            Assert.IsTrue(game.IsUnsolvable);

            game.Settings[2] = 1;

            Assert.IsFalse(game.IsUnsolvable);
        }

        [TestMethod]
        public void TestMissingShipFields()
        {
            int numRows = 4;
            int numColumns = 3;

            Game game = SetupGame(numRows, numColumns);

            game.Settings[1] = 1;
            game.Settings[2] = 1;

            game.RowTally[0] = 1;
            game.RowTally[1] = 1;
            game.RowTally[3] = 1;

            game.ColumnTally[0] = 3;

            game.Grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_CONT_UP);
            game.Grid.SetFieldValue(new GridPoint(0, 2), FieldValues.SHIP_UNDETERMINED);

            //  300
            // 1???
            // 0???
            // 1???
            // 1S?S

            Assert.AreEqual(-1, game.MissingShipFieldsRow(0));
            Assert.AreEqual(1, game.MissingShipFieldsRow(1));
            Assert.AreEqual(0, game.MissingShipFieldsRow(2));
            Assert.AreEqual(1, game.MissingShipFieldsRow(3));

            Assert.AreEqual(2, game.MissingShipFieldsColumn(0));
            Assert.AreEqual(0, game.MissingShipFieldsColumn(1));
            Assert.AreEqual(-1, game.MissingShipFieldsColumn(2));
        }

        [TestMethod]
        public void TestIsValid()
        {
            int numRows = 4;
            int numColumns = 3;

            Game game = SetupGame(numRows, numColumns);

            Assert.IsTrue(game.IsValid);

            game.Settings[1] = 1;

            Assert.IsFalse(game.IsValid);

            game.RowTally[0] = 1;

            Assert.IsFalse(game.IsValid);

            game.ColumnTally[2] = 1;

            Assert.IsTrue(game.IsValid);

            game.Grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_SINGLE);

            Assert.IsFalse(game.IsValid);

            game.Grid.SetFieldValue(new GridPoint(0, 0), FieldValues.WATER);

            Assert.IsTrue(game.IsValid);

            game.Grid.SetFieldValue(new GridPoint(0, 1), FieldValues.WATER);

            Assert.IsTrue(game.IsValid);

            game.Grid.SetFieldValue(new GridPoint(0, 2), FieldValues.WATER);

            Assert.IsFalse(game.IsValid);

            game.Grid.SetFieldValue(new GridPoint(0, 2), FieldValues.SHIP_SINGLE);

            Assert.IsTrue(game.IsValid);

            game.Grid.SetFieldValue(new GridPoint(0, 2), FieldValues.SHIP_CONT_DOWN);

            Assert.IsFalse(game.IsValid);
        }

        [TestMethod]
        public void TestIsSolved()
        {
            int numRows = 4;
            int numColumns = 3;

            Game game = SetupGame(numRows, numColumns);

            Assert.IsFalse(game.IsSolved);

            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < numColumns; columnIndex++)
                {
                    GridPoint point = new GridPoint(rowIndex, columnIndex);
                    game.Grid.SetFieldValue(point, FieldValues.WATER);
                }
            }

            Assert.IsTrue(game.IsSolved);

            game.RowTally[0] = 2;
            game.RowTally[1] = 2;
            game.RowTally[2] = 1;
            game.RowTally[3] = 2;

            Assert.IsFalse(game.IsSolved);

            game.ColumnTally[0] = 3;
            game.ColumnTally[2] = 4;

            Assert.IsFalse(game.IsSolved);

            game.Settings[1] = 1;
            game.Settings[2] = 1;
            game.Settings[4] = 1;

            //1xSUBMARINE, 1xDESTROYER, 1xBATTLESHIP
            //   304
            //
            // 2 WWW
            // 1 WWW
            // 2 WWW
            // 2 WWW

            Assert.IsFalse(game.IsSolved);

            game.Grid.SetFieldValue(new GridPoint(3, 0), FieldValues.SHIP_SINGLE);

            game.Grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_CONT_UP);
            game.Grid.SetFieldValue(new GridPoint(1, 0), FieldValues.SHIP_CONT_DOWN);

            game.Grid.SetFieldValue(new GridPoint(0, 2), FieldValues.SHIP_CONT_UP);
            game.Grid.SetFieldValue(new GridPoint(1, 2), FieldValues.SHIP_MIDDLE);
            game.Grid.SetFieldValue(new GridPoint(2, 2), FieldValues.SHIP_MIDDLE);
            game.Grid.SetFieldValue(new GridPoint(3, 2), FieldValues.SHIP_CONT_DOWN);

            Assert.IsTrue(game.IsSolved);

            game.Grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_SINGLE);
            game.Grid.SetFieldValue(new GridPoint(1, 0), FieldValues.SHIP_SINGLE);
            Assert.IsFalse(game.IsSolved);
            game.Grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_CONT_UP);
            game.Grid.SetFieldValue(new GridPoint(1, 0), FieldValues.SHIP_CONT_DOWN);
            Assert.IsTrue(game.IsSolved);

            game.Grid.SetFieldValue(new GridPoint(1, 2), FieldValues.SHIP_UNDETERMINED);
            Assert.IsFalse(game.IsSolved);
            game.Grid.SetFieldValue(new GridPoint(1, 2), FieldValues.SHIP_MIDDLE);
            Assert.IsTrue(game.IsSolved);

            game.Settings[1] = 0;
            Assert.IsFalse(game.IsSolved);
            game.Settings[1] = 1;
            Assert.IsTrue(game.IsSolved);

            game.ColumnTally[1] = 1;
            Assert.IsFalse(game.IsSolved);
            game.ColumnTally[1] = 0;
            Assert.IsTrue(game.IsSolved);

            game.RowTally[1] = 1;
            Assert.IsFalse(game.IsSolved);
            game.RowTally[1] = 2;
            Assert.IsTrue(game.IsSolved);
        }
    }
}
