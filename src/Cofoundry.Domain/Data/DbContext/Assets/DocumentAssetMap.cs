using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class DocumentAssetMap : EntityTypeConfiguration<DocumentAsset>
    {
        public DocumentAssetMap()
        {
            ToTable("DocumentAsset", DbConstants.CofoundrySchema);

            // Properties
            Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(100);

            Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(100);

            Property(t => t.Description)
                .IsRequired();

            Property(t => t.FileExtension)
                .IsRequired()
                .HasMaxLength(5);

            Property(t => t.ContentType)
                .HasMaxLength(100);

            UpdateAuditableMappingHelper.Map(this);
        }
    }
}
