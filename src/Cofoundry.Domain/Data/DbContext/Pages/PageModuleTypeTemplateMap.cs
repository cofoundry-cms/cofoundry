
using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Data
{
    public class ModuleTemplateMap : IEntityTypeConfiguration<PageModuleTypeTemplate>
    {
        public void Create(EntityTypeBuilder<PageModuleTypeTemplate> builder)
        {
            builder.ToTable("PageModuleTypeTemplate", DbConstants.CofoundrySchema);

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

            builder.HasOne(s => s.PageModuleType)
                .WithMany(s => s.PageModuleTemplates)
                .HasForeignKey(d => d.PageModuleTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
