using System;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain.Data
{
    public class ImageAssetMap : IEntityTypeConfiguration<ImageAsset>
    {
        public void Configure(EntityTypeBuilder<ImageAsset> builder)
        {
            builder.ToTable("ImageAsset", DbConstants.CofoundrySchema);

            // Properties
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

            // Table & Column Mappings
            builder.Property(s => s.DefaultAnchorLocation).HasColumnName("ImageCropAnchorLocationId");


            UpdateAuditableMappingHelper.Map(builder);
        }
    }
}
