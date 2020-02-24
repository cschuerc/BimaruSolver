using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimaru;
using System.Linq;

namespace BimaruTest
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

            Assert.AreEqual(FieldValues.WATER, grid.GetFieldValue(pointOutOfGrid0));
            Assert.AreEqual(FieldValues.WATER, grid.GetFieldValue(pointOutOfGrid1));
            Assert.AreEqual(FieldValues.WATER, grid.GetFieldValue(pointOutOfGrid2));
            Assert.AreEqual(FieldValues.WATER, grid.GetFieldValue(pointOutOfGrid3));
            Assert.AreEqual(FieldValues.WATER, grid.GetFieldValue(pointOutOfGrid4));

            Assert.AreEqual(FieldValues.UNDETERMINED, grid.GetFieldValue(pointInGrid0));
            Assert.AreEqual(FieldValues.UNDETERMINED, grid.GetFieldValue(pointInGrid1));
        }

        [TestMethod]
        public void TestFieldCount()
        {
            int numRows = 4;
            int numColumns = 3;
            var grid = new Grid(numRows, numColumns);

            Assert.IsTrue(grid.GetNumEmptyFieldsColumn.SequenceEqual(new int[numColumns].InitValues(numRows)));
            Assert.IsTrue(grid.GetNumShipFieldsColumn.SequenceEqual(new int[numColumns].InitValues(0)));
            Assert.IsTrue(grid.GetNumEmptyFieldsRow.SequenceEqual(new int[numRows].InitValues(numColumns)));
            Assert.IsTrue(grid.GetNumShipFieldsRow.SequenceEqual(new int[numRows].InitValues(0)));

            //Assert.AreEqual<IReadOnlyList<int>>(new int[numRows],
            //    grid.GetNumShipFieldsRow);

            // SEW
            // EES
            // EEE
            // SEE
            grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_SINGLE);
            grid.SetFieldValue(new GridPoint(3, 2), FieldValues.WATER);
            grid.SetFieldValue(new GridPoint(3, 0), FieldValues.SHIP_MIDDLE);
            grid.SetFieldValue(new GridPoint(2, 2), FieldValues.SHIP_UNDETERMINED);

            Assert.IsTrue(grid.GetNumEmptyFieldsColumn.SequenceEqual(new int[] { 2, 4, 2 }));
            Assert.IsTrue(grid.GetNumShipFieldsColumn.SequenceEqual(new int[] { 2, 0, 1 }));
            Assert.IsTrue(grid.GetNumEmptyFieldsRow.SequenceEqual(new int[] { 2, 3, 2, 1 }));
            Assert.IsTrue(grid.GetNumShipFieldsRow.SequenceEqual(new int[] { 1, 0, 1, 1 }));
        }

        [TestMethod]
        public void TestShipCountOne()
        {
            int numRows = 4;
            int numColumns = 3;

            var grid = new Grid(numRows, numColumns);

            // SWE
            // EES
            // EEE
            // SEE
            grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_SINGLE);
            grid.SetFieldValue(new GridPoint(3, 0), FieldValues.SHIP_CONT_DOWN);
            grid.SetFieldValue(new GridPoint(3, 1), FieldValues.WATER);
            grid.SetFieldValue(new GridPoint(2, 2), FieldValues.SHIP_SINGLE);

            Assert.IsTrue(new int[] { 0, 2, 0, 0, 0 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestShipCountTwo()
        {
            int numRows = 4;
            int numColumns = 3;

            var grid = new Grid(numRows, numColumns);

            // SWE
            // SEE
            // EES
            // EES
            grid.SetFieldValue(new GridPoint(2, 0), FieldValues.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(3, 0), FieldValues.SHIP_CONT_DOWN);

            grid.SetFieldValue(new GridPoint(3, 1), FieldValues.WATER);

            grid.SetFieldValue(new GridPoint(0, 2), FieldValues.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(1, 2), FieldValues.SHIP_CONT_DOWN);

            Assert.IsTrue(new int[] { 0, 0, 2, 0, 0 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestShipCountThree()
        {
            int numRows = 4;
            int numColumns = 3;

            var grid = new Grid(numRows, numColumns);

            // EEE
            // EES
            // SES
            // SES
            grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(1, 0), FieldValues.SHIP_UNDETERMINED);
            grid.SetFieldValue(new GridPoint(2, 0), FieldValues.SHIP_CONT_DOWN);

            grid.SetFieldValue(new GridPoint(0, 2), FieldValues.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(1, 2), FieldValues.SHIP_MIDDLE);
            grid.SetFieldValue(new GridPoint(2, 2), FieldValues.SHIP_CONT_DOWN);

            Assert.IsTrue(new int[] { 0, 0, 0, 1, 0 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestShipCountFour()
        {
            int numRows = 4;
            int numColumns = 3;

            var grid = new Grid(numRows, numColumns);

            // EES
            // EES
            // SES
            // SES
            grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(1, 0), FieldValues.SHIP_MIDDLE);

            grid.SetFieldValue(new GridPoint(0, 2), FieldValues.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(1, 2), FieldValues.SHIP_MIDDLE);
            grid.SetFieldValue(new GridPoint(2, 2), FieldValues.SHIP_MIDDLE);
            grid.SetFieldValue(new GridPoint(3, 2), FieldValues.SHIP_CONT_DOWN);

            Assert.IsTrue(new int[] { 0, 0, 0, 0, 1 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestShipCountChaos()
        {
            int numRows = 4;
            int numColumns = 3;

            var grid = new Grid(numRows, numColumns);

            // SES
            // EES
            // SES
            // SES
            grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(1, 0), FieldValues.SHIP_MIDDLE);
            grid.SetFieldValue(new GridPoint(1, 0), FieldValues.SHIP_CONT_DOWN);

            grid.SetFieldValue(new GridPoint(3, 0), FieldValues.SHIP_SINGLE);
            grid.SetFieldValue(new GridPoint(3, 0), FieldValues.SHIP_CONT_LEFT);
            grid.SetFieldValue(new GridPoint(3, 0), FieldValues.WATER);
            grid.SetFieldValue(new GridPoint(3, 0), FieldValues.SHIP_SINGLE);

            grid.SetFieldValue(new GridPoint(0, 2), FieldValues.SHIP_CONT_UP);
            grid.SetFieldValue(new GridPoint(1, 2), FieldValues.SHIP_MIDDLE);
            grid.SetFieldValue(new GridPoint(2, 2), FieldValues.SHIP_MIDDLE);
            grid.SetFieldValue(new GridPoint(3, 2), FieldValues.SHIP_CONT_DOWN);
            grid.SetFieldValue(new GridPoint(1, 2), FieldValues.UNDETERMINED);
            grid.SetFieldValue(new GridPoint(1, 2), FieldValues.SHIP_MIDDLE);

            Assert.IsTrue(new int[] { 0, 1, 1, 0, 1 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestClone()
        {
            int numRows = 1;
            int numColumns = 2;
            var grid = new Grid(numRows, numColumns);

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);
            FieldValues valueP0 = FieldValues.SHIP_SINGLE;
            FieldValues valueP1 = FieldValues.WATER;

            grid.SetFieldValue(p0, valueP0);
            grid.SetFieldValue(p1, valueP1);

            Grid clonedGrid = (Grid)grid.Clone();

            Assert.AreEqual(numRows, clonedGrid.NumRows);
            Assert.AreEqual(numColumns, clonedGrid.NumColumns);

            Assert.AreEqual(valueP0, clonedGrid.GetFieldValue(p0));
            Assert.AreEqual(valueP1, clonedGrid.GetFieldValue(p1));

            Assert.IsTrue(grid.GetNumEmptyFieldsColumn.SequenceEqual(clonedGrid.GetNumEmptyFieldsColumn));
            Assert.IsTrue(grid.GetNumEmptyFieldsRow.SequenceEqual(clonedGrid.GetNumEmptyFieldsRow));
            Assert.IsTrue(grid.GetNumShipFieldsColumn.SequenceEqual(clonedGrid.GetNumShipFieldsColumn));
            Assert.IsTrue(grid.GetNumShipFieldsRow.SequenceEqual(clonedGrid.GetNumShipFieldsRow));

            Assert.IsTrue(grid.GetNumShips.SequenceEqual(clonedGrid.GetNumShips));

            grid.SetFieldValue(p0, FieldValues.UNDETERMINED);

            Assert.AreEqual(valueP0, clonedGrid.GetFieldValue(p0));

            Assert.IsFalse(grid.GetNumEmptyFieldsColumn.SequenceEqual(clonedGrid.GetNumEmptyFieldsColumn));
            Assert.IsFalse(grid.GetNumEmptyFieldsRow.SequenceEqual(clonedGrid.GetNumEmptyFieldsRow));
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

            grid.SetFieldValue(new GridPoint(0, 0), FieldValues.WATER);
            grid.SetFieldValue(new GridPoint(0, 1), FieldValues.SHIP_CONT_DOWN);
            grid.SetFieldValue(new GridPoint(0, 2), FieldValues.SHIP_SINGLE);
            grid.SetFieldValue(new GridPoint(1, 0), FieldValues.SHIP_UNDETERMINED);
            grid.SetFieldValue(new GridPoint(1, 1), FieldValues.UNDETERMINED);
            grid.SetFieldValue(new GridPoint(1, 2), FieldValues.SHIP_MIDDLE);

            Assert.IsFalse(grid.IsFullyDetermined);

            grid.SetFieldValue(new GridPoint(1, 1), FieldValues.SHIP_CONT_RIGHT);

            Assert.IsFalse(grid.IsFullyDetermined);

            grid.SetFieldValue(new GridPoint(1, 0), FieldValues.SHIP_CONT_LEFT);

            Assert.IsTrue(grid.IsFullyDetermined);

            grid.SetFieldValue(new GridPoint(1, 2), FieldValues.UNDETERMINED);

            Assert.IsFalse(grid.IsFullyDetermined);

            grid.SetFieldValue(new GridPoint(1, 2), FieldValues.WATER);

            Assert.IsTrue(grid.IsFullyDetermined);
        }

        [TestMethod]
        public void TestIsValid()
        {
            int numRows = 2;
            int numColumns = 3;
            var grid = new Grid(numRows, numColumns);

            Assert.IsTrue(grid.IsValid);

            grid.SetFieldValue(new GridPoint(0, 0), FieldValues.WATER);
            Assert.IsTrue(grid.IsValid);
            grid.SetFieldValue(new GridPoint(0, 1), FieldValues.SHIP_CONT_DOWN);
            Assert.IsFalse(grid.IsValid);
            grid.SetFieldValue(new GridPoint(0, 2), FieldValues.SHIP_SINGLE);
            Assert.IsFalse(grid.IsValid);
            grid.SetFieldValue(new GridPoint(1, 0), FieldValues.SHIP_UNDETERMINED);
            Assert.IsFalse(grid.IsValid);
            grid.SetFieldValue(new GridPoint(1, 1), FieldValues.UNDETERMINED);
            Assert.IsFalse(grid.IsValid);
            grid.SetFieldValue(new GridPoint(1, 2), FieldValues.SHIP_MIDDLE);
            Assert.IsFalse(grid.IsValid);

            // S? ?? SM
            // WW /\  o

            grid.SetFieldValue(new GridPoint(0, 1), FieldValues.WATER);
            Assert.IsFalse(grid.IsValid);
            grid.SetFieldValue(new GridPoint(0, 2), FieldValues.SHIP_CONT_UP);
            Assert.IsTrue(grid.IsValid);

            // S? ?? SM
            // WW WW \/

            grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_SINGLE);
            Assert.IsFalse(grid.IsValid);
            grid.SetFieldValue(new GridPoint(1, 0), FieldValues.WATER);
            Assert.IsTrue(grid.IsValid);

            // WW ?? SM
            //  o WW \/

            grid.SetFieldValue(new GridPoint(1, 2), FieldValues.UNDETERMINED);
            Assert.IsTrue(grid.IsValid);
            grid.SetFieldValue(new GridPoint(0, 1), FieldValues.UNDETERMINED);
            Assert.IsTrue(grid.IsValid);
            grid.SetFieldValue(new GridPoint(1, 1), FieldValues.SHIP_CONT_DOWN);
            Assert.IsFalse(grid.IsValid);

            // WW /\ ??
            //  o ?? \/
        }
    }
}
