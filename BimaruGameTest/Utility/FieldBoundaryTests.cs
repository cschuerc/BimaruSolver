using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utility
{
    [TestClass]
    public class FieldBoundaryTests
    {
        [TestMethod]
        public void TestBoundaryUniqueness()
        {
            var b0 = new FieldBoundary(new GridPoint(5, 5), Direction.DOWN);
            var b1 = new FieldBoundary(new GridPoint(4, 5), Direction.UP);

            Assert.AreEqual(b0, b1);

            var b2 = new FieldBoundary(new GridPoint(5, 6), Direction.LEFT_DOWN);
            var b3 = new FieldBoundary(new GridPoint(4, 5), Direction.RIGHT_UP);

            Assert.AreEqual(b2, b3);

            var b4 = new FieldBoundary(new GridPoint(4, 6), Direction.LEFT);
            var b5 = new FieldBoundary(new GridPoint(4, 5), Direction.RIGHT);

            Assert.AreEqual(b4, b5);

            var b6 = new FieldBoundary(new GridPoint(3, 6), Direction.LEFT_UP);
            var b7 = new FieldBoundary(new GridPoint(4, 5), Direction.RIGHT_DOWN);

            Assert.AreEqual(b6, b7);

            var b8 = new FieldBoundary(new GridPoint(3, 5), Direction.LEFT_UP);
            var b9 = new FieldBoundary(new GridPoint(4, 6), Direction.RIGHT_DOWN);

            Assert.AreNotEqual(b8, b9);
        }
    }
}
