using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class DocumentAssetGroupMap : EntityTypeConfiguration<DocumentAssetGroup>
    {
        public DocumentAssetGroupMap()
        {
            ToTable("DocumentAssetGroup", DbConstants.CofoundrySchema);

            // Properties
            Property(t => t.GroupName)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships
            HasOptional(t => t.ParentDocumentAssetGroup)
                .WithMany(t => t.ChildDocumentAssetGroups)
                .HasForeignKey(d => d.ParentDocumentAssetGroupId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
