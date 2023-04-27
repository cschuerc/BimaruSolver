namespace Bimaru.Interfaces
{
    public interface IGameFactory
    {
        IBimaruGame GenerateEmptyGame(int numRows, int numColumns);
    }
}
