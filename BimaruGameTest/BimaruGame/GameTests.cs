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
            int numRows = 4;
            int numColumns = 3;

            IGame game = (new GameFactory()).GenerateEmptyGame(numRows, numColumns);

            game.ShipSettings[1] = 1;
            game.ShipSettings[2] = 1;

            game.RowTally[0] = 1;
            game.RowTally[1] = 1;
            game.RowTally[3] = 1;

            game.ColumnTally[0] = 3;

            game.Grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_CONT_UP);
            game.Grid.SetFieldValue(new GridPoint(0, 2), BimaruValue.SHIP_UNDETERMINED);
            game.Grid.SetFieldValue(new GridPoint(3, 0), BimaruValue.WATER);

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
            int numRows = 4;
            int numColumns = 3;

            IGame game = (new GameFactory()).GenerateEmptyGame(numRows, numColumns);

            Assert.IsFalse(game.IsUnsolvable);

            game.RowTally[0] = 3;
            game.RowTally[1] = 1;
            game.RowTally[3] = 3;

            Assert.IsTrue(game.IsUnsolvable);

            game.ColumnTally[0] = 4;
            game.ColumnTally[2] = 3;

            Assert.IsTrue(game.IsUnsolvable);

            game.ShipSettings[2] = 2;

            Assert.IsTrue(game.IsUnsolvable);

            game.ShipSettings[3] = 1;

            Assert.IsFalse(game.IsUnsolvable);

            game.ShipSettings[5] = 1;
            game.ShipSettings[2] = 1;

            Assert.IsTrue(game.IsUnsolvable);
        }

        [TestMethod]
        public void TestValidity()
        {
            int numRows = 4;
            int numColumns = 3;

            IGame game = (new GameFactory()).GenerateEmptyGame(numRows, numColumns);

            Assert.IsTrue(game.IsValid);

            game.ShipSettings[1] = 1;

            Assert.IsFalse(game.IsValid);

            game.RowTally[0] = 1;

            Assert.IsFalse(game.IsValid);

            game.ColumnTally[2] = 1;

            Assert.IsTrue(game.IsValid);

            // 1xSUBMARINE
            //   001
            //   ---
            // 0|???
            // 0|???
            // 0|???
            // 1|???

            game.Grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_SINGLE);

            Assert.IsFalse(game.IsValid);

            game.Grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.WATER);

            Assert.IsTrue(game.IsValid);

            game.Grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.WATER);

            Assert.IsTrue(game.IsValid);

            game.Grid.SetFieldValue(new GridPoint(0, 2), BimaruValue.WATER);

            Assert.IsFalse(game.IsValid);

            game.Grid.SetFieldValue(new GridPoint(0, 2), BimaruValue.SHIP_SINGLE);

            Assert.IsTrue(game.IsValid);

            game.Grid.SetFieldValue(new GridPoint(0, 2), BimaruValue.SHIP_CONT_DOWN);

            Assert.IsFalse(game.IsValid);
        }

        [TestMethod]
        public void TestSolvability()
        {
            int numRows = 4;
            int numColumns = 3;

            IGame game = (new GameFactory()).GenerateEmptyGame(numRows, numColumns);

            Assert.IsFalse(game.IsSolved);

            foreach (GridPoint p in game.Grid.AllPoints())
            {
                game.Grid.SetFieldValue(p, BimaruValue.WATER);
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

            game.ShipSettings[1] = 1;
            game.ShipSettings[2] = 1;
            game.ShipSettings[4] = 1;

            // 1xSUBMARINE, 1xDESTROYER, 1xBATTLESHIP
            //   304
            //
            // 2 WWW
            // 1 WWW
            // 2 WWW
            // 2 WWW

            Assert.IsFalse(game.IsSolved);

            game.Grid.SetFieldValue(new GridPoint(3, 0), BimaruValue.SHIP_SINGLE);

            game.Grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_CONT_UP);
            game.Grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.SHIP_CONT_DOWN);

            game.Grid.SetFieldValue(new GridPoint(0, 2), BimaruValue.SHIP_CONT_UP);
            game.Grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.SHIP_MIDDLE);
            game.Grid.SetFieldValue(new GridPoint(2, 2), BimaruValue.SHIP_MIDDLE);
            game.Grid.SetFieldValue(new GridPoint(3, 2), BimaruValue.SHIP_CONT_DOWN);

            Assert.IsTrue(game.IsSolved);

            game.Grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_SINGLE);
            game.Grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.SHIP_SINGLE);
            Assert.IsFalse(game.IsSolved);
            game.Grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_CONT_UP);
            game.Grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.SHIP_CONT_DOWN);
            Assert.IsTrue(game.IsSolved);

            game.Grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.SHIP_UNDETERMINED);
            Assert.IsFalse(game.IsSolved);
            game.Grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.SHIP_MIDDLE);
            Assert.IsTrue(game.IsSolved);

            game.ShipSettings[1] = 0;
            Assert.IsFalse(game.IsSolved);
            game.ShipSettings[1] = 1;
            Assert.IsTrue(game.IsSolved);

            game.ColumnTally[1] = 1;
            Assert.IsFalse(game.IsSolved);
            game.ColumnTally[1] = 0;
            Assert.IsTrue(game.IsSolved);

            game.RowTally[1] = 1;
            Assert.IsFalse(game.IsSolved);
            game.RowTally[1] = 2;
            Assert.IsTrue(game.IsSolved);
            game.Grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_MIDDLE);
            Assert.IsFalse(game.IsSolved);
        }
    }
}
