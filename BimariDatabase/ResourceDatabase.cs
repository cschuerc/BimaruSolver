using Bimaru.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Bimaru.DatabaseUtil
{
    /// <summary>
    /// Bimaru game database containing the embedded resources
    /// </summary>
    public class ResourceDatabase : IGameDatabase
    {
        /// <param name="serializer"> Serializer used to deserialize the resources </param>
        public ResourceDatabase(IFormatter serializer)
        {
            Serializer = serializer;
            RandomNumberGenerator = new Random();

            LoadThumbnails();
        }

        private IFormatter serializer;

        private IFormatter Serializer
        {
            get
            {
                return serializer;
            }

            set
            {
                serializer = value ?? throw new ArgumentNullException();
            }
        }

        private Random RandomNumberGenerator
        {
            get;
            set;
        }

        /// <summary>
        /// Low-memory thumbnail of a game in the database
        /// </summary>
        private class Thumbnail
        {
            /// <param name="resourceFilename"> Filename of the embedded resource </param>
            public Thumbnail(IGameMetaInfo metaInfo, string resourceFilename)
            {
                MetaInfo = metaInfo;
                ResourceFilename = resourceFilename;
            }

            public IGameMetaInfo MetaInfo
            {
                get;
                private set;
            }

            public string ResourceFilename
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// Thumbnails of all games in the database
        /// </summary>
        private List<Thumbnail> Thumbnails
        {
            get;
            set;
        }

        private void LoadThumbnails()
        {
            Thumbnails = new List<Thumbnail>();

            var assembly = Assembly.GetExecutingAssembly();
            foreach (string resourceFilename in assembly.GetManifestResourceNames())
            {
                var game = LoadGame(resourceFilename);
                Thumbnails.Add(new Thumbnail(game.MetaInfo, resourceFilename));
            }
        }

        private IGameWithMetaInfo LoadGame(string resourceFilename)
        {
            IGameWithMetaInfo game = null;

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceFilename))
            {
                game = (IGameWithMetaInfo)Serializer.Deserialize(stream);
            }

            return game;
        }

        public IEnumerable<IGameWithMetaInfo> GetAllGames(Func<IGameMetaInfo, bool> filter)
        {
            foreach (var t in Thumbnails.Where(t => filter == null || filter(t.MetaInfo)))
            {
                yield return LoadGame(t.ResourceFilename);
            }
        }

        public IGameWithMetaInfo GetRandomGame(Func<IGameMetaInfo, bool> filter)
        {
            var filteredGames = Thumbnails.Where(t => filter == null || filter(t.MetaInfo));

            if (filteredGames.Count() == 0)
            {
                return null;
            }

            var thumbnail = filteredGames.ElementAt(RandomNumberGenerator.Next(filteredGames.Count()));

            return LoadGame(thumbnail.ResourceFilename);
        }

        public IGameWithMetaInfo GetSpecificGame(int ID)
        {
            var thumbnail = Thumbnails.FirstOrDefault(t => t.MetaInfo.ID == ID);

            if (thumbnail == null)
            {
                return null;
            }

            return LoadGame(thumbnail.ResourceFilename);
        }
    }
}
