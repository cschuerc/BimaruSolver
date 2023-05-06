using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webapi.Entities;

namespace webapi.DbContexts;

public class GameEntityConfiguration : IEntityTypeConfiguration<GameEntity>
{
    public void Configure(EntityTypeBuilder<GameEntity> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.TargetNumberOfShipFieldsPerRow)
            .IsRequired();

        builder.Property(g => g.TargetNumberOfShipFieldsPerColumn)
            .IsRequired();

        builder.Property(g => g.TargetNumberOfShipsPerLength)
            .IsRequired();
    }
}