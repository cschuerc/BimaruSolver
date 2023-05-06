using System;
using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface.Database;

namespace Bimaru.Database
{
    public class GameDatabase : IGameDatabase
    {
        public GameDatabase(IGameSource gameSource)
        {
            GameSource = gameSource;
            Thumbnails = new List<GameMetaInfo>(GameSource.GetMetaInfoOfAllGames());
            RandomNumberGenerator = new Random();
        }

        private IGameSource gameSource;

        private IGameSource GameSource
        {
            get => gameSource;

            set => gameSource = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Thumbnails (only meta info) of all games in the database
        /// </summary>
        private List<GameMetaInfo> Thumbnails { get; }

        private Random RandomNumberGenerator { get; }

        public IEnumerable<GameMetaInfo> GetMetaInfoOfGames()
        {
            return Thumbnails;
        }

        public GameWithMetaInfo GetRandomGame(Func<GameMetaInfo, bool> filter)
        {
            var filteredGames = Thumbnails.Where(filter).ToList();

            if (!filteredGames.Any())
            {
                return null;
            }

            var thumbnail = filteredGames.ElementAt(RandomNumberGenerator.Next(filteredGames.Count));

            return GameSource.GetGame(thumbnail.Id);
        }

        public GameWithMetaInfo GetGameById(int id)
        {
            return GameSource.GetGame(id);
        }
    }
}
