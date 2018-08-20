using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain.Data
{
    public class PageTemplateMap : IEntityTypeConfiguration<PageTemplate>
    {
        public void Configure(EntityTypeBuilder<PageTemplate> builder)
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
                .HasMaxLength(400);

            builder.Property(s => s.Description)
                .IsNVarCharMaxType();

            // Relationships
            builder.HasOne(s => s.CustomEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.CustomEntityDefinitionCode);
        }
    }
}
