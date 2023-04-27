using System;
using System.Collections.Generic;
using Bimaru.Game;
using Bimaru.Interface;
using Bimaru.Interface.Game;
using Utility;
using Xunit;

// ReSharper disable AccessToModifiedClosure

namespace Bimaru.Tests.Game
{
    public class GameTests
    {
        [Theory]
        [MemberData(nameof(CreateDataToTestNullArguments))]
        public void TestNullArguments(IGridTally rowTally, IGridTally columnTally, IShipTarget shipTarget, IBimaruGrid grid, Type expectedExceptionType)
        {
            var caughtException = Record.Exception(() => new BimaruGame(rowTally, columnTally, shipTarget, grid));

            Assert.Equal(expectedExceptionType, caughtException?.GetType());
        }

        public static IEnumerable<object[]> CreateDataToTestNullArguments()
        {
            const int numRows = 4;
            const int numColumns = 3;

            var rowTally = new GridTally(numRows);
            var columnTally = new GridTally(numColumns);
            var shipTarget = new ShipTarget();
            var grid = new BimaruGrid(numRows, numColumns);

            yield return new object[] { null, columnTally, shipTarget, grid, typeof(ArgumentNullException) };
            yield return new object[] { rowTally, null, shipTarget, grid, typeof(ArgumentNullException) };
            yield return new object[] { rowTally, columnTally, null, grid, typeof(ArgumentNullException) };
            yield return new object[] { rowTally, columnTally, shipTarget, null, typeof(ArgumentNullException) };
            yield return new object[] { rowTally, columnTally, shipTarget, grid, null };
        }

        [Theory]
        [InlineData(3, 3, typeof(ArgumentOutOfRangeException))]
        [InlineData(4, 2, typeof(ArgumentOutOfRangeException))]
        [InlineData(4, 3, null)]
        [InlineData(5, 3, typeof(ArgumentOutOfRangeException))]
        [InlineData(4, 4, typeof(ArgumentOutOfRangeException))]
        public void TestTallyGridMismatch(int rowTallyLength, int columnTallyLength, Type expectedExceptionType)
        {
            var rowTally = new GridTally(rowTallyLength);
            var columnTally = new GridTally(columnTallyLength);
            var shipTarget = new ShipTarget();
            var grid = new BimaruGrid(4, 3);

            var caughtException = Record.Exception(() => new BimaruGame(rowTally, columnTally, shipTarget, grid));

            Assert.Equal(expectedExceptionType, caughtException?.GetType());
        }

        [Fact]
        public void TestNumberOfMissingShipFields()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(4, 3);

            game.TargetNumberOfShipFieldsPerRow[0] = 1;
            game.TargetNumberOfShipFieldsPerRow[1] = 1;
            game.TargetNumberOfShipFieldsPerRow[3] = 1;

            game.TargetNumberOfShipFieldsPerColumn[0] = 3;

            game.TargetNumberOfShipsPerLength[1] = 1;
            game.TargetNumberOfShipsPerLength[2] = 1;

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_UP;
            game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_SINGLE;
            game.Grid[new GridPoint(3, 0)] = BimaruValue.WATER;

            // 1xDESTROYER, 1xSUBMARINE
            //   300
            //   ---
            // 1|W??
            // 0|???
            // 1|???
            // 1|S?S

            Assert.Equal(-1, game.NumberOfMissingShipFieldsPerRow(0));
            Assert.Equal(1, game.NumberOfMissingShipFieldsPerRow(1));
            Assert.Equal(0, game.NumberOfMissingShipFieldsPerRow(2));
            Assert.Equal(1, game.NumberOfMissingShipFieldsPerRow(3));

            Assert.Equal(2, game.NumberOfMissingShipFieldsPerColumn(0));
            Assert.Equal(0, game.NumberOfMissingShipFieldsPerColumn(1));
            Assert.Equal(-1, game.NumberOfMissingShipFieldsPerColumn(2));

            Assert.Equal(2, game.LengthOfLongestMissingShip);

            game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_CONT_DOWN;

            Assert.Null(game.LengthOfLongestMissingShip);
        }

        [Fact]
        public void TestUnsolvabilityEmptyGame()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(4, 3);

