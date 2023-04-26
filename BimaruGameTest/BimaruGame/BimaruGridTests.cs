using System.Collections.Generic;
using System.Linq;
using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace Bimaru.Test
{
    [TestClass]
    public class BimaruGridTests
    {
        [TestMethod]
        public void TestDefaultFieldValues()
        {
            var grid = new BimaruGrid(3, 4);

            foreach (var p in grid.AllPoints())
            {
                Assert.AreEqual(BimaruValue.UNDETERMINED, grid[p]);
            }
        }

        [TestMethod]
        public void TestGetOffGridFieldValues()
        {
            var grid = new BimaruGrid(3, 4);

            foreach (var p in GetSomeOffGridPoints(grid))
            {
                Assert.AreEqual(BimaruValue.WATER, grid[p]);
            }
        }

        private IEnumerable<GridPoint> GetSomeOffGridPoints(BimaruGrid grid)
        {
            yield return new GridPoint(0, -1);
            yield return new GridPoint(-1, 0);
            yield return new GridPoint(-1, -1);

            yield return new GridPoint(0, grid.NumberOfColumns);
            yield return new GridPoint(-1, grid.NumberOfColumns - 1);
            yield return new GridPoint(-1, grid.NumberOfColumns);

            yield return new GridPoint(grid.NumberOfRows, 0);
            yield return new GridPoint(grid.NumberOfRows - 1, -1);
            yield return new GridPoint(grid.NumberOfRows, -1);

            yield return new GridPoint(grid.NumberOfRows - 1, grid.NumberOfColumns);
            yield return new GridPoint(grid.NumberOfRows, grid.NumberOfColumns - 1);
            yield return new GridPoint(grid.NumberOfRows, grid.NumberOfColumns);
        }

        [TestMethod]
        public void TestSetOffGridFieldValuesToWater()
        {
            var grid = new BimaruGrid(3, 4);

            foreach (var p in GetSomeOffGridPoints(grid))
            {
                grid[p] = BimaruValue.WATER;
            }
        }

        [TestMethod]
        public void TestSetOffGridFieldValuesToNotWater()
        {
            var grid = new BimaruGrid(3, 4);

            foreach (var p in GetSomeOffGridPoints(grid))
            {
                foreach (var v in BimaruValues.AllBimaruValues().Where(v => v != BimaruValue.WATER))
                {
                    Assert.ThrowsException<InvalidFieldValueChangeException>(() => grid[p] = v);
                }
            }
        }

        [TestMethod]
        public void TestFillUndeterminedFieldsColumnToNo()
        {
            var grid = new BimaruGrid(2, 3);

            grid.FillUndeterminedFieldsColumn(1, BimaruValueConstraint.NO);

            var expectedFieldValues = new[,]
            {
                { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED }
            };

            grid.AssertEqual(expectedFieldValues);
        }

        [TestMethod]
        public void TestFillUndeterminedFieldsColumnToWater()
        {
            var grid = new BimaruGrid(2, 3);

            grid.FillUndeterminedFieldsColumn(1, BimaruValueConstraint.WATER);

            var expectedFieldValues = new[,]
            {
                { BimaruValue.UNDETERMINED, BimaruValue.WATER, BimaruValue.UNDETERMINED },
                { BimaruValue.UNDETERMINED, BimaruValue.WATER, BimaruValue.UNDETERMINED }
            };

            grid.AssertEqual(expectedFieldValues);
        }

        [TestMethod]
        public void TestFillUndeterminedFieldsColumnToShip()
        {
            var grid = new BimaruGrid(2, 3);

            grid.FillUndeterminedFieldsColumn(1, BimaruValueConstraint.SHIP);

            var expectedFieldValues = new[,]
            {
                { BimaruValue.UNDETERMINED, BimaruValue.SHIP_UNDETERMINED, BimaruValue.UNDETERMINED },
                { BimaruValue.UNDETERMINED, BimaruValue.SHIP_UNDETERMINED, BimaruValue.UNDETERMINED }
            };

            grid.AssertEqual(expectedFieldValues);
        }

        [TestMethod]
        public void TestFillUndeterminedFieldsColumnPreset()
        {
            var grid = new BimaruGrid(2, 3)
            {
                [new GridPoint(0, 1)] = BimaruValue.WATER
            };

            grid.FillUndeterminedFieldsColumn(1, BimaruValueConstraint.SHIP);

            var expectedFieldValues = new[,]
            {
                { BimaruValue.UNDETERMINED, BimaruValue.WATER, BimaruValue.UNDETERMINED },
                { BimaruValue.UNDETERMINED, BimaruValue.SHIP_UNDETERMINED, BimaruValue.UNDETERMINED }
            };

            grid.AssertEqual(expectedFieldValues);
        }

        [TestMethod]
        public void TestFillUndeterminedFieldsRowToNo()
        {
            var grid = new BimaruGrid(2, 3);

            grid.FillUndeterminedFieldsRow(1, BimaruValueConstraint.NO);

            var expectedFieldValues = new[,]
            {
                { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED }
            };

            grid.AssertEqual(expectedFieldValues);
        }

        [TestMethod]
        public void TestFillUndeterminedFieldsRowToWater()
        {
            var grid = new BimaruGrid(2, 3);

            grid.FillUndeterminedFieldsRow(1, BimaruValueConstraint.WATER);

            var expectedFieldValues = new[,]
            {
                { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER }
            };

            grid.AssertEqual(expectedFieldValues);
        }

        [TestMethod]
        public void TestFillUndeterminedFieldsRowToShip()
        {
            var grid = new BimaruGrid(2, 3);

            grid.FillUndeterminedFieldsRow(1, BimaruValueConstraint.SHIP);

            var expectedFieldValues = new[,]
            {
                { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                { BimaruValue.SHIP_UNDETERMINED, BimaruValue.SHIP_UNDETERMINED, BimaruValue.SHIP_UNDETERMINED }
            };

            grid.AssertEqual(expectedFieldValues);
        }

        [TestMethod]
        public void TestFillUndeterminedFieldsRowPreset()
        {
            var grid = new BimaruGrid(2, 3)
            {
                [new GridPoint(1, 0)] = BimaruValue.WATER
            };

            grid.FillUndeterminedFieldsRow(1, BimaruValueConstraint.SHIP);

            var expectedFieldValues = new[,]
            {
                { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                { BimaruValue.WATER, BimaruValue.SHIP_UNDETERMINED, BimaruValue.SHIP_UNDETERMINED }
            };

            grid.AssertEqual(expectedFieldValues);
        }

        [TestMethod]
        public void TestNumberOfShipFields()
        {
            var numRows = 4;
            var numColumns = 3;
            var grid = new BimaruGrid(numRows, numColumns);

            Assert.IsTrue(grid.NumberOfUndeterminedFieldsPerColumn.SequenceEqual(new int[numColumns].InitValues(numRows)));
            Assert.IsTrue(grid.NumberOfShipFieldsPerColumn.SequenceEqual(new int[numColumns].InitValues(0)));
            Assert.IsTrue(grid.NumberOfUndeterminedFieldsPerRow.SequenceEqual(new int[numRows].InitValues(numColumns)));
            Assert.IsTrue(grid.NumberOfShipFieldsPerRow.SequenceEqual(new int[numRows].InitValues(0)));

            // SUW
            // UUS
            // UUU
            // SUU
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            grid[new GridPoint(3, 2)] = BimaruValue.WATER;
            grid[new GridPoint(3, 0)] = BimaruValue.SHIP_MIDDLE;
            grid[new GridPoint(2, 2)] = BimaruValue.SHIP_SINGLE;
            grid[new GridPoint(2, 2)] = BimaruValue.WATER;
            grid[new GridPoint(2, 2)] = BimaruValue.SHIP_UNDETERMINED;

            Assert.IsTrue(grid.NumberOfUndeterminedFieldsPerColumn.SequenceEqual(new[] { 2, 4, 2 }));
            Assert.IsTrue(grid.NumberOfShipFieldsPerColumn.SequenceEqual(new[] { 2, 0, 1 }));
            Assert.IsTrue(grid.NumberOfUndeterminedFieldsPerRow.SequenceEqual(new[] { 2, 3, 2, 1 }));
            Assert.IsTrue(grid.NumberOfShipFieldsPerRow.SequenceEqual(new[] { 1, 0, 1, 1 }));
        }

        [TestMethod]
        public void TestIsFullyDetermined()
        {
            var grid = new BimaruGrid(1, 2);
            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            Assert.IsFalse(grid.IsFullyDetermined);
            grid[p0] = BimaruValue.WATER;
            grid[p1] = BimaruValue.SHIP_UNDETERMINED;
            Assert.IsFalse(grid.IsFullyDetermined);
            grid[p1] = BimaruValue.SHIP_CONT_DOWN;
            Assert.IsTrue(grid.IsFullyDetermined);
            grid[p1] = BimaruValue.SHIP_CONT_LEFT;
            Assert.IsTrue(grid.IsFullyDetermined);
            grid[p1] = BimaruValue.SHIP_CONT_RIGHT;
            Assert.IsTrue(grid.IsFullyDetermined);
            grid[p1] = BimaruValue.SHIP_CONT_UP;
            Assert.IsTrue(grid.IsFullyDetermined);
            grid[p1] = BimaruValue.SHIP_MIDDLE;
            Assert.IsTrue(grid.IsFullyDetermined);
            grid[p1] = BimaruValue.SHIP_SINGLE;
            Assert.IsTrue(grid.IsFullyDetermined);
        }

        [TestMethod]
        public void TestIsValidDiagonal()
        {
            var grid = new BimaruGrid(2, 2);
            Assert.IsTrue(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_UNDETERMINED;
            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_UNDETERMINED;
            Assert.IsFalse(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            Assert.IsTrue(grid.IsValid);
        }

        [TestMethod]
        public void TestIsValidVertical()
        {
            var grid = new BimaruGrid(2, 2);
            Assert.IsTrue(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            grid[new GridPoint(1, 0)] = BimaruValue.SHIP_UNDETERMINED;
            Assert.IsFalse(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            Assert.IsTrue(grid.IsValid);
        }

        [TestMethod]
        public void TestIsValidHorizontal()
        {
            var grid = new BimaruGrid(2, 2);
            Assert.IsTrue(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            grid[new GridPoint(0, 1)] = BimaruValue.SHIP_UNDETERMINED;
            Assert.IsFalse(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            Assert.IsTrue(grid.IsValid);
        }

        [TestMethod]
        public void TestIsValidOffGrid()
        {
            var grid = new BimaruGrid(2, 2);
            Assert.IsTrue(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_DOWN;
            grid[new GridPoint(0, 1)] = BimaruValue.WATER;
            Assert.IsFalse(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            Assert.IsTrue(grid.IsValid);
        }

        [TestMethod]
        public void TestShipCountOne()
        {
            var grid = new BimaruGrid(4, 3)
            {
                // SWU
                // UUS
                // UUU
                // SUU
                [new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE,
                [new GridPoint(3, 0)] = BimaruValue.SHIP_CONT_DOWN,
                [new GridPoint(3, 1)] = BimaruValue.WATER,
                [new GridPoint(2, 2)] = BimaruValue.SHIP_SINGLE
            };

            Assert.IsTrue(new[] { 0, 2, 0, 0, 0 }.SequenceEqual(grid.NumberOfShipsPerLength));
        }

        [TestMethod]
        public void TestShipCountTwo()
        {
            var grid = new BimaruGrid(4, 3)
            {
                // SWU
                // SUU
                // USS
                // UUU
                [new GridPoint(2, 0)] = BimaruValue.SHIP_CONT_UP,
                [new GridPoint(3, 0)] = BimaruValue.SHIP_CONT_DOWN,
                [new GridPoint(3, 1)] = BimaruValue.WATER,
                [new GridPoint(1, 1)] = BimaruValue.SHIP_CONT_RIGHT,
                [new GridPoint(1, 2)] = BimaruValue.SHIP_CONT_LEFT
            };

            Assert.IsTrue(new[] { 0, 0, 2, 0, 0 }.SequenceEqual(grid.NumberOfShipsPerLength));
        }

        [TestMethod]
        public void TestShipCountThree()
        {
            var grid = new BimaruGrid(4, 3)
            {
                // SSS
                // UUS
                // SUS
                // SUS
                [new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_UP,
                [new GridPoint(1, 0)] = BimaruValue.SHIP_UNDETERMINED,
                [new GridPoint(3, 0)] = BimaruValue.SHIP_CONT_RIGHT,
                [new GridPoint(3, 1)] = BimaruValue.SHIP_MIDDLE,
                [new GridPoint(3, 2)] = BimaruValue.SHIP_CONT_LEFT,
                [new GridPoint(0, 2)] = BimaruValue.SHIP_CONT_UP,
                [new GridPoint(1, 2)] = BimaruValue.SHIP_MIDDLE,
                [new GridPoint(2, 2)] = BimaruValue.SHIP_CONT_DOWN
            };

            Assert.IsTrue(new[] { 0, 0, 0, 2, 0 }.SequenceEqual(grid.NumberOfShipsPerLength));
        }

        [TestMethod]
        public void TestShipCountFour()
        {
            var grid = new BimaruGrid(4, 3)
            {
                // SUS
                // UUS
                // SUS
                // SUS
                [new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_UP,
                [new GridPoint(1, 0)] = BimaruValue.SHIP_MIDDLE,
                [new GridPoint(3, 0)] = BimaruValue.SHIP_CONT_DOWN,
                [new GridPoint(0, 2)] = BimaruValue.SHIP_CONT_UP,
                [new GridPoint(1, 2)] = BimaruValue.SHIP_MIDDLE,
                [new GridPoint(2, 2)] = BimaruValue.SHIP_MIDDLE,
                [new GridPoint(3, 2)] = BimaruValue.SHIP_CONT_DOWN
            };

            Assert.IsTrue(new[] { 0, 0, 0, 0, 1 }.SequenceEqual(grid.NumberOfShipsPerLength));
        }

        [TestMethod]
        public void TestShipCountChaos()
        {
            var grid = new BimaruGrid(3, 3)
            {
                // USU
                // SSS
                // USU
                [new GridPoint(0, 1)] = BimaruValue.SHIP_CONT_UP,
                [new GridPoint(1, 0)] = BimaruValue.SHIP_CONT_RIGHT,
                [new GridPoint(1, 1)] = BimaruValue.SHIP_MIDDLE,
                [new GridPoint(1, 2)] = BimaruValue.SHIP_CONT_LEFT,
                [new GridPoint(2, 1)] = BimaruValue.SHIP_CONT_DOWN
            };

            Assert.IsTrue(new[] { 0, 0, 0, 2 }.SequenceEqual(grid.NumberOfShipsPerLength));

            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_SINGLE;

            Assert.IsTrue(new[] { 0, 1, 0, 0 }.SequenceEqual(grid.NumberOfShipsPerLength));

            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_CONT_RIGHT;

            Assert.IsTrue(new[] { 0, 0, 1, 0 }.SequenceEqual(grid.NumberOfShipsPerLength));

            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_MIDDLE;

            Assert.IsTrue(new[] { 0, 0, 0, 2 }.SequenceEqual(grid.NumberOfShipsPerLength));
        }

        [TestMethod]
        public void TestClone()
        {
            var grid = new BimaruGrid(1, 2);
            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            grid[p0] = BimaruValue.SHIP_SINGLE;
            grid[p1] = BimaruValue.UNDETERMINED;

            var clonedGrid = (BimaruGrid)grid.Clone();

            grid.AssertEqual(clonedGrid);

            grid[p1] = BimaruValue.SHIP_SINGLE;

            Assert.AreEqual(BimaruValue.UNDETERMINED, clonedGrid[p1]);
        }

        [TestMethod]
        public void TestOverwriteWith()
        {
            var grid = new BimaruGrid(1, 2);
            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            grid[p0] = BimaruValue.SHIP_SINGLE;
            grid[p1] = BimaruValue.UNDETERMINED;

            var overwrittenGrid = new BimaruGrid(2, 3);

            overwrittenGrid.OverwriteWith(grid);

            grid.AssertEqual(overwrittenGrid);
        }
    }
}
