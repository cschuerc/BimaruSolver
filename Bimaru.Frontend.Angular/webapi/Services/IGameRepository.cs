using webapi.Entities;

namespace webapi.Services;

public interface IGameRepository
{
    Task<GameEntity?> GetGameAsync(int gameId);

    Task<GameEntity?> GetRandomGameAsync();

    void AddGame(GameEntity game);

    Task<int> SaveChangesAsync();
}