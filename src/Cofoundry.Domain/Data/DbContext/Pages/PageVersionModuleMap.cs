using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cofoundry.Domain.Data
{
    public class PageVersionModuleMap : IEntityTypeConfiguration<PageVersionModule>
    {
        public void Create(EntityTypeBuilder<PageVersionModule> builder)
        {
            builder.ToTable("PageVersionModule", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.SerializedData)
                .IsRequired()
                .IsNVarCharMaxType();

            // Relationships

            builder.HasOne(s => s.PageTemplateSection)
                .WithMany(s => s.PageVersionModules)
                .HasForeignKey(d => d.PageTemplateSectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PageModuleType)
                .WithMany(s => s.PageVersionModules)
                .HasForeignKey(d => d.PageModuleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PageVersion)
                .WithMany(s => s.PageVersionModules)
                .HasForeignKey(d => d.PageVersionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PageModuleTypeTemplate)
                .WithMany()
                .HasForeignKey(s => s.PageModuleTypeTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
