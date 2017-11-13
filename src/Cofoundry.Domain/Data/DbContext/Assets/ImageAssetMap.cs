using System;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

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
                .HasMaxLength(128);

            builder.Property(s => s.FileDescription)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(s => s.Extension)
                .IsRequired()
                .HasMaxLength(5);

            // Table & Column Mappings
            builder.Property(s => s.DefaultAnchorLocation).HasColumnName("ImageCropAnchorLocationId");


            UpdateAuditableMappingHelper.Map(builder);
        }
    }
}
