using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimaru;
using System;

namespace BimaruTest
{
    [TestClass]
    public class FieldValueTests
    {
        [TestMethod]
        public void TestCompatibilityOfUndetermined()
        {
            // Undetermined values give no constraints
            foreach (Directions direction in Enum.GetValues(typeof(Directions)))
            {
                foreach (FieldValues value in Enum.GetValues(typeof(FieldValues)))
                {
                    Assert.IsTrue(FieldValues.UNDETERMINED.IsCompatibleWith(direction, value));
                    Assert.IsTrue(value.IsCompatibleWith(direction, FieldValues.UNDETERMINED));
                }
            }
        }

        [TestMethod]
        public void TestCompatibilitySingleShip()
        {
            // Sigle ships are incompatible with any ship neighbours
            foreach (Directions direction in Enum.GetValues(typeof(Directions)))
            {
                foreach (FieldValues value in Enum.GetValues(typeof(FieldValues)))
                {
                    bool shouldBeCompatible = !value.IsShip();
                    Assert.AreEqual(shouldBeCompatible, FieldValues.SHIP_SINGLE.IsCompatibleWith(direction, value));
                    Assert.AreEqual(shouldBeCompatible, value.IsCompatibleWith(direction, FieldValues.SHIP_SINGLE));
                }
            }
        }

        [TestMethod]
        public void TestCompatibilityDiagonal()
        {
            // Ship fields are incompatible with neighbour ship fields at the diagonal
            foreach (Directions direction in Enum.GetValues(typeof(Directions)))
            {
                if (!direction.IsDiagonal())
                {
                    continue;
                }

                foreach (FieldValues value in Enum.GetValues(typeof(FieldValues)))
                {
                    foreach (FieldValues neighbourValue in Enum.GetValues(typeof(FieldValues)))
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
                new FieldValues[]
                {
                    FieldValues.SHIP_CONT_DOWN,
                    FieldValues.SHIP_CONT_LEFT,
                    FieldValues.SHIP_CONT_RIGHT,
                    FieldValues.SHIP_CONT_UP
                };

            foreach (Directions direction in Enum.GetValues(typeof(Directions)))
            {
                // Diagonal already checked
                if (direction.IsDiagonal())
                {
                    continue;
                }

                foreach (FieldValues value in startEndShipFields)
                {
                    foreach (FieldValues neighbourValue in Enum.GetValues(typeof(FieldValues)))
                    {
                        bool isValidShipContinuation =
                            neighbourValue == FieldValues.SHIP_MIDDLE ||
                            neighbourValue == FieldValues.SHIP_UNDETERMINED ||
                            neighbourValue == FieldValues.UNDETERMINED ||
                            neighbourValue == direction.GetEndFieldValue();

                        bool shouldBeCompatible =
                            (value != direction.GetStartFieldValue() && !neighbourValue.IsShip()) ||
                            (value == direction.GetStartFieldValue() && isValidShipContinuation);

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
                new FieldValues[]
                {
                    FieldValues.WATER,
                    FieldValues.SHIP_UNDETERMINED,
                    FieldValues.SHIP_MIDDLE
                };

            foreach (Directions direction in Enum.GetValues(typeof(Directions)))
            {
                // Diagonal already checked
                if (direction.IsDiagonal())
                {
                    continue;
                }

                foreach (FieldValues value in remainingShipFields)
                {
                    foreach (FieldValues neighbourValue in remainingShipFields)
                    {
                        Assert.IsTrue(value.IsCompatibleWith(direction, neighbourValue));
                        Assert.IsTrue(neighbourValue.IsCompatibleWith(direction, value));
                    }
                }
            }
        }
    }
}
