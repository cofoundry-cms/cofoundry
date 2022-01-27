using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class DocumentAssetTagMap : IEntityTypeConfiguration<DocumentAssetTag>
    {
        public void Configure(EntityTypeBuilder<DocumentAssetTag> builder)
        {
            builder.ToTable(nameof(DocumentAssetTag), DbConstants.CofoundrySchema);
            builder.HasKey(s => new { s.DocumentAssetId, s.TagId });

            builder.Property(s => s.DocumentAssetId)
                .ValueGeneratedNever();

            builder.Property(s => s.TagId)
                .ValueGeneratedNever();

            builder.HasOne(s => s.DocumentAsset)
                .WithMany(s => s.DocumentAssetTags)
                .HasForeignKey(d => d.DocumentAssetId);

            builder.HasOne(s => s.Tag)
                .WithMany()
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.Restrict);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}