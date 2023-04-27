namespace Bimaru.Interface.Game
{
    public interface IGameFactory
    {
        IBimaruGame GenerateEmptyGame(int numRows, int numColumns);
    }
}
