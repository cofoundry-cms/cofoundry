using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

[Obsolete("The image asset grouping system will be revised in an upcomming release.")]
public class ImageAssetGroupMap : IEntityTypeConfiguration<ImageAssetGroup>
{
    public void Configure(EntityTypeBuilder<ImageAssetGroup> builder)
    {
        builder.ToTable(nameof(ImageAssetGroup), DbConstants.CofoundrySchema);
        builder.HasKey(s => s.ImageAssetGroupId);

        builder.Property(s => s.GroupName)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasOne(s => s.ParentImageAssetGroup)
            .WithMany(s => s.ChildImageAssetGroups)
            .HasForeignKey(d => d.ParentImageAssetGroupId);

        CreateAuditableMappingHelper.Map(builder);
    }
}
