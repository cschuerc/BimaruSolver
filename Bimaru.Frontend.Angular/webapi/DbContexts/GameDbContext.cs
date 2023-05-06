using Microsoft.EntityFrameworkCore;
using webapi.Entities;

namespace webapi.DbContexts;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    public DbSet<GameEntity> Games { get; set; } = null!;

    public DbSet<GridValueEntity> GridValues { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new GameEntityConfiguration());
        modelBuilder.ApplyConfiguration(new GridValueEntityConfiguration());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<int[]>()
            .HaveConversion<ArrayConverter<int>, ArrayComparer<int>>();
    }
}