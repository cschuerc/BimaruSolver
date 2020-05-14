using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace BimaruGame
{
    [TestClass]
    public class GridTallyTests
    {
        [TestMethod]
        public void TestCreation()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GridTally(-100));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GridTally(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GridTally(0));

            new GridTally(1);
            new GridTally(2);
            new GridTally(10);
        }

        [TestMethod]
        public void TestLength()
        {
            GridTally tally;

            tally = new GridTally(1);
            Assert.AreEqual(1, tally.Length);

            tally = new GridTally(2);
            Assert.AreEqual(2, tally.Length);

            tally = new GridTally(10);
            Assert.AreEqual(10, tally.Length);
        }

        [TestMethod]
        public void TestIndex()
        {
            int length = 3;
            var tally = new GridTally(length);

            Assert.ThrowsException<IndexOutOfRangeException>(() => tally[-10]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => tally[-2]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => tally[-1]);

            int dummy;
            dummy = tally[0];
            dummy = tally[length - 1];

            Assert.ThrowsException<IndexOutOfRangeException>(() => tally[length]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => tally[length + 1]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => tally[length + 10]);
        }

        [TestMethod]
        public void TestDefaultValueZero()
        {
            var tally = new GridTally(3);

            for (int i = 0; i < tally.Length; i++)
            {
                Assert.AreEqual(0, tally[i]);
            }
        }

        [TestMethod]
        public void TestValueRange()
        {
            var tally = new GridTally(3);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => tally[0] = -10);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => tally[1] = -2);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => tally[2] = -1);

            tally[0] = 0;
            tally[1] = 1;
            tally[2] = 10;
        }

        [TestMethod]
        public void TestValueSet()
        {
            var tally = new GridTally(3);

            tally[0] = 7;
            tally[1] = 5;
            tally[2] = 0;

            Assert.AreEqual(7, tally[0]);
            Assert.AreEqual(5, tally[1]);
            Assert.AreEqual(0, tally[2]);
        }

        [TestMethod]
        public void TestTotal()
        {
            var tally = new GridTally(3);

            Assert.AreEqual(0, tally.Total);

            tally[0] = 7;

            Assert.AreEqual(7, tally.Total);

            tally[1] = 5;

            Assert.AreEqual(12, tally.Total);

            tally[2] = 0;

            Assert.AreEqual(12, tally.Total);
        }

        [TestMethod]
        public void TestSatisfiabilityLengthRange()
        {
            var tally = new GridTally(3);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => tally.GetSatisfiability(new int[2], new int[2]));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => tally.GetSatisfiability(new int[2], new int[3]));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => tally.GetSatisfiability(new int[3], new int[2]));
            tally.GetSatisfiability(new int[3], new int[3]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => tally.GetSatisfiability(new int[3], new int[4]));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => tally.GetSatisfiability(new int[4], new int[3]));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => tally.GetSatisfiability(new int[4], new int[4]));
        }

        [TestMethod]
        public void TestSatisfiabilityNoAdditional()
        {
            var tally = new GridTally(1);
            tally[0] = 3;

            Assert.AreEqual(Satisfiability.VIOLATED, tally.GetSatisfiability(new int[1] { 1 }, new int[1] { 0 }));
            Assert.AreEqual(Satisfiability.VIOLATED, tally.GetSatisfiability(new int[1] { 2 }, new int[1] { 0 }));
            Assert.AreEqual(Satisfiability.SATISFIED, tally.GetSatisfiability(new int[1] { 3 }, new int[1] { 0 }));
            Assert.AreEqual(Satisfiability.VIOLATED, tally.GetSatisfiability(new int[1] { 4 }, new int[1] { 0 }));
        }

        [TestMethod]
        public void TestSatisfiabilityAdditional()
        {
            var tally = new GridTally(1);
            tally[0] = 3;

            Assert.AreEqual(Satisfiability.VIOLATED, tally.GetSatisfiability(new int[1] { 0 }, new int[1] { 1 }));
            Assert.AreEqual(Satisfiability.VIOLATED, tally.GetSatisfiability(new int[1] { 0 }, new int[1] { 2 }));
            Assert.AreEqual(Satisfiability.SATISFIABLE, tally.GetSatisfiability(new int[1] { 0 }, new int[1] { 3 }));
            Assert.AreEqual(Satisfiability.SATISFIABLE, tally.GetSatisfiability(new int[1] { 0 }, new int[1] { 4 }));
        }

        [TestMethod]
        public void TestSatisfiabilityCombination()
        {
            var tally = new GridTally(1);
            tally[0] = 3;

            Assert.AreEqual(Satisfiability.VIOLATED, tally.GetSatisfiability(new int[1] { 1 }, new int[1] { 1 }));
            Assert.AreEqual(Satisfiability.SATISFIABLE, tally.GetSatisfiability(new int[1] { 1 }, new int[1] { 2 }));
            Assert.AreEqual(Satisfiability.SATISFIED, tally.GetSatisfiability(new int[1] { 3 }, new int[1] { 0 }));
            Assert.AreEqual(Satisfiability.SATISFIED, tally.GetSatisfiability(new int[1] { 3 }, new int[1] { 1 }));
            Assert.AreEqual(Satisfiability.VIOLATED, tally.GetSatisfiability(new int[1] { 4 }, new int[1] { 0 }));
        }

        [TestMethod]
        public void TestSatisfiabilityMoreThanOne()
        {
            var tally = new GridTally(2);
            tally[0] = 1;
            tally[1] = 2;

            Assert.AreEqual(Satisfiability.SATISFIABLE, tally.GetSatisfiability(new int[2] { 0, 0 }, new int[2] { 1, 2 }));
            Assert.AreEqual(Satisfiability.SATISFIABLE, tally.GetSatisfiability(new int[2] { 1, 0 }, new int[2] { 0, 2 }));
            Assert.AreEqual(Satisfiability.SATISFIABLE, tally.GetSatisfiability(new int[2] { 0, 2 }, new int[2] { 1, 0 }));

            Assert.AreEqual(Satisfiability.SATISFIED, tally.GetSatisfiability(new int[2] { 1, 2 }, new int[2] { 0, 0 }));

            Assert.AreEqual(Satisfiability.VIOLATED, tally.GetSatisfiability(new int[2] { 2, 1 }, new int[2] { 0, 1 }));
            Assert.AreEqual(Satisfiability.VIOLATED, tally.GetSatisfiability(new int[2] { 0, 3 }, new int[2] { 1, 0 }));
            Assert.AreEqual(Satisfiability.VIOLATED, tally.GetSatisfiability(new int[2] { 2, 2 }, new int[2] { 0, 0 }));
            Assert.AreEqual(Satisfiability.VIOLATED, tally.GetSatisfiability(new int[2] { 1, 3 }, new int[2] { 0, 0 }));
            Assert.AreEqual(Satisfiability.VIOLATED, tally.GetSatisfiability(new int[2] { 2, 3 }, new int[2] { 0, 0 }));
        }

        [TestMethod]
        public void TestEnumerator()
        {
            var tally = new GridTally(3);

            tally[0] = 7;
            tally[1] = 5;
            tally[2] = 0;

            Assert.AreEqual(tally.Length, tally.Count());

            int index = 0;
            foreach (int t in tally)
            {
                Assert.AreEqual(tally[index], t);

                index++;
            }
        }
    }
}
