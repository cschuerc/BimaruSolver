using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Utility;
using BimaruInterfaces;

namespace BimaruGame
{
    [TestClass]
    public class GridTests
    {
        [TestMethod]
        public void TestGetFieldValue()
        {
            int numRows = 4;
            int numColumns = 3;
            var grid = new Grid(numRows, numColumns);

            var pointOutOfGrid0 = new GridPoint(0, -1);
            var pointOutOfGrid1 = new GridPoint(-1, 0);
            var pointOutOfGrid2 = new GridPoint(numRows, 0);
            var pointOutOfGrid3 = new GridPoint(0, numColumns);
            var pointOutOfGrid4 = new GridPoint(100, 2);

            var pointInGrid0 = new GridPoint(3, 2);
            var pointInGrid1 = new GridPoint(0, 1);

            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(pointOutOfGrid0));
            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(pointOutOfGrid1));
            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(pointOutOfGrid2));
            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(pointOutOfGrid3));
            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(pointOutOfGrid4));

            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(pointInGrid0));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(pointInGrid1));
        }

        [TestMethod]
        public void TestSetFieldValue()
        {
            int numRows = 4;
            int numColumns = 3;
            var grid = new Grid(numRows, numColumns);

            var pointOutOfGrid0 = new GridPoint(0, -1);

            var pointInGrid0 = new GridPoint(0, 0);

            grid.SetFieldValue(pointOutOfGrid0, BimaruValue.WATER); // WATER is okay, as off-grid values are WATER
            Assert.ThrowsException<InvalidFieldChange>(() => grid.SetFieldValue(pointOutOfGrid0, BimaruValue.UNDETERMINED));
            Assert.ThrowsException<InvalidFieldChange>(() => grid.SetFieldValue(pointOutOfGrid0, BimaruValue.SHIP_SINGLE));

            grid.SetFieldValue(pointInGrid0, BimaruValue.UNDETERMINED);
            grid.SetFieldValue(pointInGrid0, BimaruValue.SHIP_CONT_LEFT);
        }

        [TestMethod]
        public void TestFieldCount()
        {
            int numRows = 4;
            int numColumns = 3;
            var grid = new Grid(numRows, numColumns);

            Assert.IsTrue(grid.GetNumUndeterminedFieldsColumn.SequenceEqual(new int[numColumns].InitValues(numRows)));
            Assert.IsTrue(grid.GetNumShipFieldsColumn.SequenceEqual(new int[numColumns].InitValues(0)));
            Assert.IsTrue(grid.GetNumUndeterminedFieldsRow.SequenceEqual(new int[numRows].InitValues(numColumns)));
            Assert.IsTrue(grid.GetNumShipFieldsRow.SequenceEqual(new int[numRows].InitValues(0)));

            // SUW
            // UUS
            // UUU
            // SUU
            grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_SINGLE);
            grid.SetFieldValue(new GridPoint(3, 2), BimaruValue.WATER);
            grid.SetFieldValue(new GridPoint(3, 0), BimaruValue.SHIP_MIDDLE);
            grid.SetFieldValue(new GridPoint(2, 2), BimaruValue.SHIP_SINGLE);
            grid.SetFieldValue(new GridPoint(2, 2), BimaruValue.WATER);
            grid.SetFieldValue(new GridPoint(2, 2), BimaruValue.SHIP_UNDETERMINED);

            Assert.IsTrue(grid.GetNumUndeterminedFieldsColumn.SequenceEqual(new int[] { 2, 4, 2 }));
            Assert.IsTrue(grid.GetNumShipFieldsColumn.SequenceEqual(new int[] { 2, 0, 1 }));
            Assert.IsTrue(grid.GetNumUndeterminedFieldsRow.SequenceEqual(new int[] { 2, 3, 2, 1 }));
            Assert.IsTrue(grid.GetNumShipFieldsRow.SequenceEqual(new int[] { 1, 0, 1, 1 }));
        }

        [TestMethod]
        public void TestShipCountOne()
        {
            int numRows = 4;
            int numColumns = 3;

            var grid = new Grid(numRows, numColumns);

            // SWU
            // UUS
            // UUU
            // SUU
            grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_SINGLE);
            grid.SetFieldValue(new GridPoint(3, 0), BimaruValue.SHIP_CONT_DOWN);
            grid.SetFieldValue(new GridPoint(3, 1), BimaruValue.WATER);
            grid.SetFieldValue(new GridPoint(2, 2), BimaruValue.SHIP_SINGLE);

            Assert.IsTrue(new int[] { 0, 2, 0, 0, 0 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestShipCountTwo()
        {
            int numRows = 4;
            int numColumns = 3;

            var grid = new Grid(numRows, numColumns);

            // SWU
            // SUU
            // USS
            // UUU
            grid.SetFieldValue(new GridPoint(2, 0), BimaruValue.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(3, 0), BimaruValue.SHIP_CONT_DOWN);

            grid.SetFieldValue(new GridPoint(3, 1), BimaruValue.WATER);

            grid.SetFieldValue(new GridPoint(1, 1), BimaruValue.SHIP_CONT_RIGHT);
            grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.SHIP_CONT_LEFT);

            Assert.IsTrue(new int[] { 0, 0, 2, 0, 0 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestShipCountThree()
        {
            int numRows = 4;
            int numColumns = 3;

            var grid = new Grid(numRows, numColumns);

            // UUU
            // SUS
            // SUS
            // SUS
            grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.SHIP_UNDETERMINED);
            grid.SetFieldValue(new GridPoint(2, 0), BimaruValue.SHIP_CONT_DOWN);

            grid.SetFieldValue(new GridPoint(0, 2), BimaruValue.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.SHIP_MIDDLE);
            grid.SetFieldValue(new GridPoint(2, 2), BimaruValue.SHIP_CONT_DOWN);

            Assert.IsTrue(new int[] { 0, 0, 0, 1, 0 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestShipCountFour()
        {
            int numRows = 4;
            int numColumns = 3;

            var grid = new Grid(numRows, numColumns);

            // UUS
            // UUS
            // SUS
            // SUS
            grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.SHIP_MIDDLE);

            grid.SetFieldValue(new GridPoint(0, 2), BimaruValue.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.SHIP_MIDDLE);
            grid.SetFieldValue(new GridPoint(2, 2), BimaruValue.SHIP_MIDDLE);
            grid.SetFieldValue(new GridPoint(3, 2), BimaruValue.SHIP_CONT_DOWN);

            Assert.IsTrue(new int[] { 0, 0, 0, 0, 1 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestShipCountChaos()
        {
            int numRows = 3;
            int numColumns = 3;

            var grid = new Grid(numRows, numColumns);

            // USU
            // SSS
            // USU
            grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.SHIP_CONT_UP);

            grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.SHIP_CONT_RIGHT);
            grid.SetFieldValue(new GridPoint(1, 1), BimaruValue.SHIP_MIDDLE);
            grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.SHIP_CONT_LEFT);

            grid.SetFieldValue(new GridPoint(2, 1), BimaruValue.SHIP_CONT_DOWN);

            Assert.IsTrue(new int[] { 0, 0, 0, 2 }.SequenceEqual(grid.GetNumShips));

            grid.SetFieldValue(new GridPoint(1, 1), BimaruValue.SHIP_SINGLE);

            Assert.IsTrue(new int[] { 0, 1, 0, 0 }.SequenceEqual(grid.GetNumShips));

            grid.SetFieldValue(new GridPoint(1, 1), BimaruValue.SHIP_CONT_RIGHT);

            Assert.IsTrue(new int[] { 0, 0, 1, 0 }.SequenceEqual(grid.GetNumShips));

            grid.SetFieldValue(new GridPoint(1, 1), BimaruValue.SHIP_MIDDLE);

            Assert.IsTrue(new int[] { 0, 0, 0, 2 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestClone()
        {
            int numRows = 1;
            int numColumns = 2;
            var grid = new Grid(numRows, numColumns);

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);
            BimaruValue valueP0 = BimaruValue.SHIP_SINGLE;
            BimaruValue valueP1 = BimaruValue.WATER;

            grid.SetFieldValue(p0, valueP0);
            grid.SetFieldValue(p1, valueP1);

            Grid clonedGrid = (Grid)grid.Clone();

            Assert.AreEqual(numRows, clonedGrid.NumRows);
            Assert.AreEqual(numColumns, clonedGrid.NumColumns);

            Assert.AreEqual(valueP0, clonedGrid.GetFieldValue(p0));
            Assert.AreEqual(valueP1, clonedGrid.GetFieldValue(p1));

            Assert.IsTrue(grid.GetNumUndeterminedFieldsColumn.SequenceEqual(clonedGrid.GetNumUndeterminedFieldsColumn));
            Assert.IsTrue(grid.GetNumUndeterminedFieldsRow.SequenceEqual(clonedGrid.GetNumUndeterminedFieldsRow));
            Assert.IsTrue(grid.GetNumShipFieldsColumn.SequenceEqual(clonedGrid.GetNumShipFieldsColumn));
            Assert.IsTrue(grid.GetNumShipFieldsRow.SequenceEqual(clonedGrid.GetNumShipFieldsRow));

            Assert.IsTrue(grid.GetNumShips.SequenceEqual(clonedGrid.GetNumShips));

            grid.SetFieldValue(p0, BimaruValue.UNDETERMINED);

            Assert.AreEqual(valueP0, clonedGrid.GetFieldValue(p0));

            Assert.IsFalse(grid.GetNumUndeterminedFieldsColumn.SequenceEqual(clonedGrid.GetNumUndeterminedFieldsColumn));
            Assert.IsFalse(grid.GetNumUndeterminedFieldsRow.SequenceEqual(clonedGrid.GetNumUndeterminedFieldsRow));
            Assert.IsFalse(grid.GetNumShipFieldsColumn.SequenceEqual(clonedGrid.GetNumShipFieldsColumn));
            Assert.IsFalse(grid.GetNumShipFieldsRow.SequenceEqual(clonedGrid.GetNumShipFieldsRow));

            Assert.IsFalse(grid.GetNumShips.SequenceEqual(clonedGrid.GetNumShips));
        }

        [TestMethod]
        public void TestIsFullyDetermined()
        {
            int numRows = 2;
            int numColumns = 3;
            var grid = new Grid(numRows, numColumns);

            Assert.IsFalse(grid.IsFullyDetermined);

            grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.WATER);
            grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.SHIP_CONT_DOWN);
            grid.SetFieldValue(new GridPoint(0, 2), BimaruValue.SHIP_SINGLE);
            grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.SHIP_UNDETERMINED);
            grid.SetFieldValue(new GridPoint(1, 1), BimaruValue.UNDETERMINED);
            grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.SHIP_MIDDLE);

            Assert.IsFalse(grid.IsFullyDetermined);

            grid.SetFieldValue(new GridPoint(1, 1), BimaruValue.SHIP_CONT_RIGHT);

            Assert.IsFalse(grid.IsFullyDetermined);

            grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.SHIP_CONT_LEFT);

            Assert.IsTrue(grid.IsFullyDetermined);

            grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.UNDETERMINED);

            Assert.IsFalse(grid.IsFullyDetermined);

            grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.WATER);

            Assert.IsTrue(grid.IsFullyDetermined);
        }

        [TestMethod]
        public void TestIsValid()
        {
            int numRows = 2;
            int numColumns = 3;
            var grid = new Grid(numRows, numColumns);

            Assert.IsTrue(grid.IsValid);

            grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.WATER);
            Assert.IsTrue(grid.IsValid);
            grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.SHIP_CONT_DOWN);
            Assert.IsFalse(grid.IsValid);
            grid.SetFieldValue(new GridPoint(0, 2), BimaruValue.SHIP_SINGLE);
            Assert.IsFalse(grid.IsValid);
            grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.SHIP_UNDETERMINED);
            Assert.IsFalse(grid.IsValid);
            grid.SetFieldValue(new GridPoint(1, 1), BimaruValue.UNDETERMINED);
            Assert.IsFalse(grid.IsValid);
            grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.SHIP_MIDDLE);
            Assert.IsFalse(grid.IsValid);

            // S? ?? SM
            // WW /\  o

            grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.WATER);
            Assert.IsFalse(grid.IsValid);
            grid.SetFieldValue(new GridPoint(0, 2), BimaruValue.SHIP_CONT_UP);
            Assert.IsTrue(grid.IsValid);

            // S? ?? SM
            // WW WW \/

            grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_SINGLE);
            Assert.IsFalse(grid.IsValid);
            grid.SetFieldValue(new GridPoint(1, 0), BimaruValue.WATER);
            Assert.IsTrue(grid.IsValid);

            // WW ?? SM
            //  o WW \/

            grid.SetFieldValue(new GridPoint(1, 2), BimaruValue.UNDETERMINED);
            Assert.IsTrue(grid.IsValid);
            grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.UNDETERMINED);
            Assert.IsTrue(grid.IsValid);
            grid.SetFieldValue(new GridPoint(1, 1), BimaruValue.SHIP_CONT_DOWN);
            Assert.IsFalse(grid.IsValid);

            // WW /\ ??
            //  o ?? \/
        }

        [TestMethod]
        public void TestFillUndeterminedFieldsColumn()
        {
            int numRows = 2;
            int numColumns = 3;
            var grid = new Grid(numRows, numColumns);

            grid.FillUndeterminedFieldsColumn(1, BimaruValueConstraint.NO);

            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 2)));


            grid.FillUndeterminedFieldsColumn(1, BimaruValueConstraint.WATER);

            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 2)));

            grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.UNDETERMINED);
            grid.FillUndeterminedFieldsColumn(1, BimaruValueConstraint.SHIP);

            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 2)));

            grid.FillUndeterminedFieldsColumn(1, BimaruValueConstraint.SHIP);

            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 2)));
        }

        [TestMethod]
        public void TestFillUndeterminedFieldsRow()
        {
            int numRows = 2;
            int numColumns = 3;
            var grid = new Grid(numRows, numColumns);

            grid.FillUndeterminedFieldsRow(0, BimaruValueConstraint.NO);

            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 2)));


            grid.FillUndeterminedFieldsRow(0, BimaruValueConstraint.WATER);

            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 2)));


            grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.UNDETERMINED);
            grid.FillUndeterminedFieldsRow(0, BimaruValueConstraint.SHIP);

            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 2)));


            grid.FillUndeterminedFieldsRow(0, BimaruValueConstraint.SHIP);

            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, grid.GetFieldValue(new GridPoint(0, 2)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid.GetFieldValue(new GridPoint(1, 2)));
        }
    }
}
