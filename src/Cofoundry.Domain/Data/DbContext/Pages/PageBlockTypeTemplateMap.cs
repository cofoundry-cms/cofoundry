using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class PageBlockTypeTemplateMap : IEntityTypeConfiguration<PageBlockTypeTemplate>
    {
        public void Configure(EntityTypeBuilder<PageBlockTypeTemplate> builder)
        {
            builder.ToTable(nameof(PageBlockTypeTemplate), DbConstants.CofoundrySchema);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.FileName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Description)
                .IsNVarCharMaxType();

            builder.HasOne(s => s.PageBlockType)
                .WithMany(s => s.PageBlockTemplates)
                .HasForeignKey(d => d.PageBlockTypeId);

            builder.Property(s => s.CreateDate).IsUtc();
        }
    }
}