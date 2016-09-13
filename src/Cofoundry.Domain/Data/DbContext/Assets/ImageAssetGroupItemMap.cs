using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class ImageAssetGroupItemMap : EntityTypeConfiguration<ImageAssetGroupItem>
    {
        public ImageAssetGroupItemMap()
        {
            ToTable("ImageAssetGroupItem", DbConstants.CofoundrySchema);

            // Primary Key
            HasKey(t => new { t.ImageAssetId, t.ImageAssetGroupId });

            // Properties
            Property(t => t.ImageAssetId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ImageAssetGroupId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Relationships
            HasRequired(t => t.ImageAssetGroup)
                .WithMany(t => t.ImageAssetGroupItems)
                .HasForeignKey(d => d.ImageAssetGroupId);
            HasRequired(t => t.ImageAsset)
                .WithMany(t => t.ImageAssetGroupItems)
                .HasForeignKey(d => d.ImageAssetId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
