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

            // Values of points outside the grid are WATER
            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(0, -1)]);
            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(-1, 0)]);
            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(numRows, 0)]);
            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(0, numColumns)]);
            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(100, 2)]);

            // Default values inside the grid are UNDETERMINED
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(3, 2)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(0, 0)]);
        }

        [TestMethod]
        public void TestSetFieldValue()
        {
            var grid = new Grid(4, 3);

            // WATER is okay, as off-grid values are WATER
            var pointOutOfGrid0 = new GridPoint(0, -1);
            grid[pointOutOfGrid0] = BimaruValue.WATER;
            Assert.ThrowsException<InvalidFieldChange>(() => grid[pointOutOfGrid0] = BimaruValue.UNDETERMINED);
            Assert.ThrowsException<InvalidFieldChange>(() => grid[pointOutOfGrid0] = BimaruValue.SHIP_SINGLE);

            // Inside the grid, everything is okay
            var pointInGrid0 = new GridPoint(0, 0);
            grid[pointInGrid0] = BimaruValue.UNDETERMINED;
            grid[pointInGrid0] = BimaruValue.SHIP_CONT_LEFT;
            grid[pointInGrid0] = BimaruValue.UNDETERMINED;
        }

        [TestMethod]
        public void TestFillUndeterminedFieldsColumn()
        {
            var grid = new Grid(2, 2);

            grid.FillUndeterminedFieldsColumn(1, BimaruValueConstraint.NO);

            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 1)]);


            grid.FillUndeterminedFieldsColumn(1, BimaruValueConstraint.WATER);

            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(1, 1)]);


            grid[new GridPoint(0, 1)] = BimaruValue.UNDETERMINED;
            grid.FillUndeterminedFieldsColumn(1, BimaruValueConstraint.SHIP);

            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(1, 1)]);


            grid.FillUndeterminedFieldsColumn(1, BimaruValueConstraint.SHIP);

            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(1, 1)]);
        }

        [TestMethod]
        public void TestFillUndeterminedFieldsRow()
        {
            var grid = new Grid(2, 2);

            grid.FillUndeterminedFieldsRow(0, BimaruValueConstraint.NO);

            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 1)]);


            grid.FillUndeterminedFieldsRow(0, BimaruValueConstraint.WATER);

            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 1)]);


            grid[new GridPoint(0, 1)] = BimaruValue.UNDETERMINED;
            grid.FillUndeterminedFieldsRow(0, BimaruValueConstraint.SHIP);

            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 1)]);


            grid.FillUndeterminedFieldsRow(0, BimaruValueConstraint.SHIP);

            Assert.AreEqual(BimaruValue.WATER, grid[new GridPoint(0, 0)]);
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, grid[new GridPoint(0, 1)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 0)]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, grid[new GridPoint(1, 1)]);
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
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            grid[new GridPoint(3, 2)] = BimaruValue.WATER;
            grid[new GridPoint(3, 0)] = BimaruValue.SHIP_MIDDLE;
            grid[new GridPoint(2, 2)] = BimaruValue.SHIP_SINGLE;
            grid[new GridPoint(2, 2)] = BimaruValue.WATER;
            grid[new GridPoint(2, 2)] = BimaruValue.SHIP_UNDETERMINED;

            Assert.IsTrue(grid.GetNumUndeterminedFieldsColumn.SequenceEqual(new int[] { 2, 4, 2 }));
            Assert.IsTrue(grid.GetNumShipFieldsColumn.SequenceEqual(new int[] { 2, 0, 1 }));
            Assert.IsTrue(grid.GetNumUndeterminedFieldsRow.SequenceEqual(new int[] { 2, 3, 2, 1 }));
            Assert.IsTrue(grid.GetNumShipFieldsRow.SequenceEqual(new int[] { 1, 0, 1, 1 }));
        }

        [TestMethod]
        public void TestIsFullyDetermined()
        {
            var grid = new Grid(2, 3);

            Assert.IsFalse(grid.IsFullyDetermined);

            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            grid[new GridPoint(0, 1)] = BimaruValue.SHIP_CONT_DOWN;
            grid[new GridPoint(0, 2)] = BimaruValue.SHIP_SINGLE;
            grid[new GridPoint(1, 0)] = BimaruValue.SHIP_UNDETERMINED;
            grid[new GridPoint(1, 1)] = BimaruValue.UNDETERMINED;
            grid[new GridPoint(1, 2)] = BimaruValue.SHIP_MIDDLE;

            Assert.IsFalse(grid.IsFullyDetermined);

            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_CONT_RIGHT;

            Assert.IsFalse(grid.IsFullyDetermined);

            grid[new GridPoint(1, 0)] = BimaruValue.SHIP_CONT_LEFT;

            Assert.IsTrue(grid.IsFullyDetermined);

            grid[new GridPoint(1, 2)] = BimaruValue.UNDETERMINED;

            Assert.IsFalse(grid.IsFullyDetermined);

            grid[new GridPoint(1, 2)] = BimaruValue.WATER;

            Assert.IsTrue(grid.IsFullyDetermined);
        }

        [TestMethod]
        public void TestIsValid()
        {
            // Test diagonal boundary
            var grid = new Grid(2, 2);
            Assert.IsTrue(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_UNDETERMINED;
            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_UNDETERMINED;
            Assert.IsFalse(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            Assert.IsTrue(grid.IsValid);

            // Test vertical boundary
            grid = new Grid(2, 2);
            Assert.IsTrue(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            grid[new GridPoint(1, 0)] = BimaruValue.SHIP_UNDETERMINED;
            Assert.IsFalse(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            Assert.IsTrue(grid.IsValid);

            // Test horizontal boundary
            grid = new Grid(2, 2);
            Assert.IsTrue(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            grid[new GridPoint(0, 1)] = BimaruValue.SHIP_UNDETERMINED;
            Assert.IsFalse(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            Assert.IsTrue(grid.IsValid);

            // Test grid boundary
            grid = new Grid(2, 2);
            Assert.IsTrue(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_DOWN;
            grid[new GridPoint(0, 1)] = BimaruValue.WATER;
            Assert.IsFalse(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            Assert.IsTrue(grid.IsValid);
        }

        [TestMethod]
        public void TestShipCountOne()
        {
            var grid = new Grid(4, 3);

            // SWU
            // UUS
            // UUU
            // SUU
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            grid[new GridPoint(3, 0)] = BimaruValue.SHIP_CONT_DOWN;
            grid[new GridPoint(3, 1)] = BimaruValue.WATER;
            grid[new GridPoint(2, 2)] = BimaruValue.SHIP_SINGLE;

            Assert.IsTrue(new int[] { 0, 2, 0, 0, 0 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestShipCountTwo()
        {
            var grid = new Grid(4, 3);

            // SWU
            // SUU
            // USS
            // UUU
            grid[new GridPoint(2, 0)] = BimaruValue.SHIP_CONT_UP;
            grid[new GridPoint(3, 0)] = BimaruValue.SHIP_CONT_DOWN;
            grid[new GridPoint(3, 1)] = BimaruValue.WATER;
            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_CONT_RIGHT;
            grid[new GridPoint(1, 2)] = BimaruValue.SHIP_CONT_LEFT;

            Assert.IsTrue(new int[] { 0, 0, 2, 0, 0 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestShipCountThree()
        {
            var grid = new Grid(4, 3);

            // SSS
            // UUS
            // SUS
            // SUS
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_UP;
            grid[new GridPoint(1, 0)] = BimaruValue.SHIP_UNDETERMINED;
            grid[new GridPoint(3, 0)] = BimaruValue.SHIP_CONT_RIGHT;
            grid[new GridPoint(3, 1)] = BimaruValue.SHIP_MIDDLE;
            grid[new GridPoint(3, 2)] = BimaruValue.SHIP_CONT_LEFT;
            grid[new GridPoint(0, 2)] = BimaruValue.SHIP_CONT_UP;
            grid[new GridPoint(1, 2)] = BimaruValue.SHIP_MIDDLE;
            grid[new GridPoint(2, 2)] = BimaruValue.SHIP_CONT_DOWN;

            Assert.IsTrue(new int[] { 0, 0, 0, 2, 0 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestShipCountFour()
        {
            var grid = new Grid(4, 3);

            // SUS
            // UUS
            // SUS
            // SUS
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_UP;
            grid[new GridPoint(1, 0)] = BimaruValue.SHIP_MIDDLE;
            grid[new GridPoint(3, 0)] = BimaruValue.SHIP_CONT_DOWN;

            grid[new GridPoint(0, 2)] = BimaruValue.SHIP_CONT_UP;
            grid[new GridPoint(1, 2)] = BimaruValue.SHIP_MIDDLE;
            grid[new GridPoint(2, 2)] = BimaruValue.SHIP_MIDDLE;
            grid[new GridPoint(3, 2)] = BimaruValue.SHIP_CONT_DOWN;

            Assert.IsTrue(new int[] { 0, 0, 0, 0, 1 }.SequenceEqual(grid.GetNumShips));
        }

        [TestMethod]
        public void TestShipCountChaos()
        {
            var grid = new Grid(3, 3);

            // USU
            // SSS
            // USU
            grid[new GridPoint(0, 1)] = BimaruValue.SHIP_CONT_UP;
            grid[new GridPoint(1, 0)] = BimaruValue.SHIP_CONT_RIGHT;
            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_MIDDLE;
            grid[new GridPoint(1, 2)] = BimaruValue.SHIP_CONT_LEFT;
            grid[new GridPoint(2, 1)] = BimaruValue.SHIP_CONT_DOWN;

            Assert.IsTrue(new int[] { 0, 0, 0, 2 }.SequenceEqual(grid.GetNumShips));

            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_SINGLE;

            Assert.IsTrue(new int[] { 0, 1, 0, 0 }.SequenceEqual(grid.GetNumShips));

            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_CONT_RIGHT;

            Assert.IsTrue(new int[] { 0, 0, 1, 0 }.SequenceEqual(grid.GetNumShips));

            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_MIDDLE;

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
            BimaruValue valueP1 = BimaruValue.UNDETERMINED;

            grid[p0] = valueP0;
            grid[p1] = valueP1;

            Grid clonedGrid = (Grid)grid.Clone();

            Assert.AreEqual(numRows, clonedGrid.NumRows);
            Assert.AreEqual(numColumns, clonedGrid.NumColumns);

            Assert.AreEqual(valueP0, clonedGrid[p0]);
            Assert.AreEqual(valueP1, clonedGrid[p1]);

            Assert.IsTrue(grid.GetNumUndeterminedFieldsColumn.SequenceEqual(clonedGrid.GetNumUndeterminedFieldsColumn));
            Assert.IsTrue(grid.GetNumUndeterminedFieldsRow.SequenceEqual(clonedGrid.GetNumUndeterminedFieldsRow));
            Assert.IsTrue(grid.GetNumShipFieldsColumn.SequenceEqual(clonedGrid.GetNumShipFieldsColumn));
            Assert.IsTrue(grid.GetNumShipFieldsRow.SequenceEqual(clonedGrid.GetNumShipFieldsRow));

            Assert.IsTrue(grid.GetNumShips.SequenceEqual(clonedGrid.GetNumShips));

            Assert.AreEqual(grid.IsValid, clonedGrid.IsValid);
            Assert.AreEqual(grid.IsFullyDetermined, clonedGrid.IsFullyDetermined);

            grid[p1] = BimaruValue.SHIP_SINGLE;

            Assert.AreEqual(valueP1, clonedGrid[p1]);

            Assert.IsFalse(grid.GetNumUndeterminedFieldsColumn.SequenceEqual(clonedGrid.GetNumUndeterminedFieldsColumn));
            Assert.IsFalse(grid.GetNumUndeterminedFieldsRow.SequenceEqual(clonedGrid.GetNumUndeterminedFieldsRow));
            Assert.IsFalse(grid.GetNumShipFieldsColumn.SequenceEqual(clonedGrid.GetNumShipFieldsColumn));
            Assert.IsFalse(grid.GetNumShipFieldsRow.SequenceEqual(clonedGrid.GetNumShipFieldsRow));

            Assert.IsFalse(grid.GetNumShips.SequenceEqual(clonedGrid.GetNumShips));

            Assert.AreNotEqual(grid.IsValid, clonedGrid.IsValid);
            Assert.AreNotEqual(grid.IsFullyDetermined, clonedGrid.IsFullyDetermined);
        }
    }
}
