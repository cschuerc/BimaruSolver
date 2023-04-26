using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace Bimaru.Test
{
    [TestClass]
    public class GridPointTests
    {
        [TestMethod]
        public void TestRowColumnIndex()
        {
            const int rowIndex = 0;
            const int columnIndex = 3;

            var p = new GridPoint(rowIndex, columnIndex);

            Assert.AreEqual(rowIndex, p.RowIndex);
            Assert.AreEqual(columnIndex, p.ColumnIndex);
        }

        [TestMethod]
        public void TestGetNextPoint()
        {
            var point = new GridPoint(2, 3);
            GridPoint nextPoint;

            nextPoint = point.GetNextPoint(Direction.DOWN);
            Assert.AreEqual(new GridPoint(1, 3), nextPoint);

            nextPoint = point.GetNextPoint(Direction.LEFT);
            Assert.AreEqual(new GridPoint(2, 2), nextPoint);

            nextPoint = point.GetNextPoint(Direction.LEFT_DOWN);
            Assert.AreEqual(new GridPoint(1, 2), nextPoint);

            nextPoint = point.GetNextPoint(Direction.LEFT_UP);
            Assert.AreEqual(new GridPoint(3, 2), nextPoint);

            nextPoint = point.GetNextPoint(Direction.RIGHT);
            Assert.AreEqual(new GridPoint(2, 4), nextPoint);

            nextPoint = point.GetNextPoint(Direction.RIGHT_DOWN);
            Assert.AreEqual(new GridPoint(1, 4), nextPoint);

            nextPoint = point.GetNextPoint(Direction.RIGHT_UP);
            Assert.AreEqual(new GridPoint(3, 4), nextPoint);

            nextPoint = point.GetNextPoint(Direction.UP);
            Assert.AreEqual(new GridPoint(3, 3), nextPoint);
        }
    }
}
