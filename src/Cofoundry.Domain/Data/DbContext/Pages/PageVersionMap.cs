using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class PageVersionMap : IEntityTypeConfiguration<PageVersion>
    {
        public void Configure(EntityTypeBuilder<PageVersion> builder)
        {
            builder.ToTable(nameof(PageVersion), DbConstants.CofoundrySchema);

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(s => s.MetaDescription)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(s => s.OpenGraphTitle)
                .HasMaxLength(300);

            builder.HasOne(s => s.OpenGraphImageAsset)
                .WithMany()
                .HasForeignKey(d => d.OpenGraphImageId);

            builder.HasOne(s => s.PageTemplate)
                .WithMany(s => s.PageVersions)
                .HasForeignKey(d => d.PageTemplateId);

            builder.HasOne(s => s.Page)
                .WithMany(s => s.PageVersions)
                .HasForeignKey(d => d.PageId);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}