using System.Collections.Generic;

namespace Bimaru.Interfaces
{
    public interface IGameSource
    {
        IEnumerable<IGameMetaInfo> GetMetaInfoOfAllGames();

        IGameWithMetaInfo GetGame(int id);
    }
}
