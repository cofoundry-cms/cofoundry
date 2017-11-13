using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class PageTemplateRegionMap : IEntityTypeConfiguration<PageTemplateRegion>
    {
        public void Configure(EntityTypeBuilder<PageTemplateRegion> builder)
        {
            builder.ToTable("PageTemplateRegion", DbConstants.CofoundrySchema);

            // Properties
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Relationships

            builder.HasOne(s => s.PageTemplate)
                .WithMany(s => s.PageTemplateRegions)
                .HasForeignKey(d => d.PageTemplateId);
        }
    }
}
