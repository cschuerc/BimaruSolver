using BimaruGame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BimaruTest
{
    [TestClass]
    public class TallyTests
    {
        [TestMethod]
        public void TestInvalidLength()
        {
            int length = -1;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Tally(length));
        }

        [TestMethod]
        public void TestDefaultZero()
        {
            int length = 3;

            var tally = new Tally(length);

            Assert.AreEqual(length, tally.Length);

            for (int index = 0; index < length; index++)
            {
                Assert.AreEqual(0, tally[index]);
            }
        }


        [TestMethod]
        public void TestInvalidIndex()
        {
            int length = 3;

            var tally = new Tally(length);

            Assert.ThrowsException<IndexOutOfRangeException>(() => tally[-1]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => tally[length]);
        }

        [TestMethod]
        public void TestSetAnsSum()
        {
            int length = 3;

            var tally = new Tally(length);

            tally[0] = 7;
            tally[1] = 5;
            tally[2] = 0;

            Assert.AreEqual(7, tally[0]);
            Assert.AreEqual(5, tally[1]);
            Assert.AreEqual(0, tally[2]);

            Assert.AreEqual(7 + 5 + 0, tally.Sum);
        }

        [TestMethod]
        public void TestIsReadOnly()
        {
            int length = 3;

            var tally = new Tally(length);

            Assert.IsFalse(tally.IsReadOnly);

            tally[0] = 7;
            tally[1] = 5;
            tally[2] = 0;

            tally.IsReadOnly = true;

            Assert.IsTrue(tally.IsReadOnly);
            Assert.ThrowsException<InvalidOperationException>(() => tally[0] = 3);
            Assert.AreEqual(7, tally[0]);

            tally.IsReadOnly = false;
            tally[0] = 3;

            Assert.AreEqual(3, tally[0]);
        }
    }
}
