using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

[Obsolete("The document asset grouping system will be revised in an upcomming release.")]
public class DocumentAssetGroupMap : IEntityTypeConfiguration<DocumentAssetGroup>
{
    public void Configure(EntityTypeBuilder<DocumentAssetGroup> builder)
    {
        builder.ToTable(nameof(DocumentAssetGroup), DbConstants.CofoundrySchema);

        builder.Property(s => s.GroupName)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasOne(s => s.ParentDocumentAssetGroup)
            .WithMany(s => s.ChildDocumentAssetGroups)
            .HasForeignKey(d => d.ParentDocumentAssetGroupId);

        CreateAuditableMappingHelper.Map(builder);
    }
}
