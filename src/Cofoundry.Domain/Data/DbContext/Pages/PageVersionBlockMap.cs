using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class PageVersionBlockMap : IEntityTypeConfiguration<PageVersionBlock>
    {
        public void Configure(EntityTypeBuilder<PageVersionBlock> builder)
        {
            builder.ToTable(nameof(PageVersionBlock), DbConstants.CofoundrySchema);

            builder.Property(s => s.SerializedData)
                .IsRequired()
                .IsNVarCharMaxType();

            builder.HasOne(s => s.PageTemplateRegion)
                .WithMany(s => s.PageVersionBlocks)
                .HasForeignKey(d => d.PageTemplateRegionId);

            builder.HasOne(s => s.PageBlockType)
                .WithMany(s => s.PageVersionBlocks)
                .HasForeignKey(d => d.PageBlockTypeId);

            builder.HasOne(s => s.PageVersion)
                .WithMany(s => s.PageVersionBlocks)
                .HasForeignKey(d => d.PageVersionId);

            builder.HasOne(s => s.PageBlockTypeTemplate)
                .WithMany()
                .HasForeignKey(s => s.PageBlockTypeTemplateId);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}