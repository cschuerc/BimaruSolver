using Bimaru.Interfaces;

namespace Bimaru.Game
{
    public class GameFactory : IGameFactory
    {
        public IBimaruGame GenerateEmptyGame(int numRows, int numColumns)
        {
            var rowTally = new GridTally(numRows);
            var columnTally = new GridTally(numColumns);
            var shipSettings = new ShipTarget();
            var grid = new BimaruGrid(numRows, numColumns);

            return new BimaruGame(rowTally, columnTally, shipSettings, grid);
        }
    }
}
