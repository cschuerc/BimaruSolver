using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace Bimaru.Test
{
    [TestClass]
    public class FieldBoundaryTests
    {
        [TestMethod]
        public void TestBoundaryEqualityVertical()
        {
            var b0 = new FieldBoundary(new GridPoint(5, 5), Direction.DOWN);
            var b1 = new FieldBoundary(new GridPoint(4, 5), Direction.UP);

            Assert.AreEqual(b0, b1);
        }

        [TestMethod]
        public void TestBoundaryEqualityHorizontal()
        {
            var b0 = new FieldBoundary(new GridPoint(4, 6), Direction.LEFT);
            var b1 = new FieldBoundary(new GridPoint(4, 5), Direction.RIGHT);

            Assert.AreEqual(b0, b1);
        }

        [TestMethod]
        public void TestBoundaryEqualityDiagonal()
        {
            var b0 = new FieldBoundary(new GridPoint(5, 6), Direction.LEFT_DOWN);
            var b1 = new FieldBoundary(new GridPoint(4, 5), Direction.RIGHT_UP);

            Assert.AreEqual(b0, b1);

            var b2 = new FieldBoundary(new GridPoint(3, 6), Direction.LEFT_UP);
            var b3 = new FieldBoundary(new GridPoint(4, 5), Direction.RIGHT_DOWN);

            Assert.AreEqual(b2, b3);
        }

        [TestMethod]
        public void TestBoundaryEqualityNotEqual()
        {
            var b0 = new FieldBoundary(new GridPoint(3, 5), Direction.LEFT_UP);
            var b1 = new FieldBoundary(new GridPoint(4, 6), Direction.RIGHT_DOWN);

            Assert.AreNotEqual(b0, b1);
        }
    }
}
