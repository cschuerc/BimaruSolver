namespace BimaruInterfaces
{
    public interface IGameFactory
    {
        IGame GenerateEmptyGame(int numRows, int numColumns);
    }
}
