namespace Bimaru.Interfaces
{
    public interface IGameFactory
    {
        IGame GenerateEmptyGame(int numRows, int numColumns);
    }
}
