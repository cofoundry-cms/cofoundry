using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class ImageAssetMap : IEntityTypeConfiguration<ImageAsset>
    {
        public void Configure(EntityTypeBuilder<ImageAsset> builder)
        {
            builder.ToTable(nameof(ImageAsset), DbConstants.CofoundrySchema);

            builder.Property(s => s.FileName)
                .IsRequired()
                .HasMaxLength(130);

            builder.Property(s => s.FileNameOnDisk)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(130);

            builder.Property(s => s.FileExtension)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(s => s.VerificationToken)
                .IsRequired()
                .IsCharType(6);

            builder.Property(s => s.DefaultAnchorLocation).HasColumnName("ImageCropAnchorLocationId");

            builder.Property(s => s.FileUpdateDate).IsUtc();

            UpdateAuditableMappingHelper.Map(builder);
        }
    }
}