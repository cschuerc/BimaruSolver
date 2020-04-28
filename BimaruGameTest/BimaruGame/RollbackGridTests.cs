using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
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
                Assert.AreEqual(grid.GetFieldValue(p), otherGrid.GetFieldValue(p));
            }

            Assert.IsTrue(grid.GetNumUndeterminedFieldsColumn.SequenceEqual(otherGrid.GetNumUndeterminedFieldsColumn));
            Assert.IsTrue(grid.GetNumUndeterminedFieldsRow.SequenceEqual(otherGrid.GetNumUndeterminedFieldsRow));
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

            initialGrid.SetFieldValue(p0, BimaruValue.SHIP_SINGLE);
            initialGrid.SetFieldValue(p1, BimaruValue.WATER);

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

            initialGrid.SetFieldValue(p0, BimaruValue.SHIP_SINGLE);
            initialGrid.SetFieldValue(p1, BimaruValue.WATER);

            var rollbackGrid = new RollbackGrid(initialGrid);

            int numRollbackEvents = 0;
            rollbackGrid.RollbackHappened += delegate ()
            {
                numRollbackEvents++;
            };

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p1, BimaruValue.UNDETERMINED);

            Assert.AreEqual(BimaruValue.UNDETERMINED, rollbackGrid.GetFieldValue(p1));
            Assert.AreEqual(0, numRollbackEvents);

            rollbackGrid.Rollback();

            Assert.AreEqual(BimaruValue.WATER, rollbackGrid.GetFieldValue(p1));
            Assert.AreEqual(1, numRollbackEvents);

            AssertAreEqualGrids(rollbackGrid, initialGrid);
        }

        private void CheckChangedEvent(FieldValueChangedEventArgs<BimaruValue> eExp, FieldValueChangedEventArgs<BimaruValue> eActual)
        {
            Assert.AreEqual(eExp.Point, eActual.Point);
            Assert.AreEqual(eExp.OriginalValue, eActual.OriginalValue);
        }

        [TestMethod]
        public void TestRollbackTwoSteps()
        {
            int numRows = 1;
            int numColumns = 2;
            var initialGrid = new Grid(numRows, numColumns);

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            initialGrid.SetFieldValue(p0, BimaruValue.SHIP_SINGLE);
            initialGrid.SetFieldValue(p1, BimaruValue.WATER);

            var rollbackGrid = new RollbackGrid(initialGrid);

            int numRollbackEvents = 0;
            rollbackGrid.RollbackHappened += delegate ()
            {
                numRollbackEvents++;
            };

            var changedEventArgs = new List<FieldValueChangedEventArgs<BimaruValue>>();
            rollbackGrid.FieldValueChanged += delegate (object sender, FieldValueChangedEventArgs<BimaruValue> e)
            {
                changedEventArgs.Add(e);
            };

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p1, BimaruValue.UNDETERMINED);
            Assert.AreEqual(1, changedEventArgs.Count);
            CheckChangedEvent(new FieldValueChangedEventArgs<BimaruValue>(p1, BimaruValue.WATER), changedEventArgs[0]);

            rollbackGrid.SetSavePoint();

            rollbackGrid.SetFieldValue(p0, BimaruValue.SHIP_CONT_DOWN);
            Assert.AreEqual(2, changedEventArgs.Count);
            CheckChangedEvent(new FieldValueChangedEventArgs<BimaruValue>(p0, BimaruValue.SHIP_SINGLE), changedEventArgs[1]);

            Assert.AreEqual(BimaruValue.SHIP_CONT_DOWN, rollbackGrid.GetFieldValue(p0));
            Assert.AreEqual(BimaruValue.UNDETERMINED, rollbackGrid.GetFieldValue(p1));
            Assert.AreEqual(0, numRollbackEvents);

            rollbackGrid.Rollback();

            Assert.AreEqual(BimaruValue.SHIP_SINGLE, rollbackGrid.GetFieldValue(p0));
            Assert.AreEqual(BimaruValue.UNDETERMINED, rollbackGrid.GetFieldValue(p1));
            Assert.AreEqual(1, numRollbackEvents);

            rollbackGrid.Rollback();

            Assert.AreEqual(2, numRollbackEvents);
            Assert.AreEqual(2, changedEventArgs.Count);

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

            initialGrid.SetFieldValue(p0, BimaruValue.SHIP_SINGLE);
            initialGrid.SetFieldValue(p1, BimaruValue.WATER);

            var rollbackGrid = new RollbackGrid(initialGrid);

            rollbackGrid.RemovePrevious();

            AssertAreEqualGrids(rollbackGrid, initialGrid);

            rollbackGrid.SetSavePoint();
            rollbackGrid.SetFieldValue(p1, BimaruValue.UNDETERMINED);

            rollbackGrid.RemovePrevious();

            Assert.AreEqual(BimaruValue.UNDETERMINED, rollbackGrid.GetFieldValue(p1));

            rollbackGrid.SetSavePoint();
            rollbackGrid.SetFieldValue(p0, BimaruValue.SHIP_CONT_DOWN);
            rollbackGrid.SetSavePoint();
            rollbackGrid.SetFieldValue(p1, BimaruValue.SHIP_MIDDLE);

            rollbackGrid.RemovePrevious();

            Assert.AreEqual(BimaruValue.SHIP_CONT_DOWN, rollbackGrid.GetFieldValue(p0));
            Assert.AreEqual(BimaruValue.SHIP_MIDDLE, rollbackGrid.GetFieldValue(p1));

            rollbackGrid.Rollback();

            Assert.AreEqual(BimaruValue.SHIP_SINGLE, rollbackGrid.GetFieldValue(p0));
            Assert.AreEqual(BimaruValue.UNDETERMINED, rollbackGrid.GetFieldValue(p1));
        }
    }
}
