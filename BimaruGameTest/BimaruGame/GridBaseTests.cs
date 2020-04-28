using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Utility;

namespace BimaruGame
{
    [TestClass]
    public class GridBaseTests
    {
        [TestMethod]
        public void TestPositiveNumColumns()
        {
            int numRows = 3;
            int numColumns = 0;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GridBase<int>(numRows, numColumns));
        }

        [TestMethod]
        public void TestPositiveNumRows()
        {
            int numRows = 0;
            int numColumns = 3;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GridBase<int>(numRows, numColumns));
        }

        [TestMethod]
        public void TestInvalidRowIndex()
        {
            int numRows = 3;
            int numColumns = 4;
            var grid = new GridBase<int>(numRows, numColumns);

            var p = new GridPoint(-1, 0);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.GetFieldValue(p));
            Assert.ThrowsException<InvalidFieldChange>(() => grid.SetFieldValue(p, 2));

            p = new GridPoint(numRows, 0);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.GetFieldValue(p));
            Assert.ThrowsException<InvalidFieldChange>(() => grid.SetFieldValue(p, 2));
        }

        [TestMethod]
        public void TestInvalidColumnIndex()
        {
            int numRows = 3;
            int numColumns = 4;
            var grid = new GridBase<int>(numRows, numColumns);

            var p = new GridPoint(0, -1);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.GetFieldValue(p));
            Assert.ThrowsException<InvalidFieldChange>(() => grid.SetFieldValue(p, 2));

            p = new GridPoint(0, numColumns);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.GetFieldValue(p));
            Assert.ThrowsException<InvalidFieldChange>(() => grid.SetFieldValue(p, 2));
        }

        [TestMethod]
        public void TestDefaultValue()
        {
            int numRows = 2;
            int numColumns = 3;
            int defaultValue = 17;

            var grid = new GridBase<int>(numRows, numColumns, defaultValue);

            foreach (GridPoint p in grid.AllPoints())
            {
                Assert.AreEqual(defaultValue, grid.GetFieldValue(p));
            }
        }

        [TestMethod]
        public void TestSetField()
        {
            int numRows = 1;
            int numColumns = 1;
            var grid = new GridBase<int>(numRows, numColumns);

            var p = new GridPoint(0, 0);
            grid.SetFieldValue(p, 15);
            Assert.AreEqual(15, grid.GetFieldValue(p));

            grid.SetFieldValue(p, -7);
            Assert.AreEqual(-7, grid.GetFieldValue(p));
        }

        [TestMethod]
        public void TestFieldValueChanged()
        {
            int numRows = 1;
            int numColumns = 2;
            var grid = new GridBase<int>(numRows, numColumns);

            var eventArgs = new List<FieldValueChangedEventArgs<int>>();

            grid.FieldValueChanged += delegate (object sender, FieldValueChangedEventArgs<int> args)
            {
                eventArgs.Add(args);
            };

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);
            grid.SetFieldValue(p0, 15);
            grid.SetFieldValue(p1, -7);
            grid.SetFieldValue(p1, -7); // Should not trigger the event as no change
            grid.SetFieldValue(p0, 16);

            Assert.AreEqual(3, eventArgs.Count);

            Assert.AreEqual(0, eventArgs[0].OriginalValue);
            Assert.AreEqual(p0, eventArgs[0].Point);

            Assert.AreEqual(0, eventArgs[1].OriginalValue);
            Assert.AreEqual(p1, eventArgs[1].Point);

            Assert.AreEqual(15, eventArgs[2].OriginalValue);
            Assert.AreEqual(p0, eventArgs[2].Point);
        }

        [TestMethod]
        public void TestClone()
        {
            int numRows = 1;
            int numColumns = 2;
            var grid = new GridBase<int>(numRows, numColumns);

            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);
            int valueP0 = 5;
            int valueP1 = 17;

            grid.SetFieldValue(p0, valueP0);
            grid.SetFieldValue(p1, valueP1);

            GridBase<int> clonedGrid = (GridBase<int>)grid.Clone();

            grid.SetFieldValue(p0, -1);

            Assert.AreEqual(numRows, clonedGrid.NumRows);
            Assert.AreEqual(numColumns, clonedGrid.NumColumns);

            Assert.AreEqual(valueP0, clonedGrid.GetFieldValue(p0));
            Assert.AreEqual(valueP1, clonedGrid.GetFieldValue(p1));
        }

        private void CheckPointsOfRow<T>(GridBase<T> grid, int rowIndex)
        {
            int columnIndex = 0;
            foreach (GridPoint p in grid.PointsOfRow(rowIndex))
            {
                Assert.AreEqual(new GridPoint(rowIndex, columnIndex), p);
                columnIndex++;
            }
            Assert.AreEqual(grid.NumColumns, columnIndex);
        }

        [TestMethod]
        public void TestPointsOfRow()
        {
            int rowIndex = 2;
            int numColumns = 1;
            var grid = new GridBase<int>(rowIndex + 1, numColumns);

            CheckPointsOfRow(grid, rowIndex);

            numColumns = 10;
            grid = new GridBase<int>(rowIndex + 1, numColumns);
            CheckPointsOfRow(grid, rowIndex);
        }

        private void CheckPointsOfColumn<T>(GridBase<T> grid, int columnIndex)
        {
            int rowIndex = 0;
            foreach (GridPoint p in grid.PointsOfColumn(columnIndex))
            {
                Assert.AreEqual(new GridPoint(rowIndex, columnIndex), p);
                rowIndex++;
            }
            Assert.AreEqual(grid.NumRows, rowIndex);
        }

        [TestMethod]
        public void TestPointsOfColumn()
        {
            int numRows = 1;
            int columnIndex = 2;
            var grid = new GridBase<int>(numRows, columnIndex + 1);

            CheckPointsOfColumn(grid, columnIndex);

            numRows = 10;
            grid = new GridBase<int>(numRows, columnIndex + 1);
            CheckPointsOfColumn(grid, columnIndex);
        }

        [TestMethod]
        public void TestAllPoints()
        {
            int numRows = 2;
            int numColumns = 3;
            var grid = new GridBase<int>(numRows, numColumns);

            var pointsInGrid = new HashSet<GridPoint>()
            {   new GridPoint(0, 0),
                new GridPoint(0, 1),
                new GridPoint(0, 2),
                new GridPoint(1, 0),
                new GridPoint(1, 1),
                new GridPoint(1, 2),
            };

            foreach (GridPoint p in grid.AllPoints())
            {
                Assert.IsTrue(pointsInGrid.Remove(p));
            }

            Assert.AreEqual(0, pointsInGrid.Count);
        }
    }
}
