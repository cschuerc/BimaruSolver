using Bimaru.Interfaces;

namespace Bimaru.DatabaseUtil
{
    public static class DatabaseFactory
    {
        public static IGameDatabase GetDatabase()
        {
            var gameSource = new GameSourceFromResources();
            return new Database(gameSource);
        }
    }
}
