using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class PageBlockTypeMap : IEntityTypeConfiguration<PageBlockType>
{
    public void Configure(EntityTypeBuilder<PageBlockType> builder)
    {
        builder.ToTable(nameof(PageBlockType), DbConstants.CofoundrySchema);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Description)
            .IsNVarCharMaxType();

        builder.Property(s => s.FileName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.CreateDate).IsUtc();
        builder.Property(s => s.UpdateDate).IsUtc();
    }
}
