using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class PageTemplateRegionMap : IEntityTypeConfiguration<PageTemplateRegion>
{
    public void Configure(EntityTypeBuilder<PageTemplateRegion> builder)
    {
        builder.ToTable(nameof(PageTemplateRegion), DbConstants.CofoundrySchema);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.CreateDate).IsUtc();
        builder.Property(s => s.UpdateDate).IsUtc();

        builder.HasOne(s => s.PageTemplate)
            .WithMany(s => s.PageTemplateRegions)
            .HasForeignKey(d => d.PageTemplateId);
    }
}
