using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class ImageAssetGroupItemMap : IEntityTypeConfiguration<ImageAssetGroupItem>
{
    public void Configure(EntityTypeBuilder<ImageAssetGroupItem> builder)
    {
        builder.ToTable(nameof(ImageAssetGroupItem), DbConstants.CofoundrySchema);
        builder.HasKey(s => new { s.ImageAssetId, s.ImageAssetGroupId });

        builder.Property(s => s.ImageAssetId).ValueGeneratedNever();

        builder.Property(s => s.ImageAssetGroupId).ValueGeneratedNever();

        builder.HasOne(s => s.ImageAssetGroup)
            .WithMany(s => s.ImageAssetGroupItems)
            .HasForeignKey(d => d.ImageAssetGroupId);

        builder.HasOne(s => s.ImageAsset)
            .WithMany(s => s.ImageAssetGroupItems)
            .HasForeignKey(d => d.ImageAssetId);

        CreateAuditableMappingHelper.Map(builder);
    }
}
