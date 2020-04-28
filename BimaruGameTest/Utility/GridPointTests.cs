using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utility
{
    [TestClass]
    public class GridPointTests
    {
        [TestMethod]
        public void TestValidRowColumnIndex()
        {
            int rowIndex = 0;
            int columnIndex = 3;

            GridPoint p = new GridPoint(rowIndex, columnIndex);

            Assert.AreEqual(rowIndex, p.RowIndex);
            Assert.AreEqual(columnIndex, p.ColumnIndex);
        }

        [TestMethod]
        public void TestGetNextPoint()
        {
            int rowIndex = 2;
            int columnIndex = 3;

            GridPoint point = new GridPoint(rowIndex, columnIndex);
            GridPoint nextPoint;

            nextPoint = point.GetNextPoint(Direction.DOWN);
            Assert.AreEqual(new GridPoint(rowIndex - 1, columnIndex), nextPoint);

            nextPoint = point.GetNextPoint(Direction.LEFT);
            Assert.AreEqual(new GridPoint(rowIndex, columnIndex - 1), nextPoint);

            nextPoint = point.GetNextPoint(Direction.LEFT_DOWN);
            Assert.AreEqual(new GridPoint(rowIndex - 1, columnIndex - 1), nextPoint);

            nextPoint = point.GetNextPoint(Direction.LEFT_UP);
            Assert.AreEqual(new GridPoint(rowIndex + 1, columnIndex - 1), nextPoint);

            nextPoint = point.GetNextPoint(Direction.RIGHT);
            Assert.AreEqual(new GridPoint(rowIndex, columnIndex + 1), nextPoint);

            nextPoint = point.GetNextPoint(Direction.RIGHT_DOWN);
            Assert.AreEqual(new GridPoint(rowIndex - 1, columnIndex + 1), nextPoint);

            nextPoint = point.GetNextPoint(Direction.RIGHT_UP);
            Assert.AreEqual(new GridPoint(rowIndex + 1, columnIndex + 1), nextPoint);

            nextPoint = point.GetNextPoint(Direction.UP);
            Assert.AreEqual(new GridPoint(rowIndex + 1, columnIndex), nextPoint);
        }
    }
}
