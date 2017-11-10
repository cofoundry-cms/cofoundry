using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class PageTemplateMap : IEntityTypeConfiguration<PageTemplate>
    {
        public void Create(EntityTypeBuilder<PageTemplate> builder)
        {
            builder.ToTable("PageTemplate", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.FileName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.FullPath)
                .HasMaxLength(400)
                .IsRequired();

            builder.Property(s => s.CustomEntityDefinitionCode)
                .HasMaxLength(6);

            builder.Property(s => s.CustomEntityModelType)
                .HasMaxLength(100);

            builder.Property(s => s.Description)
                .IsNVarCharMaxType();

            // Relationships
            builder.HasOne(s => s.CustomEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.CustomEntityDefinitionCode);
        }
    }
}
