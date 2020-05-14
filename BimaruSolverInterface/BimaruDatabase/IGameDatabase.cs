using System;
using System.Collections.Generic;

namespace BimaruInterfaces
{
    public interface IGameDatabase
    {
        IGameWithMetaInfo GetSpecificGame(int ID);

        /// <summary>
        /// Get a random game among all games satisfying the given filter.
        /// </summary>
        IGameWithMetaInfo GetRandomGame(Func<IGameMetaInfo, bool> filter);

        /// <summary>
        /// Get all games satisfying the given filter
        /// </summary>
        IEnumerable<IGameWithMetaInfo> GetAllGames(Func<IGameMetaInfo, bool> filter);
    }
}
