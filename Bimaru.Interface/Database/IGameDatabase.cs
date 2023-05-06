using System;
using System.Collections.Generic;

namespace Bimaru.Interface.Database
{
    public interface IGameDatabase
    {
        /// <summary>
        /// Get the meta info of all games in the database.
        /// </summary>
        /// <returns></returns>
        IEnumerable<GameMetaInfo> GetMetaInfoOfGames();

        /// <summary>
        /// Get a random game from the filtered database.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        GameWithMetaInfo GetRandomGame(Func<GameMetaInfo, bool> filter);

        /// <summary>
        /// Get the game by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        GameWithMetaInfo GetGameById(int id);
    }
}
