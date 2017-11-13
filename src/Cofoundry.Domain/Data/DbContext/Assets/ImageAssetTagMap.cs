using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cofoundry.Domain.Data
{
    public class ImageAssetTagMap : IEntityTypeConfiguration<ImageAssetTag>
    {
        public void Configure(EntityTypeBuilder<ImageAssetTag> builder)
        {
            builder.ToTable("ImageAssetTag", DbConstants.CofoundrySchema);

            // Primary Key
            builder.HasKey(s =>new { s.ImageAssetId, s.TagId });

            // Properties
            builder.Property(s => s.ImageAssetId)
                .ValueGeneratedNever();

            builder.Property(s => s.TagId)
                .ValueGeneratedNever();


            // Relationships
            builder.HasOne(s => s.ImageAsset)
                .WithMany(s => s.ImageAssetTags)
                .HasForeignKey(d => d.ImageAssetId);

            builder.HasOne(s => s.Tag)
                .WithMany()
                .HasForeignKey(d => d.TagId);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
