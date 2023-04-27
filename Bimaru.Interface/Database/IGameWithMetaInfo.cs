using Bimaru.Interface.Game;

namespace Bimaru.Interface.Database
{
    public interface IGameWithMetaInfo
    {
        IGameMetaInfo MetaInfo
        {
            get;
        }

        IBimaruGame Game
        {
            get;
        }
    }
}
