using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimaru;

namespace BimaruTest
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

            IGridPoint point = new GridPoint(rowIndex, columnIndex);
            IGridPoint nextPoint;

            nextPoint = point.GetNextPoint(Directions.DOWN);
            Assert.AreEqual(new GridPoint(rowIndex - 1, columnIndex), nextPoint);

            nextPoint = point.GetNextPoint(Directions.LEFT);
            Assert.AreEqual(new GridPoint(rowIndex, columnIndex - 1), nextPoint);

            nextPoint = point.GetNextPoint(Directions.LEFT_DOWN);
            Assert.AreEqual(new GridPoint(rowIndex - 1, columnIndex - 1), nextPoint);

            nextPoint = point.GetNextPoint(Directions.LEFT_UP);
            Assert.AreEqual(new GridPoint(rowIndex + 1, columnIndex - 1), nextPoint);

            nextPoint = point.GetNextPoint(Directions.RIGHT);
            Assert.AreEqual(new GridPoint(rowIndex, columnIndex + 1), nextPoint);

            nextPoint = point.GetNextPoint(Directions.RIGHT_DOWN);
            Assert.AreEqual(new GridPoint(rowIndex - 1, columnIndex + 1), nextPoint);

            nextPoint = point.GetNextPoint(Directions.RIGHT_UP);
            Assert.AreEqual(new GridPoint(rowIndex + 1, columnIndex + 1), nextPoint);

            nextPoint = point.GetNextPoint(Directions.UP);
            Assert.AreEqual(new GridPoint(rowIndex + 1, columnIndex), nextPoint);
        }
    }
}