            Assert.False(game.IsUnsolvable);
        }

        [Fact]
        public void TestUnsolvability()
        {
            var game = GetNotUnsolvableGame();

            Assert.False(game.IsUnsolvable);
        }

        /// <summary>
        /// 
        /// 1xDESTROYER, 1xCRUISER
        ///   403
        ///   ---
        /// 3|???
        /// 0|???
        /// 1|???
        /// 3|???
        /// 
        /// </summary>
        private static BimaruGame GetNotUnsolvableGame()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(4, 3);

            game.TargetNumberOfShipFieldsPerRow[0] = 3;
            game.TargetNumberOfShipFieldsPerRow[1] = 1;
            game.TargetNumberOfShipFieldsPerRow[3] = 3;

            game.TargetNumberOfShipFieldsPerColumn[0] = 4;
            game.TargetNumberOfShipFieldsPerColumn[2] = 3;

            game.TargetNumberOfShipsPerLength[2] = 2;
            game.TargetNumberOfShipsPerLength[3] = 1;

            return game;
        }

        [Fact]
        public void TestUnsolvabilityTallyTotal()
        {
            var game = GetNotUnsolvableGame();

            game.TargetNumberOfShipFieldsPerRow[2] = 1;

            // Target number of ship fields per row request more
            // than the target number of ship fields per column
            Assert.True(game.IsUnsolvable);
        }

        [Fact]
        public void TestUnsolvabilityTargetShips()
        {
            var game = GetNotUnsolvableGame();

            game.TargetNumberOfShipsPerLength[1] = 1;

            // Target number of ship fields per row request more
            // ship fields than the target number of ships per length
            Assert.True(game.IsUnsolvable);
        }

        [Fact]
        public void TestUnsolvabilityMoreShipFieldsThanRows()
        {
            var game = GetNotUnsolvableGame();

            game.TargetNumberOfShipFieldsPerRow[2]++;
            game.TargetNumberOfShipFieldsPerColumn[0]++;
            game.TargetNumberOfShipsPerLength[1]++;

            // Higher target number of ship fields in column 0 than possible
            Assert.True(game.IsUnsolvable);
        }

        [Fact]
        public void TestUnsolvabilityMoreShipFieldsThanColumns()
        {
            var game = GetNotUnsolvableGame();

            game.TargetNumberOfShipFieldsPerRow[0]++;
            game.TargetNumberOfShipFieldsPerColumn[1]++;
            game.TargetNumberOfShipsPerLength[1]++;

            // Higher target number of ship fields in row 0 than possible
            Assert.True(game.IsUnsolvable);
        }

        [Fact]
        public void TestUnsolvabilityTooLongShipTarget()
        {
            var game = GetNotUnsolvableGame();

            game.TargetNumberOfShipsPerLength[2]--;
            game.TargetNumberOfShipsPerLength[3]--;
            game.TargetNumberOfShipsPerLength[5]++;

            // Longest target ship length does not fit in grid
            Assert.True(game.IsUnsolvable);
        }

        [Fact]
        public void TestValidityEmptyGame()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(2, 3);

            Assert.True(game.IsValid);
        }

        [Fact]
        public void TestValidityOfValidGame()
        {
            var game = GetValidGame();

            Assert.True(game.IsValid);
        }

        /// <summary>
        /// 
        /// 1xDESTROYER
        ///   101
        ///   ---
        /// 0|???
        /// 2|???
        /// 
        /// </summary>
        private static BimaruGame GetValidGame()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(2, 3);

            game.TargetNumberOfShipFieldsPerRow[0] = 2;

            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[2] = 1;

            game.TargetNumberOfShipsPerLength[2] = 1;

            return game;
        }

        [Fact]
        public void TestValidityWhenUnsolvable()
        {
            var game = GetValidGame();

            game.TargetNumberOfShipFieldsPerRow[0] = 3;

            // IsUnsolvable => Not valid
            Assert.False(game.IsValid);
        }

        [Fact]
        public void TestValidityTooManyShipFieldsColumn()
        {
            var game = GetValidGame();

            game.Grid[new GridPoint(0, 1)] = BimaruValue.SHIP_CONT_RIGHT;

            // Column 1 has more ship fields than targeted
            Assert.False(game.IsValid);
        }

        [Fact]
        public void TestValidityTooManyShipFieldsRow()
        {
            var game = GetValidGame();

            game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;
            game.Grid[new GridPoint(0, 2)] = BimaruValue.WATER;

            // Row 0 can not fulfill the ship field target
            Assert.False(game.IsValid);
        }

        [Fact]
        public void TestValidityWrongShipLengths()
        {
            var game = GetValidGame();

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_SINGLE;

            // No single ships are requested
            Assert.False(game.IsValid);
        }

        [Fact]
        public void TestValidityGridInvalid()
        {
            var game = GetValidGame();

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_RIGHT;
            game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;

            // Water is not allowed to be right to a ship
            // field that continues to the right
            Assert.False(game.IsValid);
        }

        [Fact]
        public void TestIsSolvedEmptyGame()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(4, 3);

            Assert.False(game.IsSolved);
        }

        [Fact]
        public void TestIsSolvedWhenSolved()
        {
            var game = GetSolvedGame();

            Assert.True(game.IsSolved);
        }

        /// <summary>
        /// 
        /// 1xSUBMARINE, 1xDESTROYER, 1xBATTLESHIP
        ///   304
        ///   ---
        /// 2|SWS
        /// 1|WWS
        /// 2|SWS
        /// 2|SWS
        /// 
        /// </summary>
        private static BimaruGame GetSolvedGame()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(4, 3);

            game.TargetNumberOfShipFieldsPerRow[0] = 2;
            game.TargetNumberOfShipFieldsPerRow[1] = 2;
            game.TargetNumberOfShipFieldsPerRow[2] = 1;
            game.TargetNumberOfShipFieldsPerRow[3] = 2;

            game.TargetNumberOfShipFieldsPerColumn[0] = 3;
            game.TargetNumberOfShipFieldsPerColumn[2] = 4;

            game.TargetNumberOfShipsPerLength[1] = 1;
            game.TargetNumberOfShipsPerLength[2] = 1;
            game.TargetNumberOfShipsPerLength[4] = 1;

            foreach (var p in game.Grid.AllPoints())
            {
                game.Grid[p] = BimaruValue.WATER;
            }

            game.Grid[new GridPoint(3, 0)] = BimaruValue.SHIP_SINGLE;

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_UP;
            game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_CONT_DOWN;

            game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_CONT_UP;
            game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_MIDDLE;
            game.Grid[new GridPoint(2, 2)] = BimaruValue.SHIP_MIDDLE;
            game.Grid[new GridPoint(3, 2)] = BimaruValue.SHIP_CONT_DOWN;

            return game;
        }

        [Fact]
        public void TestIsSolvedNotFullyDetermined()
        {
            var game = GetSolvedGame();

            game.Grid[new GridPoint(0, 1)] = BimaruValue.UNDETERMINED;

            // Targets satisfied but not fully determined
            Assert.False(game.IsSolved);
        }

        [Fact]
        public void TestIsSolvedTallyViolated()
        {
            var game = GetSolvedGame();

            game.Grid[new GridPoint(3, 0)] = BimaruValue.SHIP_CONT_DOWN;
            game.Grid[new GridPoint(2, 0)] = BimaruValue.SHIP_CONT_UP;
            game.Grid[new GridPoint(1, 0)] = BimaruValue.WATER;
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;

            // Row 1 and 2 don't fulfill the target number of ship fields
            Assert.False(game.IsSolved);
        }

        [Fact]
        public void TestIsSolvedWrongShips()
        {
            var game = GetSolvedGame();

            game.TargetNumberOfShipsPerLength[1] = 3;
            game.TargetNumberOfShipsPerLength[2] = 0;

            // Targets are satisfied but with wrong ship lengths
            Assert.False(game.IsSolved);
        }

        [Fact]
        public void TestIsSolvedGridInvalid()
        {
            var game = GetSolvedGame();

            game.TargetNumberOfShipsPerLength[1] = 3;
            game.TargetNumberOfShipsPerLength[2] = 0;

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_SINGLE;

            // Grid is not valid
            Assert.False(game.IsSolved);
        }
    }
}
