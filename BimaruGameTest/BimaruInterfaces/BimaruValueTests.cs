using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace BimaruInterfaces
{
    [TestClass]
    public class BimaruValueTests
    {
        [TestMethod]
        public void TestCompatibilitySingleShip()
        {
            // Sigle ships are incompatible with any ship neighbours
            foreach (Direction direction in DirectionExtensions.AllDirections())
            {
                foreach (BimaruValue value in BimaruValueExtensions.AllBimaruValues())
                {
                    bool shouldBeCompatible = !value.IsShip();
                    Assert.AreEqual(shouldBeCompatible, BimaruValue.SHIP_SINGLE.IsCompatibleWith(direction, value));
                    Assert.AreEqual(shouldBeCompatible, value.IsCompatibleWith(direction, BimaruValue.SHIP_SINGLE));
                }
            }
        }

        [TestMethod]
        public void TestCompatibilityDiagonal()
        {
            // Ship fields are incompatible with neighbour ship fields at the diagonal
            foreach (Direction direction in DirectionExtensions.AllDiagonalDirections())
            {
                foreach (BimaruValue value in BimaruValueExtensions.AllBimaruValues())
                {
                    foreach (BimaruValue neighbourValue in BimaruValueExtensions.AllBimaruValues())
                    {
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


            // Diagonal already checked
            foreach (Direction direction in DirectionExtensions.AllNonDiagonalDirections())
            {
                foreach (BimaruValue value in startEndShipFields)
                {
                    foreach (BimaruValue neighbourValue in BimaruValueExtensions.AllBimaruValues())
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

            // Diagonal already checked
            foreach (Direction direction in DirectionExtensions.AllNonDiagonalDirections())
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
        public void TestCompatibleChange()
        {
            // WATER is only compatible to WATER
            Assert.IsTrue(BimaruValue.WATER.IsCompatibleChange(BimaruValue.WATER));
            Assert.IsFalse(BimaruValue.WATER.IsCompatibleChange(BimaruValue.UNDETERMINED));
            Assert.IsFalse(BimaruValue.WATER.IsCompatibleChange(BimaruValue.SHIP_UNDETERMINED));
            Assert.IsFalse(BimaruValue.WATER.IsCompatibleChange(BimaruValue.SHIP_SINGLE));
            Assert.IsFalse(BimaruValue.WATER.IsCompatibleChange(BimaruValue.SHIP_CONT_RIGHT));

            // UNDETERMINED is compatible to every other value
            Assert.IsTrue(BimaruValue.UNDETERMINED.IsCompatibleChange(BimaruValue.WATER));
            Assert.IsTrue(BimaruValue.UNDETERMINED.IsCompatibleChange(BimaruValue.UNDETERMINED));
            Assert.IsTrue(BimaruValue.UNDETERMINED.IsCompatibleChange(BimaruValue.SHIP_UNDETERMINED));
            Assert.IsTrue(BimaruValue.UNDETERMINED.IsCompatibleChange(BimaruValue.SHIP_SINGLE));
            Assert.IsTrue(BimaruValue.UNDETERMINED.IsCompatibleChange(BimaruValue.SHIP_CONT_RIGHT));

            // SHIP_UNDETERMINED is compatible with any other ship value
            Assert.IsFalse(BimaruValue.SHIP_UNDETERMINED.IsCompatibleChange(BimaruValue.WATER));
            Assert.IsFalse(BimaruValue.SHIP_UNDETERMINED.IsCompatibleChange(BimaruValue.UNDETERMINED));
            Assert.IsTrue(BimaruValue.SHIP_UNDETERMINED.IsCompatibleChange(BimaruValue.SHIP_UNDETERMINED));
            Assert.IsTrue(BimaruValue.SHIP_UNDETERMINED.IsCompatibleChange(BimaruValue.SHIP_SINGLE));
            Assert.IsTrue(BimaruValue.SHIP_UNDETERMINED.IsCompatibleChange(BimaruValue.SHIP_CONT_RIGHT));

            // Any specific ship is only compatible to itself
            Assert.IsFalse(BimaruValue.SHIP_SINGLE.IsCompatibleChange(BimaruValue.WATER));
            Assert.IsFalse(BimaruValue.SHIP_SINGLE.IsCompatibleChange(BimaruValue.UNDETERMINED));
            Assert.IsFalse(BimaruValue.SHIP_SINGLE.IsCompatibleChange(BimaruValue.SHIP_UNDETERMINED));
            Assert.IsTrue(BimaruValue.SHIP_SINGLE.IsCompatibleChange(BimaruValue.SHIP_SINGLE));
            Assert.IsFalse(BimaruValue.SHIP_SINGLE.IsCompatibleChange(BimaruValue.SHIP_CONT_RIGHT));
        }

        private void CheckFieldValues(IEnumerable<BimaruValue> fieldValuesToCheck, IList<BimaruValue> fieldValuesCorrect)
        {
            int index = 0;
            foreach (BimaruValue value in fieldValuesToCheck)
            {
                Assert.AreEqual(fieldValuesCorrect[index], value);
                index++;
            }

            Assert.AreEqual(fieldValuesCorrect.Count, index);
        }

        [TestMethod]
        public void TestFieldValuesOfShip()
        {
            // Length should be at least 1
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => BimaruValueExtensions.FieldValuesOfShip(Direction.RIGHT , - 2).GetEnumerator().MoveNext());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => BimaruValueExtensions.FieldValuesOfShip(Direction.RIGHT, 0).GetEnumerator().MoveNext());

            // Only check RIGHT, LEFT, UP and DOWN as no ships exist in the other directions.
            CheckFieldValues(BimaruValueExtensions.FieldValuesOfShip(Direction.RIGHT, 1),
                new List<BimaruValue>() { BimaruValue.SHIP_SINGLE });

            CheckFieldValues(BimaruValueExtensions.FieldValuesOfShip(Direction.LEFT, 1),
                new List<BimaruValue>() { BimaruValue.SHIP_SINGLE });

            CheckFieldValues(BimaruValueExtensions.FieldValuesOfShip(Direction.UP, 1),
                new List<BimaruValue>() { BimaruValue.SHIP_SINGLE });

            CheckFieldValues(BimaruValueExtensions.FieldValuesOfShip(Direction.DOWN, 1),
                new List<BimaruValue>() { BimaruValue.SHIP_SINGLE });


            CheckFieldValues(BimaruValueExtensions.FieldValuesOfShip(Direction.RIGHT, 2),
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_RIGHT,
                    BimaruValue.SHIP_CONT_LEFT
                });

            CheckFieldValues(BimaruValueExtensions.FieldValuesOfShip(Direction.LEFT, 2),
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_LEFT,
                    BimaruValue.SHIP_CONT_RIGHT
                });

            CheckFieldValues(BimaruValueExtensions.FieldValuesOfShip(Direction.UP, 2),
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_UP,
                    BimaruValue.SHIP_CONT_DOWN
                });

            CheckFieldValues(BimaruValueExtensions.FieldValuesOfShip(Direction.DOWN, 2),
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_DOWN,
                    BimaruValue.SHIP_CONT_UP
                });


            CheckFieldValues(BimaruValueExtensions.FieldValuesOfShip(Direction.RIGHT, 4),
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_RIGHT,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_CONT_LEFT
                });

            CheckFieldValues(BimaruValueExtensions.FieldValuesOfShip(Direction.LEFT, 4),
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_LEFT,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_CONT_RIGHT
                });

            CheckFieldValues(BimaruValueExtensions.FieldValuesOfShip(Direction.UP, 4),
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_UP,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_CONT_DOWN
                });

            CheckFieldValues(BimaruValueExtensions.FieldValuesOfShip(Direction.DOWN, 4),
                new List<BimaruValue>()
                {   BimaruValue.SHIP_CONT_DOWN,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_MIDDLE,
                    BimaruValue.SHIP_CONT_UP
                });
        }
    }
}
