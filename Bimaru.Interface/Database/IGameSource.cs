using System.Collections.Generic;

namespace Bimaru.Interface.Database
{
    public interface IGameSource
    {
        IEnumerable<IGameMetaInfo> GetMetaInfoOfAllGames();

        IGameWithMetaInfo GetGame(int id);
    }
}
