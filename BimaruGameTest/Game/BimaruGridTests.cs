using System.Collections.Generic;
using System.Linq;
using Bimaru.Game;
using Bimaru.Interface;
using Bimaru.Interface.Game;
using Utility;
using Xunit;

namespace Bimaru.Test
{
    public class BimaruGridTests
    {
        [Fact]
        public void TestDefaultFieldValues()
        {
            var grid = new BimaruGrid(3, 4);

            foreach (var p in grid.AllPoints())
            {
                Assert.Equal(BimaruValue.UNDETERMINED, grid[p]);
            }
        }

        [Fact]
        public void TestGetOffGridFieldValues()
        {
            var grid = new BimaruGrid(3, 4);

            foreach (var p in GetSomeOffGridPoints(grid))
            {
                Assert.Equal(BimaruValue.WATER, grid[p]);
            }
        }

        private static IEnumerable<GridPoint> GetSomeOffGridPoints(BimaruGrid grid)
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

        [Fact]
        public void TestSetOffGridFieldValuesToWater()
        {
            var grid = new BimaruGrid(3, 4);

            foreach (var p in GetSomeOffGridPoints(grid))
            {
                Assert.Null(Record.Exception(() => grid[p] = BimaruValue.WATER));
            }
        }

        [Fact]
        public void TestSetOffGridFieldValuesToNotWater()
        {
            var grid = new BimaruGrid(3, 4);

            foreach (var p in GetSomeOffGridPoints(grid))
            {
                foreach (var v in BimaruValues.AllBimaruValues().Where(v => v != BimaruValue.WATER))
                {
                    Assert.Throws<InvalidFieldValueChangeException>(() => grid[p] = v);
                }
            }
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void TestNumberOfShipFields()
        {
            var numRows = 4;
            var numColumns = 3;
            var grid = new BimaruGrid(numRows, numColumns);

            Assert.True(grid.NumberOfUndeterminedFieldsPerColumn.SequenceEqual(new int[numColumns].InitValues(numRows)));
            Assert.True(grid.NumberOfShipFieldsPerColumn.SequenceEqual(new int[numColumns].InitValues(0)));
            Assert.True(grid.NumberOfUndeterminedFieldsPerRow.SequenceEqual(new int[numRows].InitValues(numColumns)));
            Assert.True(grid.NumberOfShipFieldsPerRow.SequenceEqual(new int[numRows].InitValues(0)));

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

            Assert.True(grid.NumberOfUndeterminedFieldsPerColumn.SequenceEqual(new[] { 2, 4, 2 }));
            Assert.True(grid.NumberOfShipFieldsPerColumn.SequenceEqual(new[] { 2, 0, 1 }));
            Assert.True(grid.NumberOfUndeterminedFieldsPerRow.SequenceEqual(new[] { 2, 3, 2, 1 }));
            Assert.True(grid.NumberOfShipFieldsPerRow.SequenceEqual(new[] { 1, 0, 1, 1 }));
        }

        [Fact]
        public void TestIsFullyDetermined()
        {
            var grid = new BimaruGrid(1, 2);
            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            Assert.False(grid.IsFullyDetermined);
            grid[p0] = BimaruValue.WATER;
            grid[p1] = BimaruValue.SHIP_UNDETERMINED;
            Assert.False(grid.IsFullyDetermined);
            grid[p1] = BimaruValue.SHIP_CONT_DOWN;
            Assert.True(grid.IsFullyDetermined);
            grid[p1] = BimaruValue.SHIP_CONT_LEFT;
            Assert.True(grid.IsFullyDetermined);
            grid[p1] = BimaruValue.SHIP_CONT_RIGHT;
            Assert.True(grid.IsFullyDetermined);
            grid[p1] = BimaruValue.SHIP_CONT_UP;
            Assert.True(grid.IsFullyDetermined);
            grid[p1] = BimaruValue.SHIP_MIDDLE;
            Assert.True(grid.IsFullyDetermined);
            grid[p1] = BimaruValue.SHIP_SINGLE;
            Assert.True(grid.IsFullyDetermined);
        }

        [Fact]
        public void TestIsValidDiagonal()
        {
            var grid = new BimaruGrid(2, 2);
            Assert.True(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_UNDETERMINED;
            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_UNDETERMINED;
            Assert.False(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            Assert.True(grid.IsValid);
        }

        [Fact]
        public void TestIsValidVertical()
        {
            var grid = new BimaruGrid(2, 2);
            Assert.True(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            grid[new GridPoint(1, 0)] = BimaruValue.SHIP_UNDETERMINED;
            Assert.False(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            Assert.True(grid.IsValid);
        }

        [Fact]
        public void TestIsValidHorizontal()
        {
            var grid = new BimaruGrid(2, 2);
            Assert.True(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            grid[new GridPoint(0, 1)] = BimaruValue.SHIP_UNDETERMINED;
            Assert.False(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            Assert.True(grid.IsValid);
        }

        [Fact]
        public void TestIsValidOffGrid()
        {
            var grid = new BimaruGrid(2, 2);
            Assert.True(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_DOWN;
            grid[new GridPoint(0, 1)] = BimaruValue.WATER;
            Assert.False(grid.IsValid);
            grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            Assert.True(grid.IsValid);
        }

        [Fact]
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

            Assert.True(new[] { 0, 2, 0, 0, 0 }.SequenceEqual(grid.NumberOfShipsPerLength));
        }

        [Fact]
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

            Assert.True(new[] { 0, 0, 2, 0, 0 }.SequenceEqual(grid.NumberOfShipsPerLength));
        }

        [Fact]
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

            Assert.True(new[] { 0, 0, 0, 2, 0 }.SequenceEqual(grid.NumberOfShipsPerLength));
        }

        [Fact]
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

            Assert.True(new[] { 0, 0, 0, 0, 1 }.SequenceEqual(grid.NumberOfShipsPerLength));
        }

        [Fact]
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

            Assert.True(new[] { 0, 0, 0, 2 }.SequenceEqual(grid.NumberOfShipsPerLength));

            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_SINGLE;

            Assert.True(new[] { 0, 1, 0, 0 }.SequenceEqual(grid.NumberOfShipsPerLength));

            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_CONT_RIGHT;

            Assert.True(new[] { 0, 0, 1, 0 }.SequenceEqual(grid.NumberOfShipsPerLength));

            grid[new GridPoint(1, 1)] = BimaruValue.SHIP_MIDDLE;

            Assert.True(new[] { 0, 0, 0, 2 }.SequenceEqual(grid.NumberOfShipsPerLength));
        }

        [Fact]
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

            Assert.Equal(BimaruValue.UNDETERMINED, clonedGrid[p1]);
        }

        [Fact]
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
