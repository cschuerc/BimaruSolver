using Bimaru.Interfaces;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bimaru.DatabaseUtil
{
    public class DatabaseFactory
    {
        public IGameDatabase GetDatabase()
        {
            var serializer = new BinaryFormatter();
            var gameSource = new GameSourceFromResources(serializer);
            return new Database(gameSource);
        }
    }
}
