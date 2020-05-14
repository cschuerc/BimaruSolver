using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BimaruGame
{
    [TestClass]
    public class ShipTargetTests
    {
        [TestMethod]
        public void TestCreation()
        {
            new ShipTarget();
        }

        [TestMethod]
        public void TestShipLengthRange()
        {
            var shipTarget = new ShipTarget();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => shipTarget[-10] = 1);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => shipTarget[-1] = 1);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => shipTarget[0] = 1);

            shipTarget[1] = 1;
            shipTarget[2] = 1;
            shipTarget[10] = 1;
        }

        public void TestNumberOfShipsRange()
        {
            var shipTarget = new ShipTarget();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => shipTarget[1] = -10);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => shipTarget[1] = -2);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => shipTarget[1] = -1);

            shipTarget[1] = 0;
            shipTarget[1] = 1;
            shipTarget[1] = 10;
        }

        [TestMethod]
        public void TestDefaultZero()
        {
            var shipTarget = new ShipTarget();

            Assert.AreEqual(0, shipTarget[-10]);
            Assert.AreEqual(0, shipTarget[-2]);
            Assert.AreEqual(0, shipTarget[-1]);
            Assert.AreEqual(0, shipTarget[0]);
            Assert.AreEqual(0, shipTarget[1]);
            Assert.AreEqual(0, shipTarget[10]);
        }

        [TestMethod]
        public void TestSetTarget()
        {
            var shipTarget = new ShipTarget();

            shipTarget[1] = 3;
            shipTarget[2] = 3;
            shipTarget[3] = 6;

            Assert.AreEqual(3, shipTarget[1]);
            Assert.AreEqual(3, shipTarget[2]);
            Assert.AreEqual(6, shipTarget[3]);
            Assert.AreEqual(0, shipTarget[4]);
        }

        [TestMethod]
        public void TestTotalShipFields()
        {
            var shipTarget = new ShipTarget();

            Assert.AreEqual(0, shipTarget.TotalShipFields);

            shipTarget[1] = 3;

            Assert.AreEqual(3*1, shipTarget.TotalShipFields);

            shipTarget[2] = 4;

            Assert.AreEqual(3*1 + 4*2, shipTarget.TotalShipFields);

            shipTarget[2] = 0;

            Assert.AreEqual(3*1, shipTarget.TotalShipFields);
        }

        [TestMethod]
        public void TestLongestShipLength()
        {
            var shipTarget = new ShipTarget();

            Assert.AreEqual(0, shipTarget.LongestShipLength);

            shipTarget[1] = 3;

            Assert.AreEqual(1, shipTarget.LongestShipLength);

            shipTarget[3] = 1;

            Assert.AreEqual(3, shipTarget.LongestShipLength);

            shipTarget[1] = 0;

            Assert.AreEqual(3, shipTarget.LongestShipLength);

            shipTarget[3] = 0;

            Assert.AreEqual(0, shipTarget.LongestShipLength);
        }

        [TestMethod]
        public void TestSatisfiabilitySingleLength()
        {
            var shipTarget = new ShipTarget();
            shipTarget[1] = 3;

            Assert.AreEqual(Satisfiability.SATISFIABLE, shipTarget.GetSatisfiability(new int[2] { 0, 0 }));
            Assert.AreEqual(Satisfiability.SATISFIABLE, shipTarget.GetSatisfiability(new int[2] { 0, 1 }));
            Assert.AreEqual(Satisfiability.SATISFIABLE, shipTarget.GetSatisfiability(new int[2] { 0, 2 }));
            Assert.AreEqual(Satisfiability.SATISFIED, shipTarget.GetSatisfiability(new int[2] { 0, 3 }));
            Assert.AreEqual(Satisfiability.VIOLATED, shipTarget.GetSatisfiability(new int[2] { 0, 4 }));
            Assert.AreEqual(Satisfiability.VIOLATED, shipTarget.GetSatisfiability(new int[2] { 0, 5 }));
        }

        [TestMethod]
        public void TestSatisfiabilitySeveralLength()
        {
            var shipTarget = new ShipTarget();
            shipTarget[1] = 2;
            shipTarget[2] = 1;

            Assert.AreEqual(Satisfiability.SATISFIABLE, shipTarget.GetSatisfiability(new int[3] { 0, 0, 0 }));
            Assert.AreEqual(Satisfiability.SATISFIABLE, shipTarget.GetSatisfiability(new int[3] { 0, 0, 1 }));
            Assert.AreEqual(Satisfiability.VIOLATED, shipTarget.GetSatisfiability(new int[3] { 0, 0, 2 }));
            Assert.AreEqual(Satisfiability.SATISFIED, shipTarget.GetSatisfiability(new int[3] { 0, 2, 1 }));
            Assert.AreEqual(Satisfiability.VIOLATED, shipTarget.GetSatisfiability(new int[3] { 0, 3, 1 }));
            Assert.AreEqual(Satisfiability.VIOLATED, shipTarget.GetSatisfiability(new int[3] { 0, 3, 2 }));
        }
    }
}
