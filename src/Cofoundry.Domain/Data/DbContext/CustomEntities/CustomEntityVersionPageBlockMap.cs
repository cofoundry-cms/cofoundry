using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityVersionPageBlockMap : IEntityTypeConfiguration<CustomEntityVersionPageBlock>
    {
        public void Create(EntityTypeBuilder<CustomEntityVersionPageBlock> builder)
        {
            builder.ToTable("CustomEntityVersionPageBlock", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.SerializedData)
                .IsRequired()
                .IsNVarCharMaxType();

            // Relationships

            builder.HasOne(s => s.CustomEntityVersion)
                .WithMany(d => d.CustomEntityVersionPageBlocks)
                .HasForeignKey(s => s.CustomEntityVersionId);

            builder.HasOne(s => s.PageBlockType)
                .WithMany(d => d.CustomEntityVersionPageBlocks)
                .HasForeignKey(s => s.PageBlockTypeId);

            builder.HasOne(s => s.PageTemplateRegion)
                .WithMany()
                .HasForeignKey(s => s.PageTemplateRegionId);

            builder.HasOne(s => s.PageBlockTypeTemplate)
                .WithMany()
                .HasForeignKey(s => s.PageBlockTypeTemplateId);
        }
    }
}
