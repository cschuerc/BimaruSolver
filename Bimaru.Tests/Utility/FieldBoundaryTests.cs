using Bimaru.Interface.Utility;
using Xunit;

namespace Bimaru.Tests.Utility
{
    public class FieldBoundaryTests
    {
        [Fact]
        public void TestBoundaryEqualityVertical()
        {
            var b0 = new FieldBoundary(new GridPoint(5, 5), Direction.DOWN);
            var b1 = new FieldBoundary(new GridPoint(4, 5), Direction.UP);

            Assert.Equal(b0, b1);
        }

        [Fact]
        public void TestBoundaryEqualityHorizontal()
        {
            var b0 = new FieldBoundary(new GridPoint(4, 6), Direction.LEFT);
            var b1 = new FieldBoundary(new GridPoint(4, 5), Direction.RIGHT);

            Assert.Equal(b0, b1);
        }

        [Fact]
        public void TestBoundaryEqualityDiagonal()
        {
            var b0 = new FieldBoundary(new GridPoint(5, 6), Direction.LEFT_DOWN);
            var b1 = new FieldBoundary(new GridPoint(4, 5), Direction.RIGHT_UP);

            Assert.Equal(b0, b1);

            var b2 = new FieldBoundary(new GridPoint(3, 6), Direction.LEFT_UP);
            var b3 = new FieldBoundary(new GridPoint(4, 5), Direction.RIGHT_DOWN);

            Assert.Equal(b2, b3);
        }

        [Fact]
        public void TestBoundaryEqualityNotEqual()
        {
            var b0 = new FieldBoundary(new GridPoint(3, 5), Direction.LEFT_UP);
            var b1 = new FieldBoundary(new GridPoint(4, 6), Direction.RIGHT_DOWN);

            Assert.NotEqual(b0, b1);
        }
    }
}
