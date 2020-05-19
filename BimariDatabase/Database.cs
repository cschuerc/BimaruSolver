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
            get
            {
                return gameSource;
            }

            set
            {
                gameSource = value ?? throw new ArgumentNullException("No game source.");
            }
        }

        /// <summary>
        /// Thumbnails (only meta info) of all games in the database
        /// </summary>
        private List<IGameMetaInfo> Thumbnails
        {
            get;
            set;
        }

        private Random RandomNumberGenerator
        {
            get;
            set;
        }

        public IEnumerable<IGameWithMetaInfo> GetAllGames(Func<IGameMetaInfo, bool> filter)
        {
            foreach (var t in Thumbnails.Where(t => filter == null || filter(t)))
            {
                yield return GameSource.GetGame(t.ID);
            }
        }

        public IGameWithMetaInfo GetRandomGame(Func<IGameMetaInfo, bool> filter)
        {
            var filteredGames = Thumbnails.Where(t => filter == null || filter(t));

            if (filteredGames.Count() == 0)
            {
                return null;
            }

            var thumbnail = filteredGames.ElementAt(RandomNumberGenerator.Next(filteredGames.Count()));

            return GameSource.GetGame(thumbnail.ID);
        }

        public IGameWithMetaInfo GetSpecificGame(int ID)
        {
            return GameSource.GetGame(ID);
        }
    }
}
