using System;
using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface.Utility;
using Bimaru.Utility;
using Xunit;

// ReSharper disable RedundantAssignment

namespace Bimaru.Tests.Utility
{
    public class GridTests
    {
        [Theory]
        [InlineData(-10, 1, typeof(ArgumentOutOfRangeException))]
        [InlineData(-1, 1, typeof(ArgumentOutOfRangeException))]
        [InlineData(0, 1, typeof(ArgumentOutOfRangeException))]
        [InlineData(1, 1, null)]
        [InlineData(2, 1, null)]
        [InlineData(10, 1, null)]
        [InlineData(1, 2, null)]
        [InlineData(1, 10, null)]
        [InlineData(1, -10, typeof(ArgumentOutOfRangeException))]
        [InlineData(1, -1, typeof(ArgumentOutOfRangeException))]
        [InlineData(1, 0, typeof(ArgumentOutOfRangeException))]

        public void TestGridSizeRange(int numberOfRows, int numberOfColumns, Type expectedExceptionType)
        {
            var caughtException = Record.Exception(() => new Grid<int>(numberOfRows, numberOfColumns));

            Assert.Equal(expectedExceptionType, caughtException?.GetType());
        }

        [Fact]
        public void TestNumberOfRowsColumns()
        {
            var grid = new Grid<int>(2, 3);

            Assert.Equal(2, grid.NumberOfRows);
            Assert.Equal(3, grid.NumberOfColumns);
        }

        [Fact]
        public void TestDefaultFieldValue()
        {
            const int defaultFieldValue = 17;

            var grid = new Grid<int>(2, 3, defaultFieldValue);

            foreach (var p in grid.AllPoints())
            {
                Assert.Equal(defaultFieldValue, grid[p]);
            }
        }

        [Theory]
        [InlineData(-1, 0, typeof(ArgumentOutOfRangeException))]
        [InlineData(3, 0, typeof(ArgumentOutOfRangeException))]
        [InlineData(0, -1, typeof(ArgumentOutOfRangeException))]
        [InlineData(0, 4, typeof(ArgumentOutOfRangeException))]
        [InlineData(0, 0, null)]
        [InlineData(2, 0, null)]
        [InlineData(0, 3, null)]
        [InlineData(2, 3, null)]
        public void TestIndexRange(int rowIndex, int columnIndex, Type expectedExceptionType)
        {
            var grid = new Grid<int>(3, 4);

            var caughtException = Record.Exception(() => grid[new GridPoint(rowIndex, columnIndex)]++);

            Assert.Equal(expectedExceptionType, caughtException?.GetType());
        }

        [Fact]
        public void TestGetSetFieldValue()
        {
            var grid = new Grid<int>(1, 2);
            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            Assert.Equal(0, grid[p0]);
            Assert.Equal(0, grid[p1]);

            grid[p0] = 15;

            Assert.Equal(15, grid[p0]);
            Assert.Equal(0, grid[p1]);

            grid[p1] = -7;

            Assert.Equal(15, grid[p0]);
            Assert.Equal(-7, grid[p1]);
        }

        [Fact]
        public void TestFieldValueChanged()
        {
            var grid = new Grid<int>(1, 2);
            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            var actualEventArgs = new List<FieldValueChangedEventArgs<int>>();

            grid.FieldValueChanged += delegate (object _, FieldValueChangedEventArgs<int> e)
            {
                actualEventArgs.Add(e);
            };

            grid[p0] = 15;
            grid[p1] = -7;
            grid[p1] = -7; // Should not trigger the event as no change
            grid[p0] = 16;

            var expectedEventArgs = new List<FieldValueChangedEventArgs<int>>()
            {
                new(p0, 0),
                new(p1, 0),
                new(p0, 15)
            };

            AssertEqualFieldValueChangedEventArgs(expectedEventArgs, actualEventArgs);
        }

