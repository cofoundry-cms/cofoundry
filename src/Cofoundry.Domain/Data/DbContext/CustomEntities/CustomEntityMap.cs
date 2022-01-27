using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityMap : IEntityTypeConfiguration<CustomEntity>
    {
        public void Configure(EntityTypeBuilder<CustomEntity> builder)
        {
            builder.ToTable(nameof(CustomEntity), DbConstants.CofoundrySchema);

            builder.Property(s => s.CustomEntityDefinitionCode)
                .IsRequired()
                .IsCharType(6);

            builder.Property(s => s.UrlSlug)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(s => s.PublishStatusCode)
                .HasMaxLength(1)
                .IsUnicode(false);

            builder.HasOne(s => s.CustomEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.CustomEntityDefinitionCode);

            builder.HasOne(s => s.Locale)
                .WithMany()
                .HasForeignKey(d => d.LocaleId);

            builder.Property(s => s.PublishDate).IsUtc();
            builder.Property(s => s.LastPublishDate).IsUtc();

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}