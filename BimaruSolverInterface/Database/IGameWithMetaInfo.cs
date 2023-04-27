namespace Bimaru.Interfaces
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
