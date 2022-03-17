using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class PageGroupItemMap : IEntityTypeConfiguration<PageGroupItem>
{
    public void Configure(EntityTypeBuilder<PageGroupItem> builder)
    {
        builder.ToTable(nameof(PageGroupItem), DbConstants.CofoundrySchema);
        builder.HasKey(s => new { s.PageId, s.PageGroupId });

        builder.Property(s => s.PageId)
            .ValueGeneratedNever();

        builder.Property(s => s.PageGroupId)
            .ValueGeneratedNever();

        builder.HasOne(s => s.PageGroup)
            .WithMany(s => s.PageGroupItems)
            .HasForeignKey(d => d.PageGroupId);

        builder.HasOne(s => s.Page)
            .WithMany(s => s.PageGroupItems)
            .HasForeignKey(d => d.PageId);

        CreateAuditableMappingHelper.Map(builder);
    }
}
