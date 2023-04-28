using System.Collections.Generic;
using System.IO;
using Bimaru.Interface.Database;
using Newtonsoft.Json;

namespace Bimaru.Database.Generator
{
    internal static class GamesToFiles
    {
        private static readonly JsonSerializer jsonSerializer = new()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto
        };

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
                using var fileWriter = File.CreateText(string.Format(databaseNameFormat, game.MetaInfo.Id));
                jsonSerializer.Serialize(fileWriter, game);
            }
        }
    }
}
