using Microsoft.EntityFrameworkCore;
using webapi.DbContexts;
using webapi.Entities;

namespace webapi.Services;

public class GameRepository : IGameRepository
{
    private readonly GameDbContext context;

    public GameRepository(GameDbContext context)
    {
        this.context = context;
    }

    public async Task<GameEntity?> GetGameAsync(int gameId)
    {
        return await context.Games
            .Include(g => g.GridValues)
            .Where(g => g.Id == gameId)
            .FirstOrDefaultAsync();
    }

    public async Task<GameEntity?> GetRandomGameAsync()
    {
        return await context.Games
            .Include(g => g.GridValues)
            .OrderBy(g => Guid.NewGuid())
            .FirstOrDefaultAsync();
    }

    public void AddGame(GameEntity game)
    {
        context.Games.Add(game);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}