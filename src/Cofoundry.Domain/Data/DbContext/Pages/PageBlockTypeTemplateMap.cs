
using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Data
{
    public class PageBlockTypeTemplateMap : IEntityTypeConfiguration<PageBlockTypeTemplate>
    {
        public void Create(EntityTypeBuilder<PageBlockTypeTemplate> builder)
        {
            builder.ToTable("PageBlockTypeTemplate", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.FileName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Description)
                .IsNVarCharMaxType();

            // Relationships

            builder.HasOne(s => s.PageBlockType)
                .WithMany(s => s.PageBlockTemplates)
                .HasForeignKey(d => d.PageBlockTypeId);
        }
    }
}
