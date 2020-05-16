using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Bimaru.Interfaces
{
    [TestClass]
    public class BimaruValueTests
    {
        [TestMethod]
        public void TestCompatibilitySingleShip()
        {
            foreach (Direction direction in Directions.GetAllDirections())
            {
                foreach (BimaruValue value in BimaruValues.AllBimaruValues())
                {
                    // Single ships are incompatible with any ship neighbours
                    bool shouldBeCompatible = !value.IsShip();
                    Assert.AreEqual(shouldBeCompatible, BimaruValue.SHIP_SINGLE.IsCompatibleWith(direction, value));
                    Assert.AreEqual(shouldBeCompatible, value.IsCompatibleWith(direction, BimaruValue.SHIP_SINGLE));
                }
            }
        }

        [TestMethod]
        public void TestCompatibilityDiagonal()
        {
            foreach (Direction direction in Directions.GetDirections(DirectionType.DIAGONAL))
            {
                foreach (BimaruValue value in BimaruValues.AllBimaruValues())
                {
                    foreach (BimaruValue neighbourValue in BimaruValues.AllBimaruValues())
                    {
                        // Ship fields are incompatible with neighbour ship fields at the diagonal
                        bool shouldBeCompatible = !value.IsShip() || !neighbourValue.IsShip();
                        Assert.AreEqual(shouldBeCompatible, value.IsCompatibleWith(direction, neighbourValue));
                        Assert.AreEqual(shouldBeCompatible, neighbourValue.IsCompatibleWith(direction, value));
                    }
                }
            }
        }

        [TestMethod]
        public void TestCompatibilityStartEnd()
        {
            var startEndShipFields =
                new BimaruValue[]
                {
                    BimaruValue.SHIP_CONT_DOWN,
                    BimaruValue.SHIP_CONT_LEFT,
                    BimaruValue.SHIP_CONT_RIGHT,
                    BimaruValue.SHIP_CONT_UP
                };

            foreach (Direction direction in Directions.GetNonDiagonalDirections())
            {
                foreach (BimaruValue value in startEndShipFields)
                {
                    foreach (BimaruValue neighbourValue in BimaruValues.AllBimaruValues())
                    {
                        bool isValidShipContinuation =
                            neighbourValue == BimaruValue.SHIP_MIDDLE ||
                            neighbourValue == BimaruValue.SHIP_UNDETERMINED ||
                            neighbourValue == BimaruValue.UNDETERMINED ||
                            neighbourValue == direction.GetLastShipValue();

                        bool shouldBeCompatible =
                            (value != direction.GetFirstShipValue() && !neighbourValue.IsShip()) ||
                            (value == direction.GetFirstShipValue() && isValidShipContinuation);

                        Assert.AreEqual(shouldBeCompatible, value.IsCompatibleWith(direction, neighbourValue));
                        Assert.AreEqual(shouldBeCompatible, neighbourValue.IsCompatibleWith(direction.GetOpposite(), value));
                    }
                }
            }
        }

        [TestMethod]
        public void TestCompatibilityRemaining()
        {
            var remainingShipFields =
                new BimaruValue[]
                {
                    BimaruValue.UNDETERMINED,
                    BimaruValue.WATER,
                    BimaruValue.SHIP_UNDETERMINED,
                    BimaruValue.SHIP_MIDDLE
                };

            foreach (Direction direction in Directions.GetNonDiagonalDirections())
            {
                foreach (BimaruValue value in remainingShipFields)
                {
                    foreach (BimaruValue neighbourValue in remainingShipFields)
                    {
                        Assert.IsTrue(value.IsCompatibleWith(direction, neighbourValue));
                        Assert.IsTrue(neighbourValue.IsCompatibleWith(direction, value));
                    }
                }
            }
        }

        [TestMethod]
        public void TestWaterIsCompatibleChangeTo()
        {
            Assert.IsTrue(BimaruValue.WATER.IsCompatibleChangeTo(BimaruValue.WATER));
            Assert.IsFalse(BimaruValue.WATER.IsCompatibleChangeTo(BimaruValue.UNDETERMINED));
            Assert.IsFalse(BimaruValue.WATER.IsCompatibleChangeTo(BimaruValue.SHIP_UNDETERMINED));
            Assert.IsFalse(BimaruValue.WATER.IsCompatibleChangeTo(BimaruValue.SHIP_SINGLE));
            Assert.IsFalse(BimaruValue.WATER.IsCompatibleChangeTo(BimaruValue.SHIP_CONT_RIGHT));
        }

        [TestMethod]
        public void TestUndeterminedIsCompatibleChangeTo()
        {
            Assert.IsTrue(BimaruValue.UNDETERMINED.IsCompatibleChangeTo(BimaruValue.WATER));
            Assert.IsTrue(BimaruValue.UNDETERMINED.IsCompatibleChangeTo(BimaruValue.UNDETERMINED));
            Assert.IsTrue(BimaruValue.UNDETERMINED.IsCompatibleChangeTo(BimaruValue.SHIP_UNDETERMINED));
            Assert.IsTrue(BimaruValue.UNDETERMINED.IsCompatibleChangeTo(BimaruValue.SHIP_SINGLE));
            Assert.IsTrue(BimaruValue.UNDETERMINED.IsCompatibleChangeTo(BimaruValue.SHIP_CONT_RIGHT));
        }

        [TestMethod]
        public void TestShipUndeterminedIsCompatibleChangeTo()
        {
            Assert.IsFalse(BimaruValue.SHIP_UNDETERMINED.IsCompatibleChangeTo(BimaruValue.WATER));
            Assert.IsFalse(BimaruValue.SHIP_UNDETERMINED.IsCompatibleChangeTo(BimaruValue.UNDETERMINED));
            Assert.IsTrue(BimaruValue.SHIP_UNDETERMINED.IsCompatibleChangeTo(BimaruValue.SHIP_UNDETERMINED));
            Assert.IsTrue(BimaruValue.SHIP_UNDETERMINED.IsCompatibleChangeTo(BimaruValue.SHIP_SINGLE));
            Assert.IsTrue(BimaruValue.SHIP_UNDETERMINED.IsCompatibleChangeTo(BimaruValue.SHIP_CONT_RIGHT));
        }

        [TestMethod]
        public void TestSpecificShipIsCompatibleChangeTo()
        {
            // Any specific ship is only compatible to itself
            Assert.IsTrue(BimaruValue.SHIP_SINGLE.IsCompatibleChangeTo(BimaruValue.SHIP_SINGLE));
            Assert.IsFalse(BimaruValue.SHIP_SINGLE.IsCompatibleChangeTo(BimaruValue.UNDETERMINED));
            Assert.IsFalse(BimaruValue.SHIP_SINGLE.IsCompatibleChangeTo(BimaruValue.WATER));
            Assert.IsFalse(BimaruValue.SHIP_SINGLE.IsCompatibleChangeTo(BimaruValue.SHIP_UNDETERMINED));
            Assert.IsFalse(BimaruValue.SHIP_SINGLE.IsCompatibleChangeTo(BimaruValue.SHIP_MIDDLE));
            Assert.IsFalse(BimaruValue.SHIP_SINGLE.IsCompatibleChangeTo(BimaruValue.SHIP_CONT_RIGHT));
        }

        [TestMethod]
        public void TestFieldValuesOfShipLengthZero()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => BimaruValues.FieldValuesOfShip(Direction.RIGHT, 0).GetEnumerator().MoveNext());
        }

        [TestMethod]
        public void TestFieldValuesOfShipLengthOne()
        {
            AssertEqualFieldValues(
                new List<BimaruValue>() { BimaruValue.SHIP_SINGLE },
                BimaruValues.FieldValuesOfShip(Direction.RIGHT, 1));

            AssertEqualFieldValues(
                new List<BimaruValue>() { BimaruValue.SHIP_SINGLE },
                BimaruValues.FieldValuesOfShip(Direction.LEFT, 1));

            AssertEqualFieldValues(
                new List<BimaruValue>() { BimaruValue.SHIP_SINGLE },
                BimaruValues.FieldValuesOfShip(Direction.UP, 1));

            AssertEqualFieldValues(
                new List<BimaruValue>() { BimaruValue.SHIP_SINGLE },
                BimaruValues.FieldValuesOfShip(Direction.DOWN, 1));
        }

        [TestMethod]
        public void TestFieldValuesOfShipLengthTwo()
        {
            AssertEqualFieldValues(
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_RIGHT,
                    BimaruValue.SHIP_CONT_LEFT
                },
                BimaruValues.FieldValuesOfShip(Direction.RIGHT, 2));

            AssertEqualFieldValues(
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_LEFT,
                    BimaruValue.SHIP_CONT_RIGHT
                },
                BimaruValues.FieldValuesOfShip(Direction.LEFT, 2));

            AssertEqualFieldValues(
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_UP,
                    BimaruValue.SHIP_CONT_DOWN
                },
                BimaruValues.FieldValuesOfShip(Direction.UP, 2));

            AssertEqualFieldValues(
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_DOWN,
                    BimaruValue.SHIP_CONT_UP
                },
                BimaruValues.FieldValuesOfShip(Direction.DOWN, 2));
        }

        [TestMethod]
        public void TestFieldValuesOfShipLengthFour()
        {
            AssertEqualFieldValues(
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_RIGHT,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_CONT_LEFT
                },
                BimaruValues.FieldValuesOfShip(Direction.RIGHT, 4));

            AssertEqualFieldValues(
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_LEFT,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_CONT_RIGHT
                },
                BimaruValues.FieldValuesOfShip(Direction.LEFT, 4));

            AssertEqualFieldValues(
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_UP,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_CONT_DOWN
                },
                BimaruValues.FieldValuesOfShip(Direction.UP, 4));

            AssertEqualFieldValues(
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_DOWN,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_CONT_UP
                },
                BimaruValues.FieldValuesOfShip(Direction.DOWN, 4));
        }

        private void AssertEqualFieldValues(IEnumerable<BimaruValue> expectedFieldValues, IEnumerable<BimaruValue> actualFieldValues)
        {
            Assert.IsTrue(expectedFieldValues.SequenceEqual(actualFieldValues));
        }
    }
}
