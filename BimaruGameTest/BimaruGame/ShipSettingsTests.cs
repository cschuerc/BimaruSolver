using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BimaruGame
{
    [TestClass]
    public class ShipSettingsTests
    {
        [TestMethod]
        public void TestInvalidShipLength()
        {
            int shipLength = 0;

            var settings = new ShipSettings();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => settings[shipLength] = 1);
        }

        [TestMethod]
        public void TestDefaultZero()
        {
            var settings = new ShipSettings();

            Assert.AreEqual(0, settings[1]);
            Assert.AreEqual(0, settings[17]);
            Assert.AreEqual(0, settings[0]);
        }

        [TestMethod]
        public void TestInvalidNumShips()
        {
            int shipLength = 1;

            var settings = new ShipSettings();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => settings[shipLength] = -1);
        }

        [TestMethod]
        public void TestSetAndSum()
        {
            var settings = new ShipSettings();

            settings[1] = 3;
            settings[1]++;
            settings[2] = 3;
            settings[2] = 0;
            settings[3] = 6;

            Assert.AreEqual(4, settings[1]);
            Assert.AreEqual(0, settings[2]);
            Assert.AreEqual(6, settings[3]);

            Assert.AreEqual(4 * 1 + 0 * 2 + 6 * 3, settings.NumShipFields);
        }

        [TestMethod]
        public void TestLongestShipLength()
        {
            var settings = new ShipSettings();

            settings[1] = 3;
            settings[1]++;
            settings[3] = 6;

            Assert.AreEqual(3, settings.LongestShipLength);

            settings[3] = 0;

            Assert.AreEqual(1, settings.LongestShipLength);

            settings[1] = 0;

            Assert.AreEqual(0, settings.LongestShipLength);
        }
    }
}
