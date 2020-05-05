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
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Tally(0));
            new Tally(1);
        }

        [TestMethod]
        public void TestValidLength()
        {
            int length = 3;
            var tally = new Tally(length);

            Assert.AreEqual(length, tally.Length);
        }

        [TestMethod]
        public void TestDefaultZero()
        {
            int length = 3;
            var tally = new Tally(length);

            for (int i = 0; i < length; i++)
            {
                Assert.AreEqual(0, tally[i]);
            }
        }


        [TestMethod]
        public void TestInvalidIndex()
        {
            int length = 3;
            var tally = new Tally(length);

            Assert.ThrowsException<IndexOutOfRangeException>(() => tally[-1]);
            int t = tally[0];
            t = tally[length - 1];
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

            Assert.AreEqual(0, tally.Sum);

            tally[0] = 7;
            tally[1] = 5;
            tally[2] = 0;

            Assert.AreEqual(7, tally[0]);
            Assert.AreEqual(5, tally[1]);
            Assert.AreEqual(0, tally[2]);
            Assert.AreEqual(7 + 5 + 0, tally.Sum);
        }

        [TestMethod]
        public void TestEnumerator()
        {
            int length = 3;
            var tally = new Tally(length);

            tally[0] = 7;
            tally[1] = 5;
            tally[2] = 0;

            int index = 0;
            foreach (var t in tally)
            {
                Assert.AreEqual(tally[index], t);
                index++;
            }
        }
    }
}
