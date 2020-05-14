using BimaruInterfaces;

namespace BimaruGame
{
    public class GameFactory : IGameFactory
    {
        public IGame GenerateEmptyGame(int numRows, int numColumns)
        {
            var rowTally = new GridTally(numRows);
            var columnTally = new GridTally(numColumns);
            var shipSettings = new ShipTarget();
            var grid = new BimaruGrid(numRows, numColumns);

            return new Game(rowTally, columnTally, shipSettings, grid);
        }
    }
}
