using Bimaru.Interfaces;

namespace Bimaru.DatabaseUtil
{
    public class DatabaseFactory
    {
        public IGameDatabase GetDatabase()
        {
            var gameSource = new GameSourceFromResources();
            return new Database(gameSource);
        }
    }
}
