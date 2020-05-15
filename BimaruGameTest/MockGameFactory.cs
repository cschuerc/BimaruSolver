using BimaruGame;

namespace BimaruTest
{
    public class MockGameFactory
    {
        public Game GenerateGameNoSolution()
        {
            var game = GenerateEmptyGame(2, 2);

            game.TargetNumberOfShipFieldsPerRow[0] = 1;
            game.TargetNumberOfShipFieldsPerRow[1] = 1;

            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[1] = 1;

            game.TargetNumberOfShipsPerLength[1] = 2;

            // 2xSUBMARINE
            //   11
            //   --
            // 1|
            // 1|
            // -> No solution

            return game;
        }

        public Game GenerateGameOneSolution()
        {
            var game = GenerateEmptyGame(2, 2);

            game.TargetNumberOfShipFieldsPerRow[0] = 1;

            game.TargetNumberOfShipFieldsPerColumn[0] = 1;

            game.TargetNumberOfShipsPerLength[1] = 1;

            // 1xSUBMARINE
            //   01
            //   --
            // 0|
            // 1|

            return game;
        }

        public Game GenerateGameTwoSolutions()
        {
            var game = GenerateEmptyGame(2, 3);

            game.TargetNumberOfShipFieldsPerRow[0] = 1;
            game.TargetNumberOfShipFieldsPerRow[1] = 1;

            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[2] = 1;

            game.TargetNumberOfShipsPerLength[1] = 2;

            // 2xSUBMARINE
            //   101
            //   ---
            // 1|
            // 1|
            // => Two solutions

            return game;
        }

        public Game GenerateEmptyGame(int numRows, int numColumns)
        {
            var rowTally = new GridTally(numRows);
            var columnTally = new GridTally(numColumns);
            var shipSettings = new ShipTarget();
            var grid = new BimaruGrid(numRows, numColumns);

            return new Game(rowTally, columnTally, shipSettings, grid);
        }
    }
}