        private static void AssertEqualFieldValueChangedEventArgs<T>(IReadOnlyList<FieldValueChangedEventArgs<T>> expected, IReadOnlyList<FieldValueChangedEventArgs<T>> actual)
        {
            Assert.Equal(expected.Count, actual.Count);

            foreach (var index in Enumerable.Range(0, expected.Count))
            {
                Assert.Equal(expected[index].Point, actual[index].Point);
                Assert.Equal(expected[index].OriginalValue, actual[index].OriginalValue);
            }
        }

        [Theory]
        [InlineData(-10, typeof(ArgumentOutOfRangeException))]
        [InlineData(-2, typeof(ArgumentOutOfRangeException))]
        [InlineData(-1, typeof(ArgumentOutOfRangeException))]
        [InlineData(0, null)]
        [InlineData(1, null)]
        [InlineData(2, null)]
        [InlineData(3, typeof(ArgumentOutOfRangeException))]
        [InlineData(4, typeof(ArgumentOutOfRangeException))]
        [InlineData(10, typeof(ArgumentOutOfRangeException))]
        public void TestPointsOfRowRange(int rowIndex, Type expectedExceptionType)
        {
            var grid = new Grid<int>(3, 2);

            var caughtException = Record.Exception(() => grid.PointsOfRow(rowIndex));

            Assert.Equal(expectedExceptionType, caughtException?.GetType());
        }

        private static void AssertPointsOfRow<T>(Grid<T> grid, int rowIndex)
        {
            var columnIndex = 0;
            foreach (var p in grid.PointsOfRow(rowIndex))
            {
                Assert.Equal(new GridPoint(rowIndex, columnIndex), p);
                columnIndex++;
            }
            Assert.Equal(grid.NumberOfColumns, columnIndex);
        }

        [Fact]
        public void TestPointsOfRow()
        {
            var grid = new Grid<int>(3, 1);

            AssertPointsOfRow(grid, 0);

            grid = new Grid<int>(3, 2);

            AssertPointsOfRow(grid, 0);

            grid = new Grid<int>(3, 10);

            AssertPointsOfRow(grid, 0);
        }

        [Theory]
        [InlineData(-10, typeof(ArgumentOutOfRangeException))]
        [InlineData(-2, typeof(ArgumentOutOfRangeException))]
        [InlineData(-1, typeof(ArgumentOutOfRangeException))]
        [InlineData(0, null)]
        [InlineData(1, null)]
        [InlineData(2, null)]
        [InlineData(3, typeof(ArgumentOutOfRangeException))]
        [InlineData(4, typeof(ArgumentOutOfRangeException))]
        [InlineData(10, typeof(ArgumentOutOfRangeException))]
        public void TestPointsOfColumnRange(int columnIndex, Type expectedExceptionType)
        {
            var grid = new Grid<int>(2, 3);

            var caughtException = Record.Exception(() => grid.PointsOfColumn(columnIndex));

            Assert.Equal(expectedExceptionType, caughtException?.GetType());
        }

        private static void AssertPointsOfColumn<T>(Grid<T> grid, int columnIndex)
        {
            var rowIndex = 0;
            foreach (var p in grid.PointsOfColumn(columnIndex))
            {
                Assert.Equal(new GridPoint(rowIndex, columnIndex), p);
                rowIndex++;
            }
            Assert.Equal(grid.NumberOfRows, rowIndex);
        }

        [Fact]
        public void TestPointsOfColumn()
        {
            var grid = new Grid<int>(1, 3);

            AssertPointsOfColumn(grid, 0);

            grid = new Grid<int>(2, 3);

            AssertPointsOfColumn(grid, 0);

            grid = new Grid<int>(10, 3);

            AssertPointsOfColumn(grid, 0);
        }

        [Fact]
        public void TestAllPoints()
        {
            var grid = new Grid<int>(2, 3);

            var expectedPointsInGrid = new HashSet<GridPoint>()
            {   new(0, 0),
                new(0, 1),
                new(0, 2),
                new(1, 0),
                new(1, 1),
                new(1, 2),
            };

            foreach (var p in grid.AllPoints())
            {
                Assert.True(expectedPointsInGrid.Remove(p));
            }

            Assert.Empty(expectedPointsInGrid);
        }
    }
}
