using System;
using System.Collections.Generic;

namespace Bimaru.Interface.Database
{
    public interface IGameDatabase
    {
        /// <summary>
        /// Get all games satisfying the given filter
        /// </summary>
        IEnumerable<GameWithMetaInfo> GetAllGames(Func<GameMetaInfo, bool> filter);
    }
}
