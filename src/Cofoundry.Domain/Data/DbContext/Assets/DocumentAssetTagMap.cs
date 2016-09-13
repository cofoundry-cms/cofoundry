using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class DocumentAssetTagMap : EntityTypeConfiguration<DocumentAssetTag>
    {
        public DocumentAssetTagMap()
        {
            ToTable("DocumentAssetTag", DbConstants.CofoundrySchema);

            // Primary Key
            HasKey(t => new { t.DocumentAssetId, t.TagId });

            // Properties
            Property(t => t.DocumentAssetId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.TagId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Relationships
            HasRequired(t => t.DocumentAsset)
                .WithMany(t => t.DocumentAssetTags)
                .HasForeignKey(d => d.DocumentAssetId);
            HasRequired(t => t.Tag)
                .WithMany()
                .HasForeignKey(d => d.TagId); 
            
            CreateAuditableMappingHelper.Map(this);
        }
    }
}
