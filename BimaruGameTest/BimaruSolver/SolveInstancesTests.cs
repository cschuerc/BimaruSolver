using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Utility;

namespace BimaruSolver
{
    [TestClass]
    public class SolveInstancesTests
    {
        private static Game SetupGame(int numRows, int numColumns)
        {
            var rowTally = new Tally(numRows);
            Tally columnTally = new Tally(numColumns);
            ShipSettings settings = new ShipSettings();
            Grid initialGrid = new Grid(numRows, numColumns);
            RollbackGrid grid = new RollbackGrid(initialGrid);

            return new Game(rowTally, columnTally, settings, grid);
        }

        private static ISolver SetupSolver()
        {
            var fieldChangedRules = new List<IFieldChangedRule>()
            {   new SetShipEnvironment(),
                new FillRowOrColumnWithWater(),
                new FillRowOrColumnWithShips()
            };
            var fullGridRules = new List<IFullGridRule>()
            {   new FillRowOrColumnWithWater(),
                new FillRowOrColumnWithShips()
            };

            var trialAndErrorRule = new TrialLongestMissingShip();

            return new Solver(fieldChangedRules, fullGridRules, trialAndErrorRule);
        }

        private static Game SetupGameSmall()
        {
            Game game = SetupGame(6, 6);

            game.ShipSettings[1] = 3;
            game.ShipSettings[2] = 2;
            game.ShipSettings[3] = 1;

            game.RowTally[0] = 2;
            game.RowTally[1] = 1;
            game.RowTally[2] = 3;
            game.RowTally[3] = 1;
            game.RowTally[4] = 1;
            game.RowTally[5] = 2;

            game.ColumnTally[0] = 2;
            game.ColumnTally[1] = 1;
            game.ColumnTally[2] = 2;
            game.ColumnTally[3] = 2;
            game.ColumnTally[4] = 0;
            game.ColumnTally[5] = 3;

            game.Grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.SHIP_CONT_RIGHT);

            game.Grid.SetFieldValue(new GridPoint(2, 2), BimaruValue.WATER);

            game.Grid.SetFieldValue(new GridPoint(5, 0), BimaruValue.WATER);
            game.Grid.SetFieldValue(new GridPoint(5, 5), BimaruValue.WATER);

            return game;
        }

        private static Game SetupGameLarge()
        {
            Game game = SetupGame(10, 10);

            game.ShipSettings[1] = 4;
            game.ShipSettings[2] = 3;
            game.ShipSettings[3] = 2;
            game.ShipSettings[4] = 1;

            game.RowTally[0] = 2;
            game.RowTally[1] = 1;
            game.RowTally[2] = 4;
            game.RowTally[3] = 2;
            game.RowTally[4] = 1;
            game.RowTally[5] = 2;
            game.RowTally[6] = 2;
            game.RowTally[7] = 1;
            game.RowTally[8] = 4;
            game.RowTally[9] = 1;

            game.ColumnTally[0] = 3;
            game.ColumnTally[1] = 2;
            game.ColumnTally[2] = 2;
            game.ColumnTally[3] = 2;
            game.ColumnTally[4] = 2;
            game.ColumnTally[5] = 1;
            game.ColumnTally[6] = 1;
            game.ColumnTally[7] = 3;
            game.ColumnTally[8] = 3;
            game.ColumnTally[9] = 1;

            game.Grid.SetFieldValue(new GridPoint(7, 0), BimaruValue.SHIP_CONT_DOWN);
            game.Grid.SetFieldValue(new GridPoint(0, 3), BimaruValue.SHIP_CONT_RIGHT);
            game.Grid.SetFieldValue(new GridPoint(0, 6), BimaruValue.WATER);
            game.Grid.SetFieldValue(new GridPoint(3, 6), BimaruValue.WATER);

            return game;
        }

        [TestMethod]
        public void TestGames()
        {
            var solver = SetupSolver();

            var gameSmall = SetupGameSmall();
            solver.Solve(gameSmall);
            Assert.IsTrue(gameSmall.IsSolved);

            var gameLarge = SetupGameLarge();
            solver.Solve(gameLarge);
            Assert.IsTrue(gameLarge.IsSolved);
        }
    }
}
