using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Bimaru.Interface.Database;

namespace Bimaru.Database.Generator
{
    internal static class GamesToFiles
    {
        internal static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            var filenameFormat = args[0];
            var games = GameDatabaseGenerator.GenerateGames();

            SerializeGamesToFile(filenameFormat, games);
        }

        private static void SerializeGamesToFile(
            string databaseNameFormat,
            IEnumerable<GameWithMetaInfo> games)
        {
            foreach (var game in games)
            {
                using var fileStream = File.Create(string.Format(databaseNameFormat, game.MetaInfo.Id));
                JsonSerializer.Serialize(fileStream, game, new JsonSerializerOptions { WriteIndented = true });
            }
        }
    }
}
