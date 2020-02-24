using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace BimaruTest
{
    [TestClass]
    public class FieldBoundaryTests
    {
        [TestMethod]
        public void TestBoundaryUniqueness()
        {
            var b0 = new FieldBoundary(new GridPoint(5, 5), Directions.DOWN);
            var b1 = new FieldBoundary(new GridPoint(4, 5), Directions.UP);

            Assert.AreEqual(b0, b1);

            var b2 = new FieldBoundary(new GridPoint(5, 6), Directions.LEFT_DOWN);
            var b3 = new FieldBoundary(new GridPoint(4, 5), Directions.RIGHT_UP);

            Assert.AreEqual(b2, b3);

            var b4 = new FieldBoundary(new GridPoint(4, 6), Directions.LEFT);
            var b5 = new FieldBoundary(new GridPoint(4, 5), Directions.RIGHT);

            Assert.AreEqual(b4, b5);

            var b6 = new FieldBoundary(new GridPoint(3, 6), Directions.LEFT_UP);
            var b7 = new FieldBoundary(new GridPoint(4, 5), Directions.RIGHT_DOWN);

            Assert.AreEqual(b6, b7);
        }
    }
}
