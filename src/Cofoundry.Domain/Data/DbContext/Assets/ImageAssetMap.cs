using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class ImageAssetMap : EntityTypeConfiguration<ImageAsset>
    {
        public ImageAssetMap()
        {
            ToTable("ImageAsset", DbConstants.CofoundrySchema);

            // Properties
            Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(128);

            Property(t => t.FileDescription)
                .IsRequired()
                .HasMaxLength(512);

            Property(t => t.Extension)
                .IsRequired()
                .HasMaxLength(5);

            // Table & Column Mappings
            Property(t => t.DefaultAnchorLocation).HasColumnName("ImageCropAnchorLocationId");


            UpdateAuditableMappingHelper.Map(this);
        }
    }
}
