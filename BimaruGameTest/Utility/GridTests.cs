using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    [TestClass]
    public class GridTests
    {
        [TestMethod]
        public void TestNumberOfRowsRange()
        {
            int numberOfColumns = 1;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Grid<int>(-10, numberOfColumns));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Grid<int>(-1, numberOfColumns));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Grid<int>(0, numberOfColumns));

            new Grid<int>(1, numberOfColumns);
            new Grid<int>(2, numberOfColumns);
            new Grid<int>(10, numberOfColumns);
        }

        [TestMethod]
        public void TestNumberOfColumnsRange()
        {
            int numberOfRows = 1;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Grid<int>(numberOfRows, -10));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Grid<int>(numberOfRows, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Grid<int>(numberOfRows, 0));

            new Grid<int>(numberOfRows, 1);
            new Grid<int>(numberOfRows, 2);
            new Grid<int>(numberOfRows, 10);
        }

        [TestMethod]
        public void TestNumberOfRowsColumns()
        {
            var grid = new Grid<int>(2, 3);

            Assert.AreEqual(2, grid.NumberOfRows);
            Assert.AreEqual(3, grid.NumberOfColumns);
        }

        [TestMethod]
        public void TestDefaultFieldValue()
        {
            int defaultFieldValue = 17;

            var grid = new Grid<int>(2, 3, defaultFieldValue);

            foreach (GridPoint p in grid.AllPoints())
            {
                Assert.AreEqual(defaultFieldValue, grid[p]);
            }
        }

        [TestMethod]
        public void TestRowIndexRangeGet()
        {
            var grid = new Grid<int>(3, 4);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid[new GridPoint(-10, 0)]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid[new GridPoint(-2, 0)]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid[new GridPoint(-1, 0)]);

            int dummy;
            dummy = grid[new GridPoint(0, 0)];
            dummy = grid[new GridPoint(1, 0)];
            dummy = grid[new GridPoint(2, 0)];

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid[new GridPoint(3, 0)]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid[new GridPoint(4, 0)]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid[new GridPoint(10, 0)]);
        }

        [TestMethod]
        public void TestColumnIndexRangeGet()
        {
            var grid = new Grid<int>(4, 3);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid[new GridPoint(0, -10)]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid[new GridPoint(0, -2)]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid[new GridPoint(0, -1)]);

            int dummy;
            dummy = grid[new GridPoint(0, 0)];
            dummy = grid[new GridPoint(0, 1)];
            dummy = grid[new GridPoint(0, 2)];

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid[new GridPoint(0, 3)]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid[new GridPoint(0, 4)]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid[new GridPoint(0, 10)]);
        }

        [TestMethod]
        public void TestRowIndexRangeSet()
        {
            var grid = new Grid<int>(3, 4);

            Assert.ThrowsException<InvalidFieldValueChange>(() => grid[new GridPoint(-10, 0)] = 0);
            Assert.ThrowsException<InvalidFieldValueChange>(() => grid[new GridPoint(-2, 0)] = 0);
            Assert.ThrowsException<InvalidFieldValueChange>(() => grid[new GridPoint(-1, 0)] = 0);

            grid[new GridPoint(0, 0)] = 0;
            grid[new GridPoint(1, 0)] = 0;
            grid[new GridPoint(2, 0)] = 0;

            Assert.ThrowsException<InvalidFieldValueChange>(() => grid[new GridPoint(3, 0)] = 0);
            Assert.ThrowsException<InvalidFieldValueChange>(() => grid[new GridPoint(4, 0)] = 0);
            Assert.ThrowsException<InvalidFieldValueChange>(() => grid[new GridPoint(10, 0)] = 0);
        }

        [TestMethod]
        public void TestColumnIndexRangeSet()
        {
            var grid = new Grid<int>(4, 3);

            Assert.ThrowsException<InvalidFieldValueChange>(() => grid[new GridPoint(0, -10)] = 0);
            Assert.ThrowsException<InvalidFieldValueChange>(() => grid[new GridPoint(0, -2)] = 0);
            Assert.ThrowsException<InvalidFieldValueChange>(() => grid[new GridPoint(0, -1)] = 0);

            grid[new GridPoint(0, 0)] = 0;
            grid[new GridPoint(0, 1)] = 0;
            grid[new GridPoint(0, 2)] = 0;

            Assert.ThrowsException<InvalidFieldValueChange>(() => grid[new GridPoint(0, 3)] = 0);
            Assert.ThrowsException<InvalidFieldValueChange>(() => grid[new GridPoint(0, 4)] = 0);
            Assert.ThrowsException<InvalidFieldValueChange>(() => grid[new GridPoint(0, 10)] = 0);
        }

        [TestMethod]
        public void TestGetSetFieldValue()
        {
            var grid = new Grid<int>(1, 2);
            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            Assert.AreEqual(0, grid[p0]);
            Assert.AreEqual(0, grid[p1]);

            grid[p0] = 15;

            Assert.AreEqual(15, grid[p0]);
            Assert.AreEqual(0, grid[p1]);

            grid[p1] = -7;

            Assert.AreEqual(15, grid[p0]);
            Assert.AreEqual(-7, grid[p1]);
        }

        [TestMethod]
        public void TestFieldValueChanged()
        {
            var grid = new Grid<int>(1, 2);
            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            var actualEventArgs = new List<FieldValueChangedEventArgs<int>>();

            grid.FieldValueChanged += delegate (object sender, FieldValueChangedEventArgs<int> e)
            {
                actualEventArgs.Add(e);
            };

            grid[p0] = 15;
            grid[p1] = -7;
            grid[p1] = -7; // Should not trigger the event as no change
            grid[p0] = 16;

            var expectedEventArgs = new List<FieldValueChangedEventArgs<int>>()
            {
                new FieldValueChangedEventArgs<int>(p0, 0),
                new FieldValueChangedEventArgs<int>(p1, 0),
                new FieldValueChangedEventArgs<int>(p0, 15)
            };

            AssertEqualFieldValueChangedEventArgs(expectedEventArgs, actualEventArgs);
        }

        private void AssertEqualFieldValueChangedEventArgs<T>(List<FieldValueChangedEventArgs<T>> expected, List<FieldValueChangedEventArgs<T>> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            foreach (int index in Enumerable.Range(0, expected.Count))
            {
                Assert.AreEqual(expected[index].Point, expected[index].Point);
                Assert.AreEqual(expected[index].OriginalValue, expected[index].OriginalValue);
            }
        }

        [TestMethod]
        public void TestPointsOfRowRange()
        {
            Grid<int> grid = new Grid<int>(3, 2);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.PointsOfRow(-10));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.PointsOfRow(-2));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.PointsOfRow(-1));

            TestPointsOfRow(grid, 0);
            TestPointsOfRow(grid, 1);
            TestPointsOfRow(grid, 2);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.PointsOfRow(3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.PointsOfRow(4));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.PointsOfRow(10));
        }

        private void TestPointsOfRow<T>(Grid<T> grid, int rowIndex)
        {
            int columnIndex = 0;
            foreach (GridPoint p in grid.PointsOfRow(rowIndex))
            {
                Assert.AreEqual(new GridPoint(rowIndex, columnIndex), p);
                columnIndex++;
            }
            Assert.AreEqual(grid.NumberOfColumns, columnIndex);
        }

        [TestMethod]
        public void TestPointsOfRow()
        {
            Grid<int> grid;
            
            grid = new Grid<int>(3, 1);

            TestPointsOfRow(grid, 0);

            grid = new Grid<int>(3, 2);

            TestPointsOfRow(grid, 0);

            grid = new Grid<int>(3, 10);

            TestPointsOfRow(grid, 0);
        }

        [TestMethod]
        public void TestPointsOfColumnRange()
        {
            Grid<int> grid = new Grid<int>(2, 3);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.PointsOfColumn(-10));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.PointsOfColumn(-2));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.PointsOfColumn(-1));

            TestPointsOfColumn(grid, 0);
            TestPointsOfColumn(grid, 1);
            TestPointsOfColumn(grid, 2);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.PointsOfColumn(3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.PointsOfColumn(4));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => grid.PointsOfColumn(10));
        }

        private void TestPointsOfColumn<T>(Grid<T> grid, int columnIndex)
        {
            int rowIndex = 0;
            foreach (GridPoint p in grid.PointsOfColumn(columnIndex))
            {
                Assert.AreEqual(new GridPoint(rowIndex, columnIndex), p);
                rowIndex++;
            }
            Assert.AreEqual(grid.NumberOfRows, rowIndex);
        }

        [TestMethod]
        public void TestPointsOfColumn()
        {
            Grid<int> grid;

            grid = new Grid<int>(1, 3);

            TestPointsOfColumn(grid, 0);

            grid = new Grid<int>(2, 3);

            TestPointsOfColumn(grid, 0);

            grid = new Grid<int>(10, 3);

            TestPointsOfColumn(grid, 0);
        }

        [TestMethod]
        public void TestAllPoints()
        {
            var grid = new Grid<int>(2, 3);

            var expectedPointsInGrid = new HashSet<GridPoint>()
            {   new GridPoint(0, 0),
                new GridPoint(0, 1),
                new GridPoint(0, 2),
                new GridPoint(1, 0),
                new GridPoint(1, 1),
                new GridPoint(1, 2),
            };

            foreach (GridPoint p in grid.AllPoints())
            {
                Assert.IsTrue(expectedPointsInGrid.Remove(p));
            }

            Assert.AreEqual(0, expectedPointsInGrid.Count);
        }
    }
}
