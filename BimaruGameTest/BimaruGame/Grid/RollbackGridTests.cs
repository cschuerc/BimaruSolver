using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Utility;

namespace BimaruGame
{
    [TestClass]
    public class RollbackGridTests
    {
        private void AssertAreEqualGrids(IGrid grid, IGrid otherGrid)
        {
            Assert.AreEqual(grid.NumRows, otherGrid.NumRows);
            Assert.AreEqual(grid.NumColumns, otherGrid.NumColumns);

            foreach (GridPoint p in grid.AllPoints())
            {
                Assert.AreEqual(grid[p], otherGrid[p]);
            }
        }

        [TestMethod]
        public void TestStackOperations()
        {
            int numRows = 1;
            int numColumns = 2;
            var initialGrid = new Grid(numRows, numColumns);

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            initialGrid[p0] = BimaruValue.SHIP_SINGLE;
            initialGrid[p1] = BimaruValue.WATER;

            var rollbackGrid = new RollbackGrid(numRows, numColumns);
            rollbackGrid.CopyFrom(initialGrid);

            rollbackGrid.SetSavePoint();

            rollbackGrid[p1] = BimaruValue.UNDETERMINED;

            rollbackGrid.SetSavePoint();

            rollbackGrid[p0] = BimaruValue.SHIP_CONT_DOWN;

            Assert.AreEqual(BimaruValue.SHIP_CONT_DOWN, rollbackGrid[p0]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, rollbackGrid[p1]);

            rollbackGrid.Rollback();

            Assert.AreEqual(BimaruValue.SHIP_SINGLE, rollbackGrid[p0]);
            Assert.AreEqual(BimaruValue.UNDETERMINED, rollbackGrid[p1]);

            rollbackGrid.Rollback();

            AssertAreEqualGrids(rollbackGrid, initialGrid);

            // Check no more grids on the stack
            Assert.ThrowsException<InvalidOperationException>(() => rollbackGrid.Rollback());
        }

        [TestMethod]
        public void TestClipboard()
        {
            int numRows = 1;
            int numColumns = 2;
            var initialGrid = new Grid(numRows, numColumns);

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            initialGrid[p0] = BimaruValue.SHIP_SINGLE;
            initialGrid[p1] = BimaruValue.WATER;

            var rollbackGrid = new RollbackGrid(numRows, numColumns);
            rollbackGrid.CopyFrom(initialGrid);

            rollbackGrid.SetSavePoint();

            Assert.ThrowsException<InvalidOperationException>(() => rollbackGrid.RestoreFromClipboard());

            rollbackGrid.CloneToClipboard();

            rollbackGrid[p0] = BimaruValue.SHIP_CONT_UP;
            rollbackGrid[p1] = BimaruValue.SHIP_CONT_DOWN;

            rollbackGrid.CloneToClipboard();

            rollbackGrid.Rollback();

            AssertAreEqualGrids(rollbackGrid, initialGrid);

            rollbackGrid.RestoreFromClipboard();

            Assert.AreEqual(BimaruValue.SHIP_CONT_UP, rollbackGrid[p0]);
            Assert.AreEqual(BimaruValue.SHIP_CONT_DOWN, rollbackGrid[p1]);

            rollbackGrid.Rollback();

            AssertAreEqualGrids(rollbackGrid, initialGrid);

            // Check no more grids on the stack
            Assert.ThrowsException<InvalidOperationException>(() => rollbackGrid.Rollback());
        }

        [TestMethod]
        public void TestRestoreEvent()
        {
            var rollbackGrid = new RollbackGrid(1, 2);

            int numRestoresHappened = 0;
            rollbackGrid.RestoreHappened += delegate ()
            {
                numRestoresHappened++;
            };

            rollbackGrid.SetSavePoint();

            Assert.AreEqual(0, numRestoresHappened);

            rollbackGrid.Rollback();

            Assert.AreEqual(1, numRestoresHappened);

            rollbackGrid.CloneToClipboard();

            Assert.AreEqual(1, numRestoresHappened);

            rollbackGrid.RestoreFromClipboard();

            Assert.AreEqual(2, numRestoresHappened);
        }

        [TestMethod]
        public void TestClone()
        {
            var rollbackGrid = new RollbackGrid(1, 2);
            Assert.ThrowsException<InvalidOperationException>(() => rollbackGrid.Clone());
        }
    }
}
