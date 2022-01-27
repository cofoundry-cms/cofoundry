using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class PageTemplateMap : IEntityTypeConfiguration<PageTemplate>
    {
        public void Configure(EntityTypeBuilder<PageTemplate> builder)
        {
            builder.ToTable(nameof(PageTemplate), DbConstants.CofoundrySchema);

            builder.Property(s => s.FileName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.FullPath)
                .HasMaxLength(400)
                .IsRequired();

            builder.Property(s => s.CustomEntityDefinitionCode).HasMaxLength(6);
            builder.Property(s => s.CustomEntityModelType).HasMaxLength(400);
            builder.Property(s => s.Description).IsNVarCharMaxType();

            builder.Property(s => s.CreateDate).IsUtc();
            builder.Property(s => s.UpdateDate).IsUtc();

            builder.HasOne(s => s.CustomEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.CustomEntityDefinitionCode);
        }
    }
}