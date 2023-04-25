using Bimaru.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Bimaru.DatabaseUtilGeneratorUtil
{
    class GamesToFiles
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            string filenameFormat = args[0];
            var games = DatabaseGenerator.GenerateGames();

            SerializeGamesToFile(filenameFormat, games);
        }

        private static void SerializeGamesToFile(
            string databaseNameFormat,
            IEnumerable<IGameWithMetaInfo> games)
        {
            foreach (var game in games)
            {
                using (Stream fileStream = File.Create(string.Format(databaseNameFormat, game.MetaInfo.ID)))
                {
                    JsonSerializer.Serialize(fileStream, game);
                }
            }
        }
    }
}
