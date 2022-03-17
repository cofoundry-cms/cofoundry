using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class PageTagMap : IEntityTypeConfiguration<PageTag>
{
    public void Configure(EntityTypeBuilder<PageTag> builder)
    {
        builder.ToTable(nameof(PageTag), DbConstants.CofoundrySchema);
        builder.HasKey(s => new { s.PageId, s.TagId });

        builder.Property(s => s.PageId).ValueGeneratedNever();
        builder.Property(s => s.TagId).ValueGeneratedNever();

        builder.HasOne(s => s.Page)
            .WithMany(s => s.PageTags)
            .HasForeignKey(d => d.PageId);

        builder.HasOne(s => s.Tag)
            .WithMany()
            .HasForeignKey(d => d.TagId);

        CreateAuditableMappingHelper.Map(builder);
    }
}
