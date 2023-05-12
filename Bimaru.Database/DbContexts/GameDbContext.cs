using Bimaru.Interface.Database;
using Microsoft.EntityFrameworkCore;

namespace Bimaru.Database.DbContexts;

public class GameDbContext : DbContext
{
    private readonly IEntityTypeConfiguration<GameEntity> gameConfiguration;
    private readonly IEntityTypeConfiguration<GridValueEntity> gridValueConfiguration;

    public GameDbContext(
        DbContextOptions<GameDbContext> options,
        IEntityTypeConfiguration<GameEntity> gameConfiguration,
        IEntityTypeConfiguration<GridValueEntity> gridValueConfiguration)
        : base(options)
    {
        this.gameConfiguration = gameConfiguration;
        this.gridValueConfiguration = gridValueConfiguration;
    }

    public DbSet<GameEntity> Games { get; set; } = null!;

    public DbSet<GridValueEntity> GridValues { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(gameConfiguration);
        modelBuilder.ApplyConfiguration(gridValueConfiguration);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<int[]>()
            .HaveConversion<ArrayConverter<int>, ArrayComparer<int>>();
    }
}