using Bimaru.Interface.Utility;
using Xunit;

namespace Bimaru.Tests.Utility
{
    public class GridPointTests
    {
        [Fact]
        public void TestRowColumnIndex()
        {
            const int rowIndex = 0;
            const int columnIndex = 3;

            var p = new GridPoint(rowIndex, columnIndex);

            Assert.Equal(rowIndex, p.RowIndex);
            Assert.Equal(columnIndex, p.ColumnIndex);
        }

        [Theory]
        [InlineData(Direction.DOWN, 1, 3)]
        [InlineData(Direction.LEFT, 2, 2)]
        [InlineData(Direction.LEFT_DOWN, 1, 2)]
        [InlineData(Direction.LEFT_UP, 3, 2)]
        [InlineData(Direction.RIGHT, 2, 4)]
        [InlineData(Direction.RIGHT_DOWN, 1, 4)]
        [InlineData(Direction.RIGHT_UP, 3, 4)]
        [InlineData(Direction.UP, 3, 3)]
        public void TestGetNextPoint(Direction direction, int expectedRowIndex, int expectedColumnIndex)
        {
            var point = new GridPoint(2, 3);

            var nextPoint = point.GetNextPoint(direction);

            Assert.Equal(new GridPoint(expectedRowIndex, expectedColumnIndex), nextPoint);
        }
    }
}
