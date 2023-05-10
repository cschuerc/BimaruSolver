using Bimaru.Game;
using Bimaru.Interface.Utility;

namespace Bimaru.Tests
{
    public static class GameFactoryForTesting
    {
        public static BimaruGame GenerateGameNoSolution()
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

        public static BimaruGame GenerateGameOneSolution()
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

        public static BimaruGame GenerateGameTwoSolutions()
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

        public static BimaruGame GenerateInvalidGame()
        {
            var game = GenerateEmptyGame(2, 3);

            game.TargetNumberOfShipFieldsPerRow[0] = 1;
            game.TargetNumberOfShipFieldsPerRow[1] = 2;

            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[2] = 1;

            game.TargetNumberOfShipsPerLength[1] = 2;

            // 2xSUBMARINE
            //   101
            //   ---
            // 2|
            // 1|
            // => Invalid

            return game;
        }

        public static BimaruGame GenerateSolvedGame()
        {
            var game = GenerateEmptyGame(2, 3);

            game.TargetNumberOfShipFieldsPerRow[0] = 0;
            game.TargetNumberOfShipFieldsPerRow[1] = 2;

            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[2] = 1;

            game.TargetNumberOfShipsPerLength[1] = 2;

            game.Grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;
            game.Grid[new GridPoint(0, 2)] = BimaruValue.WATER;
            game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_SINGLE;
            game.Grid[new GridPoint(1, 1)] = BimaruValue.WATER;
            game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_SINGLE;

            // 2xSUBMARINE
            //   101
            //   ---
            // 2|SWS
            // 0|WWW
            // => Solved

            return game;
        }

        public static BimaruGame GenerateEmptyGame(int numRows, int numColumns)
        {
            var rowTally = new GridTally(numRows);
            var columnTally = new GridTally(numColumns);
            var shipSettings = new ShipTarget();
            var grid = new BimaruGrid(numRows, numColumns);

            return new BimaruGame(rowTally, columnTally, shipSettings, grid);
        }
    }
}
