using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class PageTemplateSectionMap : IEntityTypeConfiguration<PageTemplateSection>
    {
        public void Create(EntityTypeBuilder<PageTemplateSection> builder)
        {
            builder.ToTable("PageTemplateSection", DbConstants.CofoundrySchema);

            // Properties
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Relationships

            builder.HasOne(s => s.PageTemplate)
                .WithMany(s => s.PageTemplateSections)
                .HasForeignKey(d => d.PageTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
