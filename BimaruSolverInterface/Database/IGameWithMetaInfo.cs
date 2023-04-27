namespace Bimaru.Interfaces
{
    public interface IGameWithMetaInfo
    {
        IGameMetaInfo MetaInfo
        {
            get;
        }

        IGame Game
        {
            get;
        }
    }
}
