using BimaruInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace BimaruDatabase
{
    /// <summary>
    /// Bimaru game database of the embedded resources
    /// </summary>
    public class Database : IDatabase
    {
        /// <summary>
        /// Constructor for a database.
        /// </summary>
        /// <param name="serializer"> Serializer used to deserialize the resources </param>
        public Database(IFormatter serializer)
        {
            Serializer = serializer;
            RandomGenerator = new Random();

            LoadThumbnails();
        }

        private IFormatter _serializer;

        /// <summary>
        /// Serializer used to deserialize the resources
        /// </summary>
        protected IFormatter Serializer
        {
            get
            {
                return _serializer;
            }

            private set
            {
                _serializer = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Random generator
        /// </summary>
        protected Random RandomGenerator { get; private set; }

        /// <summary>
        /// Thumbnail of a game in the database
        /// </summary>
        protected class Thumbnail
        {
            /// <summary>
            /// Constructs a thumbnail
            /// </summary>
            /// <param name="metaInfo"> Meta game info</param>
            /// <param name="resourceName"> Name of resource where the game is found </param>
            public Thumbnail(IGameMetaInfo metaInfo, string resourceName)
            {
                MetaInfo = metaInfo;

                ResourceName = resourceName;
            }

            /// <summary>
            /// Meta game info
            /// </summary>
            public IGameMetaInfo MetaInfo { get; private set; }

            /// <summary>
            /// Name of the resource
            /// </summary>
            public string ResourceName { get; private set; }
        }

        /// <summary>
        /// Thumbnails of all games in the database
        /// </summary>
        protected List<Thumbnail> Thumbnails { get; private set; }

        /// <summary>
        /// Load thumbnails for all games
        /// </summary>
        protected void LoadThumbnails()
        {
            Thumbnails = new List<Thumbnail>();

            var assembly = Assembly.GetExecutingAssembly();
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    var game = (IDatabaseGame)Serializer.Deserialize(stream);
                    Thumbnails.Add(new Thumbnail(game.MetaInfo, resourceName));
                }
            }
        }

        /// <summary>
        /// Load the game of the given resource
        /// </summary>
        /// <param name="resourceName"> Name of the resource </param>
        /// <returns> Game from the resource </returns>
        protected IDatabaseGame LoadGame(string resourceName)
        {
            IDatabaseGame game = null;

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                game = (IDatabaseGame)Serializer.Deserialize(stream);
            }

            return game;
        }

        /// <inheritdoc/>
        public IEnumerable<IDatabaseGame> GetAllGames(Func<IGameMetaInfo, bool> filter)
        {
            foreach (var t in Thumbnails.Where(t => filter == null || filter(t.MetaInfo)))
            {
                yield return LoadGame(t.ResourceName);
            }
        }

        /// <inheritdoc/>
        public IDatabaseGame GetRandomGame(Func<IGameMetaInfo, bool> filter)
        {
            var filteredGames = Thumbnails.Where(t => filter == null || filter(t.MetaInfo));

            if (filteredGames.Count() == 0)
            {
                return null;
            }

            var thumbnail = filteredGames.ElementAt(RandomGenerator.Next(filteredGames.Count()));

            return LoadGame(thumbnail.ResourceName);
        }

        /// <inheritdoc/>
        public IDatabaseGame GetSpecificGame(int ID)
        {
            var thumbnail = Thumbnails.FirstOrDefault(t => t.MetaInfo.ID == ID);

            if (thumbnail == null)
            {
                return null;
            }

            return LoadGame(thumbnail.ResourceName);
        }
    }
}
