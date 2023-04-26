using Bimaru.Interfaces;

namespace Bimaru.DatabaseUtil
{
    public class DatabaseFactory
    {
        public static IGameDatabase GetDatabase()
        {
            var gameSource = new GameSourceFromResources();
            return new Database(gameSource);
        }
    }
}
