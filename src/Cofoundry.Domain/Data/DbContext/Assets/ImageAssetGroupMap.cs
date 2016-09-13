using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class ImageAssetGroupMap : EntityTypeConfiguration<ImageAssetGroup>
    {
        public ImageAssetGroupMap()
        {
            ToTable("ImageAssetGroup", DbConstants.CofoundrySchema);

            // Primary Key
            HasKey(t => t.ImageAssetGroupId);

            // Properties
            Property(t => t.GroupName)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships
            HasOptional(t => t.ParentImageAssetGroup)
                .WithMany(t => t.ChildImageAssetGroups)
                .HasForeignKey(d => d.ParentImageAssetGroupId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
