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
    /// Bimaru games from embedded resources
    /// </summary>
    public class GameSourceFromResources : IGameSource
    {
        /// <param name="serializer"> Serializer used to deserialize the resources </param>
        public GameSourceFromResources(IFormatter serializer)
        {
            Serializer = serializer;
            ResourceNamesPerId = GetResourceNamesPerId();
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

        private Dictionary<int, string> ResourceNamesPerId
        {
            get;
            set;
        }

        private Dictionary<int, string> GetResourceNamesPerId()
        {
            var resourceNamesPerId = new Dictionary<int, string>();

            var assembly = Assembly.GetExecutingAssembly();
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                var game = (IGameWithMetaInfo)LoadResource(resourceName);

                resourceNamesPerId[game.MetaInfo.ID] = resourceName;
            }

            return resourceNamesPerId;
        }

        private object LoadResource(string resourceName)
        {
            object resource = null;

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                resource = Serializer.Deserialize(stream);
            }

            return resource;
        }

        public IEnumerable<IGameMetaInfo> GetMetaInfoOfAllGames()
        {
            return ResourceNamesPerId.Keys.Select(id => GetGame(id).MetaInfo);
        }

        public IGameWithMetaInfo GetGame(int ID)
        {
            if (!ResourceNamesPerId.ContainsKey(ID))
            {
                throw new ArgumentOutOfRangeException("No game with given ID.");
            }

            return (IGameWithMetaInfo)LoadResource(ResourceNamesPerId[ID]);
        }
    }
}
