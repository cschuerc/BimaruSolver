namespace Bimaru.Interface.Database
{

    public interface IGameRepository
    {
        Task<GameEntity?> GetGameAsync(int gameId);

        Task<GameEntity?> GetRandomGameAsync();

        void AddGame(GameEntity game);

        Task<int> SaveChangesAsync();
    }
}