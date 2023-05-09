using Bimaru.Interface.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bimaru.Database.DbContexts;

public class GridValueEntityConfiguration : IEntityTypeConfiguration<GridValueEntity>
{
    public void Configure(EntityTypeBuilder<GridValueEntity> builder)
    {
        builder.HasKey(g => new { g.GameId, g.RowIndex, g.ColumnIndex });

        builder.Property(g => g.RowIndex)
            .ValueGeneratedNever();

        builder.Property(g => g.ColumnIndex)
            .ValueGeneratedNever();

        builder.HasOne(v => v.Game)
            .WithMany(g => g.GridValues)
            .HasForeignKey(v => v.GameId);
    }
}