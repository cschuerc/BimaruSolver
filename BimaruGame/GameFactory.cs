using BimaruInterfaces;

namespace BimaruGame
{
    /// <summary>
    /// Generates standard Bimaru games
    /// </summary>
    public class GameFactory : IGameFactory
    {
        /// <inheritdoc/>
        public IGame GenerateEmptyGame(int numRows, int numColumns)
        {
            return GenerateGame(numRows, numColumns);
        }

        #region Testing
        /// <summary>
        /// Generates a Bimaru game without a solution
        /// </summary>
        /// <returns></returns>
        public Game GenerateGameNoSolution()
        {
            var game = GenerateGame(2, 2);

            game.RowTally[0] = 1;
            game.RowTally[1] = 1;

            game.ColumnTally[0] = 1;
            game.ColumnTally[1] = 1;

            game.ShipSettings[1] = 2;

            // 2xSUBMARINE
            //   11
            //   --
            // 1|
            // 1|
            // -> No solution

            return game;
        }

        /// <summary>
        /// Generates a Bimaru game with one solution
        /// </summary>
        /// <returns></returns>
        public Game GenerateGameOneSolution()
        {
            var game = GenerateGame(2, 2);

            game.RowTally[0] = 1;

            game.ColumnTally[0] = 1;

            game.ShipSettings[1] = 1;

            // 1xSUBMARINE
            //   01
            //   --
            // 0|
            // 1|

            return game;
        }

        /// <summary>
        /// Generates a Bimaru game with two solutions
        /// </summary>
        /// <returns></returns>
        public Game GenerateGameTwoSolutions()
        {
            var game = GenerateGame(2, 3);

            game.RowTally[0] = 1;
            game.RowTally[1] = 1;

            game.ColumnTally[0] = 1;
            game.ColumnTally[2] = 1;

            game.ShipSettings[1] = 2;

            // 2xSUBMARINE
            //   101
            //   ---
            // 1|
            // 1|
            // => Two solutions

            return game;
        }

        /// <summary>
        /// Generates an empty game.
        /// </summary>
        /// <param name="numRows"> Number of rows </param>
        /// <param name="numColumns"> Number of columns </param>
        /// <returns></returns>
        public Game GenerateGame(int numRows, int numColumns)
        {
            var rowTally = new Tally(numRows);
            var columnTally = new Tally(numColumns);
            var shipSettings = new ShipSettings();
            var grid = new RollbackGrid(numRows, numColumns);

            return new Game(rowTally, columnTally, shipSettings, grid);
        }
        #endregion
    }
}
