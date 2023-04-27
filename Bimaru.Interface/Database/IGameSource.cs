using System.Collections.Generic;

namespace Bimaru.Interface.Database
{
    public interface IGameSource
    {
        IEnumerable<GameMetaInfo> GetMetaInfoOfAllGames();

        GameWithMetaInfo GetGame(int id);
    }
}
