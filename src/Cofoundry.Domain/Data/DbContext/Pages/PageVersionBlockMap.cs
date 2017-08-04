using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cofoundry.Domain.Data
{
    public class PageVersionBlockMap : IEntityTypeConfiguration<PageVersionBlock>
    {
        public void Create(EntityTypeBuilder<PageVersionBlock> builder)
        {
            builder.ToTable("PageVersionBlock", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.SerializedData)
                .IsRequired()
                .IsNVarCharMaxType();

            // Relationships

            builder.HasOne(s => s.PageTemplateRegion)
                .WithMany(s => s.PageVersionBlockss)
                .HasForeignKey(d => d.PageTemplateRegionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PageBlockType)
                .WithMany(s => s.PageVersionBlocks)
                .HasForeignKey(d => d.PageBlockTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PageVersion)
                .WithMany(s => s.PageVersionBlocks)
                .HasForeignKey(d => d.PageVersionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PageBlockTypeTemplate)
                .WithMany()
                .HasForeignKey(s => s.PageBlockTypeTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
