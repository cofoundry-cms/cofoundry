using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Data
{
    public class PageVersionMap : IEntityTypeConfiguration<PageVersion>
    {
        public void Configure(EntityTypeBuilder<PageVersion> builder)
        {
            builder.ToTable("PageVersion", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(s => s.MetaDescription)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(s => s.OpenGraphTitle)
                .HasMaxLength(300);

            // Relationships

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
