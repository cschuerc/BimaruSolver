using BimaruInterfaces;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BimaruDatabaseGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            string filenameFormat = args[0];
            var games = DatabaseGenerator.GenerateGames();
            BinaryFormatter serializer = new BinaryFormatter();

            SerializeGamesToFile(filenameFormat, games, serializer);
        }

        private static void SerializeGamesToFile(
            string databaseNameFormat,
            IEnumerable<IGameWithMetaInfo> games,
            IFormatter serializer)
        {
            foreach (var game in games)
            {
                using (Stream fileStream = File.Create(string.Format(databaseNameFormat, game.MetaInfo.ID)))
                {
                    serializer.Serialize(fileStream, game);
                }
            }
        }
    }
}
