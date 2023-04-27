using System;
using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface.Utility;
using Xunit;

namespace Bimaru.Tests.Utility
{
    public class BimaruValueTests
    {
        [Fact]
        public void TestCompatibilitySingleShip()
        {
            foreach (var direction in Directions.GetAllDirections())
            {
                foreach (var value in BimaruValues.AllBimaruValues())
                {
                    // Single ships are incompatible with any ship neighbors
                    var shouldBeCompatible = !value.IsShip();
                    Assert.Equal(shouldBeCompatible, BimaruValue.SHIP_SINGLE.IsCompatibleWith(direction, value));
                    Assert.Equal(shouldBeCompatible, value.IsCompatibleWith(direction, BimaruValue.SHIP_SINGLE));
                }
            }
        }

        [Fact]
        public void TestCompatibilityDiagonal()
        {
            foreach (var direction in Directions.GetDirections(DirectionType.DIAGONAL))
            {
                foreach (var value in BimaruValues.AllBimaruValues())
                {
                    foreach (var neighborValue in BimaruValues.AllBimaruValues())
                    {
                        // Ship fields are incompatible with neighbor ship fields at the diagonal
                        var shouldBeCompatible = !value.IsShip() || !neighborValue.IsShip();
                        Assert.Equal(shouldBeCompatible, value.IsCompatibleWith(direction, neighborValue));
                        Assert.Equal(shouldBeCompatible, neighborValue.IsCompatibleWith(direction, value));
                    }
                }
            }
        }

        [Fact]
        public void TestCompatibilityStartEnd()
        {
            var startEndShipFields =
                new[]
                {
                    BimaruValue.SHIP_CONT_DOWN,
                    BimaruValue.SHIP_CONT_LEFT,
                    BimaruValue.SHIP_CONT_RIGHT,
                    BimaruValue.SHIP_CONT_UP
                };

            foreach (var direction in Directions.GetNonDiagonalDirections())
            {
                foreach (var value in startEndShipFields)
                {
                    foreach (var neighborValue in BimaruValues.AllBimaruValues())
                    {
                        var isValidShipContinuation =
                            neighborValue == BimaruValue.SHIP_MIDDLE ||
                            neighborValue == BimaruValue.SHIP_UNDETERMINED ||
                            neighborValue == BimaruValue.UNDETERMINED ||
                            neighborValue == direction.GetLastShipValue();

                        var shouldBeCompatible =
                            value != direction.GetFirstShipValue() && !neighborValue.IsShip() ||
                            value == direction.GetFirstShipValue() && isValidShipContinuation;

                        Assert.Equal(shouldBeCompatible, value.IsCompatibleWith(direction, neighborValue));
                        Assert.Equal(shouldBeCompatible, neighborValue.IsCompatibleWith(direction.GetOpposite(), value));
                    }
                }
            }
        }

        [Fact]
        public void TestCompatibilityRemaining()
        {
            var remainingShipFields =
                new[]
                {
                    BimaruValue.UNDETERMINED,
                    BimaruValue.WATER,
                    BimaruValue.SHIP_UNDETERMINED,
                    BimaruValue.SHIP_MIDDLE
                };

            foreach (var direction in Directions.GetNonDiagonalDirections())
            {
                foreach (var value in remainingShipFields)
                {
                    foreach (var neighborValue in remainingShipFields)
                    {
                        Assert.True(value.IsCompatibleWith(direction, neighborValue));
                        Assert.True(neighborValue.IsCompatibleWith(direction, value));
                    }
                }
            }
        }

        [Theory]
        [InlineData(BimaruValue.WATER, BimaruValue.WATER, true)]
        [InlineData(BimaruValue.WATER, BimaruValue.UNDETERMINED, false)]
        [InlineData(BimaruValue.WATER, BimaruValue.SHIP_UNDETERMINED, false)]
        [InlineData(BimaruValue.WATER, BimaruValue.SHIP_SINGLE, false)]
        [InlineData(BimaruValue.WATER, BimaruValue.SHIP_CONT_RIGHT, false)]
        [InlineData(BimaruValue.UNDETERMINED, BimaruValue.WATER, true)]
        [InlineData(BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, true)]
        [InlineData(BimaruValue.UNDETERMINED, BimaruValue.SHIP_UNDETERMINED, true)]
        [InlineData(BimaruValue.UNDETERMINED, BimaruValue.SHIP_SINGLE, true)]
        [InlineData(BimaruValue.UNDETERMINED, BimaruValue.SHIP_CONT_RIGHT, true)]
        [InlineData(BimaruValue.SHIP_UNDETERMINED, BimaruValue.WATER, false)]
        [InlineData(BimaruValue.SHIP_UNDETERMINED, BimaruValue.UNDETERMINED, false)]
        [InlineData(BimaruValue.SHIP_UNDETERMINED, BimaruValue.SHIP_UNDETERMINED, true)]
        [InlineData(BimaruValue.SHIP_UNDETERMINED, BimaruValue.SHIP_SINGLE, true)]
        [InlineData(BimaruValue.SHIP_UNDETERMINED, BimaruValue.SHIP_CONT_RIGHT, true)]
        [InlineData(BimaruValue.SHIP_SINGLE, BimaruValue.WATER, false)]
        [InlineData(BimaruValue.SHIP_SINGLE, BimaruValue.UNDETERMINED, false)]
        [InlineData(BimaruValue.SHIP_SINGLE, BimaruValue.SHIP_UNDETERMINED, false)]
        [InlineData(BimaruValue.SHIP_SINGLE, BimaruValue.SHIP_SINGLE, true)]
        [InlineData(BimaruValue.SHIP_SINGLE, BimaruValue.SHIP_CONT_RIGHT, false)]
        public void TestIsCompatibleChangeTo(BimaruValue actualValue, BimaruValue changedValue, bool expectedIsCompatibleChange)
        {
            Assert.Equal(expectedIsCompatibleChange, actualValue.IsCompatibleChangeTo(changedValue));
        }

        [Fact]
        public void TestFieldValuesOfShipLengthZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => BimaruValues.FieldValuesOfShip(Direction.RIGHT, 0).GetEnumerator().MoveNext());
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        private static void AssertEqualFieldValues(IEnumerable<BimaruValue> expectedFieldValues, IEnumerable<BimaruValue> actualFieldValues)
        {
            Assert.True(expectedFieldValues.SequenceEqual(actualFieldValues));
        }
    }
}
