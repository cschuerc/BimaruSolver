using Bimaru.Interface.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bimaru.Database.Entities;

public class GameEntityConfiguration : IEntityTypeConfiguration<GameEntity>
{
    private readonly IDerivedValueGenerator<GameEntity, int> uniqueIdGenerator;
    private readonly IDerivedValueGenerator<GameEntity, GameSize> sizeGenerator;
    private readonly IDerivedValueGenerator<GameEntity, GameDifficulty> difficultyGenerator;

    public GameEntityConfiguration(
        IDerivedValueGenerator<GameEntity, int> uniqueIdGenerator,
        IDerivedValueGenerator<GameEntity, GameSize> sizeGenerator,
        IDerivedValueGenerator<GameEntity, GameDifficulty> difficultyGenerator)
    {
        this.uniqueIdGenerator = uniqueIdGenerator;
        this.sizeGenerator = sizeGenerator;
        this.difficultyGenerator = difficultyGenerator;
    }

    public void Configure(EntityTypeBuilder<GameEntity> builder)
    {
        builder
            .HasKey(g => g.Id);

        builder
            .Property(g => g.TargetNumberOfShipFieldsPerRow)
            .IsRequired();

        builder
            .Property(g => g.TargetNumberOfShipFieldsPerColumn)
            .IsRequired();

        builder
            .Property(g => g.TargetNumberOfShipsPerLength)
            .IsRequired();

        builder
            .Property(g => g.AlmostUniqueGameId)
            .HasValueGenerator((_, _)
                => new DerivedValueGenerator<GameEntity, int>(uniqueIdGenerator));

        builder
            .Property(g => g.Size)
            .HasValueGenerator((_, _)
                => new DerivedValueGenerator<GameEntity, GameSize>(sizeGenerator));

        builder
            .Property(g => g.Difficulty)
            .HasValueGenerator((_, _)
                => new DerivedValueGenerator<GameEntity, GameDifficulty>(difficultyGenerator));

        builder
            .HasIndex(g => g.AlmostUniqueGameId)
            .IsUnique();
    }
}