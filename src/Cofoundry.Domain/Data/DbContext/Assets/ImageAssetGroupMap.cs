using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cofoundry.Domain.Data
{
    public class ImageAssetGroupMap : IEntityTypeConfiguration<ImageAssetGroup>
    {
        public void Configure(EntityTypeBuilder<ImageAssetGroup> builder)
        {
            builder.ToTable("ImageAssetGroup", DbConstants.CofoundrySchema);

            // Primary Key
            builder.HasKey(s => s.ImageAssetGroupId);

            // Properties
            builder.Property(s => s.GroupName)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships
            builder.HasOne(s => s.ParentImageAssetGroup)
                .WithMany(s => s.ChildImageAssetGroups)
                .HasForeignKey(d => d.ParentImageAssetGroupId);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
