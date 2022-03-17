using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class DocumentAssetGroupItemMap : IEntityTypeConfiguration<DocumentAssetGroupItem>
{
    public void Configure(EntityTypeBuilder<DocumentAssetGroupItem> builder)
    {
        builder.ToTable(nameof(DocumentAssetGroupItem), DbConstants.CofoundrySchema);
        builder.HasKey(s => new { s.DocumentAssetId, s.DocumentAssetGroupId });

        builder
            .Property(s => s.DocumentAssetId)
            .ValueGeneratedNever();

        builder
            .Property(s => s.DocumentAssetGroupId)
            .ValueGeneratedNever();

        builder.HasOne(s => s.DocumentAssetGroup)
            .WithMany(s => s.DocumentAssetGroupItems)
            .HasForeignKey(d => d.DocumentAssetGroupId);

        builder.HasOne(s => s.DocumentAsset)
            .WithMany(s => s.DocumentAssetGroupItems)
            .HasForeignKey(d => d.DocumentAssetId);

        CreateAuditableMappingHelper.Map(builder);
    }
}
