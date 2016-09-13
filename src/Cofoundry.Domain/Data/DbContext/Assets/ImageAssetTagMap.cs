using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class ImageAssetTagMap : EntityTypeConfiguration<ImageAssetTag>
    {
        public ImageAssetTagMap()
        {
            ToTable("ImageAssetTag", DbConstants.CofoundrySchema);

            // Primary Key
            HasKey(t => new { t.ImageAssetId, t.TagId });

            // Properties
            Property(t => t.ImageAssetId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.TagId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);


            // Relationships
            HasRequired(t => t.ImageAsset)
                .WithMany(t => t.ImageAssetTags)
                .HasForeignKey(d => d.ImageAssetId);
            HasRequired(t => t.Tag)
                .WithMany()
                .HasForeignKey(d => d.TagId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
