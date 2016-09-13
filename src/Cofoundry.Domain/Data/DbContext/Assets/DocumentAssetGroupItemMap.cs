using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class DocumentAssetGroupItemMap : EntityTypeConfiguration<DocumentAssetGroupItem>
    {
        public DocumentAssetGroupItemMap()
        {
            ToTable("DocumentAssetGroupItem", DbConstants.CofoundrySchema);

            // Primary Key
            HasKey(t => new { t.DocumentAssetId, t.DocumentAssetGroupId });

            // Properties
            Property(t => t.DocumentAssetId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.DocumentAssetGroupId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Relationships
            HasRequired(t => t.DocumentAssetGroup)
                .WithMany(t => t.DocumentAssetGroupItems)
                .HasForeignKey(d => d.DocumentAssetGroupId);
            HasRequired(t => t.DocumentAsset)
                .WithMany(t => t.DocumentAssetGroupItems)
                .HasForeignKey(d => d.DocumentAssetId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
