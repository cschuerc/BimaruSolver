using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Utility;

namespace BimaruGame
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void TestNullArguments()
        {
            int numRows = 4;
            int numColumns = 3;
            var rowTally = new Tally(numRows);
            Tally columnTally = new Tally(numColumns);
            ShipSettings settings = new ShipSettings();
            RollbackGrid grid = new RollbackGrid(numRows, numColumns);

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

            Tally rowTally;
            Tally columnTally;
            ShipSettings settings = new ShipSettings();
            RollbackGrid grid = new RollbackGrid(numRows, numColumns);

            rowTally = new Tally(numRows);
            columnTally = new Tally(numColumns);
            new Game(rowTally, columnTally, settings, grid);

            rowTally = new Tally(numRows + 1);
            columnTally = new Tally(numColumns);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Game(rowTally, columnTally, settings, grid));

            rowTally = new Tally(numRows);
            columnTally = new Tally(numColumns + 1);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Game(rowTally, columnTally, settings, grid));

            rowTally = new Tally(numRows - 1);
            columnTally = new Tally(numColumns);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Game(rowTally, columnTally, settings, grid));

            rowTally = new Tally(numRows);
            columnTally = new Tally(numColumns - 1);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Game(rowTally, columnTally, settings, grid));
        }

        [TestMethod]
        public void TestMissingShipFields()
        {
            Game game = (new GameFactory()).GenerateGame(4, 3);

            game.RowTally[0] = 1;
            game.RowTally[1] = 1;
            game.RowTally[3] = 1;

            game.ColumnTally[0] = 3;

            game.ShipSettings[1] = 1;
            game.ShipSettings[2] = 1;

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_UP;
            game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_UNDETERMINED;
            game.Grid[new GridPoint(3, 0)] = BimaruValue.WATER;

            // 1xDESTROYER, 1xSUBMARINE
            //   300
            //   ---
            // 1|W??
            // 0|???
            // 1|???
            // 1|S?S

            Assert.AreEqual(-1, game.MissingShipFieldsRow(0));
            Assert.AreEqual(1, game.MissingShipFieldsRow(1));
            Assert.AreEqual(0, game.MissingShipFieldsRow(2));
            Assert.AreEqual(1, game.MissingShipFieldsRow(3));

            Assert.AreEqual(2, game.MissingShipFieldsColumn(0));
            Assert.AreEqual(0, game.MissingShipFieldsColumn(1));
            Assert.AreEqual(-1, game.MissingShipFieldsColumn(2));
        }

        [TestMethod]
        public void TestUnsolvability()
        {
            Game game = (new GameFactory()).GenerateGame(4, 3);

            Assert.IsFalse(game.IsUnsolvable);

            game.RowTally[0] = 3;
            game.RowTally[1] = 1;
            game.RowTally[3] = 3;

            // Sum(RowTally) != Sum(ColumnTally)
            Assert.IsTrue(game.IsUnsolvable);

            game.ColumnTally[0] = 4;
            game.ColumnTally[2] = 3;

            // Sum(RowTally) != NumShipFields
            Assert.IsTrue(game.IsUnsolvable);

            game.ShipSettings[2] = 2;
            game.ShipSettings[3] = 1;

            Assert.IsFalse(game.IsUnsolvable);

            game.ColumnTally[0] = 5;
            game.ColumnTally[2] = 2;

            // ColumnTally[0] > numRows
            Assert.IsTrue(game.IsUnsolvable);

            game.ColumnTally[0] = 4;
            game.ColumnTally[2] = 3;

            Assert.IsFalse(game.IsUnsolvable);

            game.RowTally[0] = 4;
            game.RowTally[1] = 0;

            // RowTally[0] > numColumns
            Assert.IsTrue(game.IsUnsolvable);

            game.RowTally[0] = 3;
            game.RowTally[1] = 1;

            Assert.IsFalse(game.IsUnsolvable);

            game.ShipSettings[2] = 1;
            game.ShipSettings[3] = 0;
            game.ShipSettings[5] = 1;

            // LongestShipLength > Max(numRows, numColumns) = 4
            Assert.IsTrue(game.IsUnsolvable);
        }

        [TestMethod]
        public void TestValidity()
        {
            Game game = (new GameFactory()).GenerateGame(2, 3);

            Assert.IsTrue(game.IsValid);

            game.RowTally[0] = 2;
            game.ColumnTally[0] = 1;
            game.ColumnTally[2] = 1;

            // IsUnsolvable => Not valid
            Assert.IsFalse(game.IsValid);

            game.ShipSettings[2] = 1;

            Assert.IsTrue(game.IsValid);

            // 1xDESTROYER
            //   101
            //   ---
            // 0|???
            // 2|???

            game.Grid[new GridPoint(0, 1)] = BimaruValue.SHIP_CONT_RIGHT;

            // ColumnTally[0] violated
            Assert.IsFalse(game.IsValid);

            game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;

            Assert.IsTrue(game.IsValid);

            game.Grid[new GridPoint(0, 2)] = BimaruValue.WATER;

            // RowTally[0] violated
            Assert.IsFalse(game.IsValid);

            game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_UNDETERMINED;

            Assert.IsTrue(game.IsValid);

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_SINGLE;

            // Ship settings violated
            Assert.IsFalse(game.IsValid);

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_RIGHT;
            game.Grid[new GridPoint(0, 1)] = BimaruValue.UNDETERMINED;
            game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_CONT_LEFT;

            Assert.IsTrue(game.IsValid);

            game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;

            // Grid violated
            Assert.IsFalse(game.IsValid);
        }

        [TestMethod]
        public void TestSolvability()
        {
            Game game = (new GameFactory()).GenerateGame(4, 3);

            // Not fully determined
            Assert.IsFalse(game.IsSolved);

            game.RowTally[0] = 2;
            game.RowTally[1] = 2;
            game.RowTally[2] = 1;
            game.RowTally[3] = 2;

            game.ColumnTally[0] = 3;
            game.ColumnTally[2] = 4;

            game.ShipSettings[1] = 1;
            game.ShipSettings[2] = 1;
            game.ShipSettings[4] = 1;

            // 1xSUBMARINE, 1xDESTROYER, 1xBATTLESHIP
            //   304
            //   ---
            // 2|???
            // 1|???
            // 2|???
            // 2|???

            foreach (GridPoint p in game.Grid.AllPoints())
            {
                game.Grid[p] = BimaruValue.WATER;
            }

            // Row- and ColumnTally violated
            Assert.IsFalse(game.IsSolved);

            game.Grid[new GridPoint(3, 0)] = BimaruValue.SHIP_SINGLE;

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_UP;
            game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_CONT_DOWN;

            game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_CONT_UP;
            game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_MIDDLE;
            game.Grid[new GridPoint(2, 2)] = BimaruValue.SHIP_MIDDLE;
            game.Grid[new GridPoint(3, 2)] = BimaruValue.SHIP_CONT_DOWN;

            Assert.IsTrue(game.IsSolved);

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_SINGLE;

            // Grid is not valid and ship settings are violated
            Assert.IsFalse(game.IsSolved);

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_UP;
            game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_CONT_DOWN;

            Assert.IsTrue(game.IsSolved);

            game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_UNDETERMINED;

            // Grid not fully determined and ship settings are violated
            Assert.IsFalse(game.IsSolved);

            game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_MIDDLE;

            Assert.IsTrue(game.IsSolved);

            game.ShipSettings[1] = 0;

            // Unsolvable and ship settings violated
            Assert.IsFalse(game.IsSolved);

            game.ShipSettings[1] = 1;

            Assert.IsTrue(game.IsSolved);
        }
    }
}
