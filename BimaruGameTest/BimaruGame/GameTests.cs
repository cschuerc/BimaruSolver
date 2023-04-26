using System;
using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;
// ReSharper disable AccessToModifiedClosure

namespace Bimaru.Test
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void TestNullArguments()
        {
            const int numRows = 4;
            const int numColumns = 3;

            var rowTally = new GridTally(numRows);
            var columnTally = new GridTally(numColumns);
            var shipTarget = new ShipTarget();
            var grid = new BimaruGrid(numRows, numColumns);

            Assert.ThrowsException<ArgumentNullException>(() => new Game(null, columnTally, shipTarget, grid));
            Assert.ThrowsException<ArgumentNullException>(() => new Game(rowTally, null, shipTarget, grid));
            Assert.ThrowsException<ArgumentNullException>(() => new Game(rowTally, columnTally, null, grid));
            Assert.ThrowsException<ArgumentNullException>(() => new Game(rowTally, columnTally, shipTarget, null));
        }

        [TestMethod]
        public void TestRowTallyGridMismatch()
        {
            var columnTally = new GridTally(3);
            var shipTarget = new ShipTarget();
            var grid = new BimaruGrid(4, 3);

            var rowTally = new GridTally(3);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Game(rowTally, columnTally, shipTarget, grid));

            rowTally = new GridTally(4);
            var _ = new Game(rowTally, columnTally, shipTarget, grid);

            rowTally = new GridTally(5);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Game(rowTally, columnTally, shipTarget, grid));
        }

        [TestMethod]
        public void TestColumnTallyGridMismatch()
        {
            var rowTally = new GridTally(4);
            var shipTarget = new ShipTarget();
            var grid = new BimaruGrid(4, 3);

            var columnTally = new GridTally(2);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Game(rowTally, columnTally, shipTarget, grid));

            columnTally = new GridTally(3);
            var _ = new Game(rowTally, columnTally, shipTarget, grid);

            columnTally = new GridTally(4);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Game(rowTally, columnTally, shipTarget, grid));
        }

        [TestMethod]
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

            Assert.AreEqual(-1, game.NumberOfMissingShipFieldsPerRow(0));
            Assert.AreEqual(1, game.NumberOfMissingShipFieldsPerRow(1));
            Assert.AreEqual(0, game.NumberOfMissingShipFieldsPerRow(2));
            Assert.AreEqual(1, game.NumberOfMissingShipFieldsPerRow(3));

            Assert.AreEqual(2, game.NumberOfMissingShipFieldsPerColumn(0));
            Assert.AreEqual(0, game.NumberOfMissingShipFieldsPerColumn(1));
            Assert.AreEqual(-1, game.NumberOfMissingShipFieldsPerColumn(2));

            Assert.AreEqual(2, game.LengthOfLongestMissingShip);

            game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_CONT_DOWN;

            Assert.IsNull(game.LengthOfLongestMissingShip);
        }

        [TestMethod]
        public void TestUnsolvabilityEmptyGame()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(4, 3);

            Assert.IsFalse(game.IsUnsolvable);
        }

        [TestMethod]
        public void TestUnsolvability()
        {
            var game = GetNotUnsolvableGame();

            Assert.IsFalse(game.IsUnsolvable);
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
        private static Game GetNotUnsolvableGame()
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

        [TestMethod]
        public void TestUnsolvabilityTallyTotal()
        {
            var game = GetNotUnsolvableGame();

            game.TargetNumberOfShipFieldsPerRow[2] = 1;

            // Target number of ship fields per row request more
            // than the target number of ship fields per column
            Assert.IsTrue(game.IsUnsolvable);
        }

        [TestMethod]
        public void TestUnsolvabilityTargetShips()
        {
            var game = GetNotUnsolvableGame();

            game.TargetNumberOfShipsPerLength[1] = 1;

            // Target number of ship fields per row request more
            // ship fields than the target number of ships per length
            Assert.IsTrue(game.IsUnsolvable);
        }

        [TestMethod]
        public void TestUnsolvabilityMoreShipFieldsThanRows()
        {
            var game = GetNotUnsolvableGame();

            game.TargetNumberOfShipFieldsPerRow[2]++;
            game.TargetNumberOfShipFieldsPerColumn[0]++;
            game.TargetNumberOfShipsPerLength[1]++;

            // Higher target number of ship fields in column 0 than possible
            Assert.IsTrue(game.IsUnsolvable);
        }

        [TestMethod]
        public void TestUnsolvabilityMoreShipFieldsThanColumns()
        {
            var game = GetNotUnsolvableGame();

            game.TargetNumberOfShipFieldsPerRow[0]++;
            game.TargetNumberOfShipFieldsPerColumn[1]++;
            game.TargetNumberOfShipsPerLength[1]++;

            // Higher target number of ship fields in row 0 than possible
            Assert.IsTrue(game.IsUnsolvable);
        }

        [TestMethod]
        public void TestUnsolvabilityTooLongShipTarget()
        {
            var game = GetNotUnsolvableGame();

            game.TargetNumberOfShipsPerLength[2]--;
            game.TargetNumberOfShipsPerLength[3]--;
            game.TargetNumberOfShipsPerLength[5]++;

            // Longest target ship length does not fit in grid
            Assert.IsTrue(game.IsUnsolvable);
        }

        [TestMethod]
        public void TestValidityEmptyGame()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(2, 3);

            Assert.IsTrue(game.IsValid);
        }

        [TestMethod]
        public void TestValidityOfValidGame()
        {
            var game = GetValidGame();

            Assert.IsTrue(game.IsValid);
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
        private static Game GetValidGame()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(2, 3);

            game.TargetNumberOfShipFieldsPerRow[0] = 2;

            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[2] = 1;

            game.TargetNumberOfShipsPerLength[2] = 1;

            return game;
        }

        [TestMethod]
        public void TestValidityWhenUnsolvable()
        {
            var game = GetValidGame();

            game.TargetNumberOfShipFieldsPerRow[0] = 3;

            // IsUnsolvable => Not valid
            Assert.IsFalse(game.IsValid);
        }

        [TestMethod]
        public void TestValidityTooManyShipFieldsColumn()
        {
            var game = GetValidGame();

            game.Grid[new GridPoint(0, 1)] = BimaruValue.SHIP_CONT_RIGHT;

            // Column 1 has more ship fields than targeted
            Assert.IsFalse(game.IsValid);
        }

        [TestMethod]
        public void TestValidityTooManyShipFieldsRow()
        {
            var game = GetValidGame();

            game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;
            game.Grid[new GridPoint(0, 2)] = BimaruValue.WATER;

            // Row 0 can not fulfill the ship field target
            Assert.IsFalse(game.IsValid);
        }

        [TestMethod]
        public void TestValidityWrongShipLengths()
        {
            var game = GetValidGame();

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_SINGLE;

            // No single ships are requested
            Assert.IsFalse(game.IsValid);
        }

        [TestMethod]
        public void TestValidityGridInvalid()
        {
            var game = GetValidGame();

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_RIGHT;
            game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;

            // Water is not allowed to be right to a ship
            // field that continues to the right
            Assert.IsFalse(game.IsValid);
        }

        [TestMethod]
        public void TestIsSolvedEmptyGame()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(4, 3);

            Assert.IsFalse(game.IsSolved);
        }

        [TestMethod]
        public void TestIsSolvedWhenSolved()
        {
            var game = GetSolvedGame();

            Assert.IsTrue(game.IsSolved);
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
        private static Game GetSolvedGame()
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

        [TestMethod]
        public void TestIsSolvedNotFullyDetermined()
        {
            var game = GetSolvedGame();

            game.Grid[new GridPoint(0, 1)] = BimaruValue.UNDETERMINED;

            // Targets satisfied but not fully determined
            Assert.IsFalse(game.IsSolved);
        }

        [TestMethod]
        public void TestIsSolvedTallyViolated()
        {
            var game = GetSolvedGame();

            game.Grid[new GridPoint(3, 0)] = BimaruValue.SHIP_CONT_DOWN;
            game.Grid[new GridPoint(2, 0)] = BimaruValue.SHIP_CONT_UP;
            game.Grid[new GridPoint(1, 0)] = BimaruValue.WATER;
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;

            // Row 1 and 2 don't fulfill the target number of ship fields
            Assert.IsFalse(game.IsSolved);
        }

        [TestMethod]
        public void TestIsSolvedWrongShips()
        {
            var game = GetSolvedGame();

            game.TargetNumberOfShipsPerLength[1] = 3;
            game.TargetNumberOfShipsPerLength[2] = 0;

            // Targets are satisfied but with wrong ship lengths
            Assert.IsFalse(game.IsSolved);
        }

        [TestMethod]
        public void TestIsSolvedGridInvalid()
        {
            var game = GetSolvedGame();

            game.TargetNumberOfShipsPerLength[1] = 3;
            game.TargetNumberOfShipsPerLength[2] = 0;

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_SINGLE;

            // Grid is not valid
            Assert.IsFalse(game.IsSolved);
        }
    }
}
