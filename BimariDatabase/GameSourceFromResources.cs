using Bimaru.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Bimaru.DatabaseUtil
{
    /// <summary>
    /// Bimaru games from embedded resources
    /// </summary>
    public class GameSourceFromResources : IGameSource
    {
        public GameSourceFromResources()
        {
            ResourceNamesPerId = GetResourceNamesPerId();
        }

        private Dictionary<int, string> ResourceNamesPerId
        {
            get;
        }

        private Dictionary<int, string> GetResourceNamesPerId()
        {
            var resourceNamesPerId = new Dictionary<int, string>();

            var assembly = Assembly.GetExecutingAssembly();
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                var game = LoadResource(resourceName);

                resourceNamesPerId[game.MetaInfo.Id] = resourceName;
            }

            return resourceNamesPerId;
        }

        private static IGameWithMetaInfo LoadResource(string resourceName)
        {
            IGameWithMetaInfo resource;

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                resource = JsonSerializer.Deserialize<GameWithMetaInfo>(stream ?? throw new InvalidOperationException());
            }

            return resource;
        }

        public IEnumerable<IGameMetaInfo> GetMetaInfoOfAllGames()
        {
            return ResourceNamesPerId.Keys.Select(id => GetGame(id).MetaInfo);
        }

        public IGameWithMetaInfo GetGame(int id)
        {
            if (!ResourceNamesPerId.ContainsKey(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return LoadResource(ResourceNamesPerId[id]);
        }
    }
}
