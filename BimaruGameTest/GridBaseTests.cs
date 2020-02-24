using BimaruGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Utility;

namespace BimaruTest
{
    [TestClass]
    public class GridBaseTests
    {
        [TestMethod]
        public void TestPositiveNumColumns()
        {
            int numRows = 3;
            int numColumns = 0;

            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => new GridBase<int>(numRows, numColumns));
        }

        [TestMethod]
        public void TestPositiveNumRows()
        {
            int numRows = 0;
            int numColumns = 3;

            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => new GridBase<int>(numRows, numColumns));
        }

        [TestMethod]
        public void TestInvalidRowIndex()
        {
            int numRows = 3;
            int numColumns = 4;
            var grid = new GridBase<int>(numRows, numColumns);

            var p = new GridPoint(-1, 0);
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => grid.GetFieldValue(p));

            p = new GridPoint(numRows, 0);
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => grid.GetFieldValue(p));
        }

        [TestMethod]
        public void TestInvalidColumnIndex()
        {
            int numRows = 3;
            int numColumns = 4;
            var grid = new GridBase<int>(numRows, numColumns);

            var p = new GridPoint(0, -1);
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => grid.GetFieldValue(p));

            p = new GridPoint(0, numColumns);
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => grid.GetFieldValue(p));
        }

        [TestMethod]
        public void TestDefaultValue()
        {
            int numRows = 2;
            int numColumns = 3;
            int defaultValue = 17;

            var grid = new GridBase<int>(numRows, numColumns, defaultValue);

            for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < numColumns; columnIndex++)
                {
                    var p = new GridPoint(rowIndex, columnIndex);
                    Assert.AreEqual(defaultValue, grid.GetFieldValue(p));
                }
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
    }
}
