using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimaru;
using System.Linq;

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

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p1, FieldValues.UNDETERMINED);

            Assert.AreEqual(FieldValues.UNDETERMINED, rollbackGrid.GetFieldValue(p1));

            rollbackGrid.Rollback();

            Assert.AreEqual(FieldValues.WATER, rollbackGrid.GetFieldValue(p1));

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

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p1, FieldValues.UNDETERMINED);

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p0, FieldValues.SHIP_CONT_DOWN);

            Assert.AreEqual(FieldValues.SHIP_CONT_DOWN, rollbackGrid.GetFieldValue(p0));
            Assert.AreEqual(FieldValues.UNDETERMINED, rollbackGrid.GetFieldValue(p1));

            rollbackGrid.Rollback();

            Assert.AreEqual(FieldValues.SHIP_SINGLE, rollbackGrid.GetFieldValue(p0));
            Assert.AreEqual(FieldValues.UNDETERMINED, rollbackGrid.GetFieldValue(p1));

            rollbackGrid.Rollback();

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

            rollbackGrid.RollbackToInitial();

            AssertAreEqualGrids(rollbackGrid, initialGrid);

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p1, FieldValues.UNDETERMINED);

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p0, FieldValues.SHIP_CONT_DOWN);

            rollbackGrid.RollbackToInitial();

            AssertAreEqualGrids(rollbackGrid, initialGrid);
        }

        [TestMethod]
        public void TestRemoveIntermediate()
        {
            int numRows = 1;
            int numColumns = 2;
            var initialGrid = new Grid(numRows, numColumns);

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            initialGrid.SetFieldValue(p0, FieldValues.SHIP_SINGLE);
            initialGrid.SetFieldValue(p1, FieldValues.WATER);

            var rollbackGrid = new RollbackGrid(initialGrid);

            rollbackGrid.RemoveIntermediate();

            AssertAreEqualGrids(rollbackGrid, initialGrid);

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p1, FieldValues.UNDETERMINED);

            rollbackGrid.RemoveIntermediate();

            Assert.AreEqual(FieldValues.UNDETERMINED, rollbackGrid.GetFieldValue(p1));

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p0, FieldValues.SHIP_CONT_DOWN);

            rollbackGrid.RemoveIntermediate();

            Assert.AreEqual(FieldValues.SHIP_CONT_DOWN, rollbackGrid.GetFieldValue(p0));

            rollbackGrid.Rollback();

            AssertAreEqualGrids(rollbackGrid, initialGrid);
        }
    }
}
