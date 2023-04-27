using System;
using Bimaru.Game;
using Bimaru.Interface;
using Xunit;

namespace Bimaru.Test
{
    public class ShipTargetTests
    {
        [Fact]
        public void TestCreation()
        {
            Assert.Null(Record.Exception(() => new ShipTarget()));
        }

        [Theory]
        [InlineData(-10, typeof(ArgumentOutOfRangeException))]
        [InlineData(-1, typeof(ArgumentOutOfRangeException))]
        [InlineData(0, typeof(ArgumentOutOfRangeException))]
        [InlineData(1, null)]
        [InlineData(2, null)]
        [InlineData(10, null)]
        public void TestShipLengthRange(int shipLength, Type expectedExceptionType)
        {
            var shipTarget = new ShipTarget();

            var caughtException = Record.Exception(() => shipTarget[shipLength] = 1);

            Assert.Equal(expectedExceptionType, caughtException?.GetType());
        }

        [Theory]
        [InlineData(-10, typeof(ArgumentOutOfRangeException))]
        [InlineData(-2, typeof(ArgumentOutOfRangeException))]
        [InlineData(-1, typeof(ArgumentOutOfRangeException))]
        [InlineData(0, null)]
        [InlineData(1, null)]
        [InlineData(10, null)]
        public void TestNumberOfShipsRange(int numberOfShips, Type expectedExceptionType)
        {
            var shipTarget = new ShipTarget();

            var caughtException = Record.Exception(() => shipTarget[1] = numberOfShips);

            Assert.Equal(expectedExceptionType, caughtException?.GetType());
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public void TestDefaultZero(int shipLength)
        {
            var shipTarget = new ShipTarget();

            Assert.Equal(0, shipTarget[shipLength]);
        }

        [Fact]
        public void TestSetTarget()
        {
            var shipTarget = new ShipTarget
            {
                [1] = 3,
                [2] = 3,
                [3] = 6
            };

            Assert.Equal(3, shipTarget[1]);
            Assert.Equal(3, shipTarget[2]);
            Assert.Equal(6, shipTarget[3]);
            Assert.Equal(0, shipTarget[4]);
        }

        [Fact]
        public void TestTotalShipFields()
        {
            var shipTarget = new ShipTarget();

            Assert.Equal(0, shipTarget.TotalShipFields);

            shipTarget[1] = 3;

            Assert.Equal(3 * 1, shipTarget.TotalShipFields);

            shipTarget[2] = 4;

            Assert.Equal(3 * 1 + 4 * 2, shipTarget.TotalShipFields);

            shipTarget[2] = 0;

            Assert.Equal(3 * 1, shipTarget.TotalShipFields);
        }

        [Fact]
        public void TestLongestShipLength()
        {
            var shipTarget = new ShipTarget();

            Assert.Null(shipTarget.LongestShipLength);

            shipTarget[1] = 3;

            Assert.Equal(1, shipTarget.LongestShipLength);

            shipTarget[3] = 1;

            Assert.Equal(3, shipTarget.LongestShipLength);

            shipTarget[1] = 0;

            Assert.Equal(3, shipTarget.LongestShipLength);

            shipTarget[3] = 0;

            Assert.Null(shipTarget.LongestShipLength);
        }

        [Theory]
        [InlineData(0, Satisfiability.SATISFIED)]
        [InlineData(1, Satisfiability.VIOLATED)]
        [InlineData(2, Satisfiability.VIOLATED)]
        public void TestSatisfiabilityNoShips(int numberOfSingleShips, Satisfiability expectedSatisfiability)
        {
            var shipTarget = new ShipTarget();

            var numberOfShipsPerLength = new[] { 0, numberOfSingleShips };
            Assert.Equal(expectedSatisfiability, shipTarget.GetSatisfiability(numberOfShipsPerLength));
        }

        [Theory]
        [InlineData(0, Satisfiability.SATISFIABLE)]
        [InlineData(1, Satisfiability.SATISFIABLE)]
        [InlineData(2, Satisfiability.SATISFIABLE)]
        [InlineData(3, Satisfiability.SATISFIED)]
        [InlineData(4, Satisfiability.VIOLATED)]
        [InlineData(5, Satisfiability.VIOLATED)]
        public void TestSatisfiabilitySingleLength(int numberOfSingleShips, Satisfiability expectedSatisfiability)
        {
            var shipTarget = new ShipTarget
            {
                [1] = 3
            };

            var numberOfShipsPerLength = new[] { 0, numberOfSingleShips };
            Assert.Equal(expectedSatisfiability, shipTarget.GetSatisfiability(numberOfShipsPerLength));
        }

        [Theory]
        [InlineData(0, 0, Satisfiability.SATISFIABLE)]
        [InlineData(0, 1, Satisfiability.SATISFIABLE)]
        [InlineData(0, 2, Satisfiability.VIOLATED)]
        [InlineData(2, 1, Satisfiability.SATISFIED)]
        [InlineData(3, 1, Satisfiability.VIOLATED)]
        [InlineData(3, 2, Satisfiability.VIOLATED)]
        public void TestSatisfiabilitySeveralLength(int numberOfSingleShips, int numberOfDoubleShips, Satisfiability expectedSatisfiability)
        {
            var shipTarget = new ShipTarget
            {
                [1] = 2,
                [2] = 1
            };

            var numberOfShipsPerLength = new[] { 0, numberOfSingleShips, numberOfDoubleShips };
            Assert.Equal(expectedSatisfiability, shipTarget.GetSatisfiability(numberOfShipsPerLength));
        }

        [Fact]
        public void TestLengthOfLongestMissingShipNoShips()
        {
            var shipTarget = new ShipTarget();

            Assert.Null(shipTarget.LengthOfLongestMissingShip(new[] { 0, 0 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => shipTarget.LengthOfLongestMissingShip(new[] { 0, 1 }));
        }

        [Fact]
        public void TestLengthOfLongestMissingShip()
        {
            var shipTarget = new ShipTarget
            {
                [1] = 2,
                [2] = 1
            };

            Assert.Equal(2, shipTarget.LengthOfLongestMissingShip(new[] { 0, 0, 0 }));
            Assert.Equal(1, shipTarget.LengthOfLongestMissingShip(new[] { 0, 0, 1 }));

            Assert.Null(shipTarget.LengthOfLongestMissingShip(new[] { 0, 2, 1 }));

            Assert.Throws<ArgumentOutOfRangeException>(() => shipTarget.LengthOfLongestMissingShip(new[] { 0, 0, 2 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => shipTarget.LengthOfLongestMissingShip(new[] { 0, 3, 1 }));
        }
    }
}
