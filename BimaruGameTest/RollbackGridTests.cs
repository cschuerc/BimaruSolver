using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Utility;

namespace BimaruTest
{
    [TestClass]
    public class RollbackGridTests
    {
        private void AssertAreEqualGrids(IGrid grid, IGrid otherGrid)
        {
            Assert.AreEqual(grid.NumRows, otherGrid.NumRows);
            Assert.AreEqual(grid.NumColumns, otherGrid.NumColumns);

            for (int rowIndex = 0; rowIndex < grid.NumRows; rowIndex++)
            {
                for (int columnindex = 0; columnindex < grid.NumColumns; columnindex++)
                {
                    GridPoint p = new GridPoint(rowIndex, columnindex);
                    Assert.AreEqual(grid.GetFieldValue(p), otherGrid.GetFieldValue(p));
                }
            }

            Assert.IsTrue(grid.GetNumEmptyFieldsColumn.SequenceEqual(otherGrid.GetNumEmptyFieldsColumn));
            Assert.IsTrue(grid.GetNumEmptyFieldsRow.SequenceEqual(otherGrid.GetNumEmptyFieldsRow));
            Assert.IsTrue(grid.GetNumShipFieldsColumn.SequenceEqual(otherGrid.GetNumShipFieldsColumn));
            Assert.IsTrue(grid.GetNumShipFieldsRow.SequenceEqual(otherGrid.GetNumShipFieldsRow));

            Assert.IsTrue(grid.GetNumShips.SequenceEqual(otherGrid.GetNumShips));

            Assert.AreEqual(grid.IsFullyDetermined, otherGrid.IsFullyDetermined);
            Assert.AreEqual(grid.IsValid, otherGrid.IsValid);
        }

        [TestMethod]
        public void TestInitialGrid()
        {
            int numRows = 1;
            int numColumns = 2;
            var initialGrid = new Grid(numRows, numColumns);

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            initialGrid.SetFieldValue(p0, FieldValues.SHIP_SINGLE);
            initialGrid.SetFieldValue(p1, FieldValues.WATER);

            var rollbackGrid = new RollbackGrid(initialGrid);

            AssertAreEqualGrids(initialGrid, rollbackGrid);
        }

        [TestMethod]
        public void TestRollback()
        {
            int numRows = 1;
            int numColumns = 2;
            var initialGrid = new Grid(numRows, numColumns);

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            initialGrid.SetFieldValue(p0, FieldValues.SHIP_SINGLE);
            initialGrid.SetFieldValue(p1, FieldValues.WATER);

            var rollbackGrid = new RollbackGrid(initialGrid);

            int numRollbackEvents = 0;
            rollbackGrid.RollbackHappened += delegate ()
            {
                numRollbackEvents++;
            };

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p1, FieldValues.UNDETERMINED);

            Assert.AreEqual(FieldValues.UNDETERMINED, rollbackGrid.GetFieldValue(p1));
            Assert.AreEqual(0, numRollbackEvents);

            rollbackGrid.Rollback();

            Assert.AreEqual(FieldValues.WATER, rollbackGrid.GetFieldValue(p1));
            Assert.AreEqual(1, numRollbackEvents);

            AssertAreEqualGrids(rollbackGrid, initialGrid);
        }

        [TestMethod]
        public void TestRollbackTwoSteps()
        {
            int numRows = 1;
            int numColumns = 2;
            var initialGrid = new Grid(numRows, numColumns);

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            initialGrid.SetFieldValue(p0, FieldValues.SHIP_SINGLE);
            initialGrid.SetFieldValue(p1, FieldValues.WATER);

            var rollbackGrid = new RollbackGrid(initialGrid);

            int numRollbackEvents = 0;
            rollbackGrid.RollbackHappened += delegate ()
            {
                numRollbackEvents++;
            };

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p1, FieldValues.UNDETERMINED);

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p0, FieldValues.SHIP_CONT_DOWN);

            Assert.AreEqual(FieldValues.SHIP_CONT_DOWN, rollbackGrid.GetFieldValue(p0));
            Assert.AreEqual(FieldValues.UNDETERMINED, rollbackGrid.GetFieldValue(p1));
            Assert.AreEqual(0, numRollbackEvents);

            rollbackGrid.Rollback();

            Assert.AreEqual(FieldValues.SHIP_SINGLE, rollbackGrid.GetFieldValue(p0));
            Assert.AreEqual(FieldValues.UNDETERMINED, rollbackGrid.GetFieldValue(p1));
            Assert.AreEqual(1, numRollbackEvents);

            rollbackGrid.Rollback();

            Assert.AreEqual(2, numRollbackEvents);

            AssertAreEqualGrids(rollbackGrid, initialGrid);
        }

        [TestMethod]
        public void TestRollbackToInitial()
        {
            int numRows = 1;
            int numColumns = 2;
            var initialGrid = new Grid(numRows, numColumns);

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            initialGrid.SetFieldValue(p0, FieldValues.SHIP_SINGLE);
            initialGrid.SetFieldValue(p1, FieldValues.WATER);

            var rollbackGrid = new RollbackGrid(initialGrid);

            int numRollbackEvents = 0;
            rollbackGrid.RollbackHappened += delegate ()
            {
                numRollbackEvents++;
            };

            Assert.AreEqual(0, numRollbackEvents);

            rollbackGrid.RollbackToInitial();

            Assert.AreEqual(0, numRollbackEvents);
            AssertAreEqualGrids(rollbackGrid, initialGrid);

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p1, FieldValues.UNDETERMINED);

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p0, FieldValues.SHIP_CONT_DOWN);

            Assert.AreEqual(0, numRollbackEvents);

            rollbackGrid.RollbackToInitial();

            Assert.AreEqual(1, numRollbackEvents);
            AssertAreEqualGrids(rollbackGrid, initialGrid);
        }

        [TestMethod]
        public void TestRemovePrevious()
        {
            int numRows = 1;
            int numColumns = 2;
            var initialGrid = new Grid(numRows, numColumns);

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            initialGrid.SetFieldValue(p0, FieldValues.SHIP_SINGLE);
            initialGrid.SetFieldValue(p1, FieldValues.WATER);

            var rollbackGrid = new RollbackGrid(initialGrid);

            rollbackGrid.RemovePrevious();

            AssertAreEqualGrids(rollbackGrid, initialGrid);

            rollbackGrid.SetSavePoint();
            rollbackGrid.SetFieldValue(p1, FieldValues.UNDETERMINED);

            rollbackGrid.RemovePrevious();

            Assert.AreEqual(FieldValues.UNDETERMINED, rollbackGrid.GetFieldValue(p1));

            rollbackGrid.SetSavePoint();
            rollbackGrid.SetFieldValue(p0, FieldValues.SHIP_CONT_DOWN);
            rollbackGrid.SetSavePoint();
            rollbackGrid.SetFieldValue(p1, FieldValues.SHIP_MIDDLE);

            rollbackGrid.RemovePrevious();

            Assert.AreEqual(FieldValues.SHIP_CONT_DOWN, rollbackGrid.GetFieldValue(p0));
            Assert.AreEqual(FieldValues.SHIP_MIDDLE, rollbackGrid.GetFieldValue(p1));

            rollbackGrid.Rollback();

            Assert.AreEqual(FieldValues.SHIP_SINGLE, rollbackGrid.GetFieldValue(p0));
            Assert.AreEqual(FieldValues.UNDETERMINED, rollbackGrid.GetFieldValue(p1));
        }
    }
}
