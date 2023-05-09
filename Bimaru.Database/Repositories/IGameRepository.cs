using System.Threading.Tasks;
using Bimaru.Database.Entities;

namespace Bimaru.Database.Repositories;

public interface IGameRepository
{
    Task<GameEntity> GetGameAsync(int gameId);

    Task<GameEntity?> GetRandomGameAsync();

    void AddGame(GameEntity game);

    Task<int> SaveChangesAsync();
}