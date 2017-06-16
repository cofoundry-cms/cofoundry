using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityVersionPageModuleMap : IEntityTypeConfiguration<CustomEntityVersionPageModule>
    {
        public void Create(EntityTypeBuilder<CustomEntityVersionPageModule> builder)
        {
            builder.ToTable("CustomEntityVersionPageModule", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.SerializedData)
                .IsRequired()
                .IsNVarCharMaxType();

            // Relationships

            builder.HasOne(s => s.CustomEntityVersion)
                .WithMany(d => d.CustomEntityVersionPageModules)
                .HasForeignKey(s => s.CustomEntityVersionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PageModuleType)
                .WithMany(d => d.CustomEntityVersionPageModules)
                .HasForeignKey(s => s.PageModuleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PageTemplateSection)
                .WithMany()
                .HasForeignKey(s => s.PageTemplateSectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PageModuleTypeTemplate)
                .WithMany()
                .HasForeignKey(s => s.PageModuleTypeTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
