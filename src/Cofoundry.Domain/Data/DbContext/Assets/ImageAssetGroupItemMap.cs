using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cofoundry.Domain.Data
{
    public class ImageAssetGroupItemMap : IEntityTypeConfiguration<ImageAssetGroupItem>
    {
        public void Create(EntityTypeBuilder<ImageAssetGroupItem> builder)
        {
            builder.ToTable("ImageAssetGroupItem", DbConstants.CofoundrySchema);

            // Primary Key
            builder.HasKey(s =>new { s.ImageAssetId, s.ImageAssetGroupId });

            // Properties
            builder.Property(s => s.ImageAssetId)
                .ValueGeneratedNever();

            builder.Property(s => s.ImageAssetGroupId)
                .ValueGeneratedNever();

            // Relationships
            builder.HasOne(s => s.ImageAssetGroup)
                .WithMany(s => s.ImageAssetGroupItems)
                .HasForeignKey(d => d.ImageAssetGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.ImageAsset)
                .WithMany(s => s.ImageAssetGroupItems)
                .HasForeignKey(d => d.ImageAssetId)
                .OnDelete(DeleteBehavior.Restrict);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
