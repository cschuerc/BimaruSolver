using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Utility
{
    [TestClass]
    public class FieldsToChangeTests
    {
        [TestMethod]
        public void TestNoChanges()
        {
            var changes = new FieldsToChange<int>();

            Assert.AreEqual(0, changes.Count());
        }

        [TestMethod]
        public void TestSingleChange()
        {
            var p12 = new GridPoint(1, 2);
            var changes = new FieldsToChange<int>(p12, 7);

            Assert.AreEqual(1, changes.Count());
            Assert.AreEqual(p12, changes.ElementAt(0).Point);
            Assert.AreEqual(7, changes.ElementAt(0).NewValue);
        }

        [TestMethod]
        public void TestAdd()
        {
            var p12 = new GridPoint(1, 2);
            var p14 = new GridPoint(1, 4);
            var changes = new FieldsToChange<int>
            {
                { p12, 7 },
                { p14, 8 }
            };

            Assert.AreEqual(2, changes.Count());
            Assert.AreEqual(p12, changes.ElementAt(0).Point);
            Assert.AreEqual(7, changes.ElementAt(0).NewValue);
            Assert.AreEqual(p14, changes.ElementAt(1).Point);
            Assert.AreEqual(8, changes.ElementAt(1).NewValue);
        }

        [TestMethod]
        public void TestSeveralChanges()
        {
            int numValues = 2;
            var p12 = new GridPoint(1, 2);
            var changes = new FieldsToChange<int>(
                p12,
                Direction.RIGHT,
                Enumerable.Range(7, numValues));

            Assert.AreEqual(numValues, changes.Count());
            Assert.AreEqual(p12, changes.ElementAt(0).Point);
            Assert.AreEqual(7, changes.ElementAt(0).NewValue);
            Assert.AreEqual(new GridPoint(1, 3), changes.ElementAt(1).Point);
            Assert.AreEqual(8, changes.ElementAt(1).NewValue);
        }

        [TestMethod]
        public void TestAddSegment()
        {
            int numValues = 2;
            var p12 = new GridPoint(1, 2);
            var changes = new FieldsToChange<int>();

            changes.AddSegment(
                p12,
                Direction.RIGHT,
                Enumerable.Range(7, numValues));

            Assert.AreEqual(numValues, changes.Count());
            Assert.AreEqual(p12, changes.ElementAt(0).Point);
            Assert.AreEqual(7, changes.ElementAt(0).NewValue);
            Assert.AreEqual(new GridPoint(1, 3), changes.ElementAt(1).Point);
            Assert.AreEqual(8, changes.ElementAt(1).NewValue);
        }

        [TestMethod]
        public void TestOverwrite()
        {
            var p12 = new GridPoint(1, 2);
            var changes = new FieldsToChange<int>
            {
                { p12, 7 },
                { p12, 9 },
                { p12, 1 },
            };

            Assert.AreEqual(1, changes.Count());
            Assert.AreEqual(p12, changes.ElementAt(0).Point);
            Assert.AreEqual(1, changes.ElementAt(0).NewValue);
        }
    }
}
