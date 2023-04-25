﻿using Bimaru.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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
            set;
        }

        private Dictionary<int, string> GetResourceNamesPerId()
        {
            var resourceNamesPerId = new Dictionary<int, string>();

            var assembly = Assembly.GetExecutingAssembly();
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                var game = LoadResource(resourceName);

                resourceNamesPerId[game.MetaInfo.ID] = resourceName;
            }

            return resourceNamesPerId;
        }

        private IGameWithMetaInfo LoadResource(string resourceName)
        {
            IGameWithMetaInfo resource = null;

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                resource = JsonSerializer.Deserialize<GameWithMetaInfo>(stream);
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

            return LoadResource(ResourceNamesPerId[ID]);
        }
    }
}
