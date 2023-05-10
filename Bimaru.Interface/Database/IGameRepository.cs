namespace Bimaru.Interface.Database
{

    public interface IGameRepository
    {
        Task<GameEntity?> GetGameAsync(int gameId);

        Task<GameEntity?> GetRandomGameAsync(GameSize? size, GameDifficulty? difficulty);

        void AddGame(GameEntity game);

        Task<int> SaveChangesAsync();
    }
}