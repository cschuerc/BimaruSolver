using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BimaruGame
{
    [TestClass]
    public class TallyTests
    {
        [TestMethod]
        public void TestInvalidLength()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Tally(-1));
            new Tally(0);
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
        public void TestNegativeTallyValues()
        {
            int length = 3;

            var tally = new Tally(length);

            tally[0] = 0;
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => tally[0] = -1);

            Assert.AreEqual(0, tally[0]);
        }

        [TestMethod]
        public void TestSetAndSum()
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
    }
}
