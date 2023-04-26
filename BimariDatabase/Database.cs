using Bimaru.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bimaru.DatabaseUtil
{
    public class Database : IGameDatabase
    {
        public Database(IGameSource gameSource)
        {
            GameSource = gameSource;
            Thumbnails = new List<IGameMetaInfo>(GameSource.GetMetaInfoOfAllGames());
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
        private List<IGameMetaInfo> Thumbnails
        {
            get;
        }

        private Random RandomNumberGenerator
        {
            get;
        }

        public IEnumerable<IGameWithMetaInfo> GetAllGames(Func<IGameMetaInfo, bool> filter)
        {
            return Thumbnails.Where(t => filter == null || filter(t)).Select(t => GameSource.GetGame(t.Id));
        }

        public IGameWithMetaInfo GetRandomGame(Func<IGameMetaInfo, bool> filter)
        {
            var filteredGames = Thumbnails.Where(t => filter == null || filter(t)).ToList();

            if (!filteredGames.Any())
            {
                return null;
            }

            var thumbnail = filteredGames.ElementAt(RandomNumberGenerator.Next(filteredGames.Count));

            return GameSource.GetGame(thumbnail.Id);
        }

        public IGameWithMetaInfo GetSpecificGame(int id)
        {
            return GameSource.GetGame(id);
        }
    }
}
