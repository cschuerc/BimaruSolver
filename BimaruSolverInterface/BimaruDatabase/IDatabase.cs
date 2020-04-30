using System;
using System.Collections.Generic;

namespace BimaruInterfaces
{
    /// <summary>
    /// Database for Bimaru games
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Get the game with the specified ID
        /// </summary>
        /// <param name="ID"> Id of the desired game </param>
        /// <returns> The game or null if no such game </returns>
        IDatabaseGame GetSpecificGame(int ID);

        /// <summary>
        /// Get a random game among all games satisfying the given filter
        /// </summary>
        /// <param name="filter"> Filter to satisfy </param>
        /// <returns> Random game or null if no such game </returns>
        IDatabaseGame GetRandomGame(Func<IGameMetaInfo, bool> filter);

        /// <summary>
        /// Get all games satisfying the given filter
        /// </summary>
        /// <param name="filter"> Filter to satisfy </param>
        /// <returns> Enumerable of all such games </returns>
        IEnumerable<IDatabaseGame> GetAllGames(Func<IGameMetaInfo, bool> filter);
    }
}
