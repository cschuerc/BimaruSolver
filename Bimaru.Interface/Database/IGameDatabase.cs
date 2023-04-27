using System;
using System.Collections.Generic;

namespace Bimaru.Interface.Database
{
    public interface IGameDatabase
    {
        /// <summary>
        /// Get all games satisfying the given filter
        /// </summary>
        IEnumerable<IGameWithMetaInfo> GetAllGames(Func<IGameMetaInfo, bool> filter);
    }
}
