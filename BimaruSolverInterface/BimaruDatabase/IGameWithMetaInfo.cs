namespace BimaruInterfaces
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
