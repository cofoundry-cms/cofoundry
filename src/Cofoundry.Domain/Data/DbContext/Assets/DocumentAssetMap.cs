using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class DocumentAssetMap : IEntityTypeConfiguration<DocumentAsset>
{
    public void Configure(EntityTypeBuilder<DocumentAsset> builder)
    {
        builder.ToTable(nameof(DocumentAsset), DbConstants.CofoundrySchema, t =>
        {
            t.HasTrigger("Cofoundry.DocumentAsset_CascadeDelete");
        });

        builder.Property(s => s.FileName)
            .IsRequired()
            .HasMaxLength(130);

        builder.Property(s => s.FileNameOnDisk)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(50);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(130);

        builder.Property(s => s.Description)
            .IsRequired();

        builder.Property(s => s.FileExtension)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(s => s.VerificationToken)
            .IsRequired()
            .IsCharType(6);

        builder.Property(s => s.ContentType)
            .HasMaxLength(100);

        builder.Property(s => s.FileUpdateDate).IsUtc();

        UpdateAuditableMappingHelper.Map(builder);
    }
}
