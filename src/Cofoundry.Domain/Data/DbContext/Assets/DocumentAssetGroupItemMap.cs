using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cofoundry.Domain.Data
{
    public class DocumentAssetGroupItemMap : IEntityTypeConfiguration<DocumentAssetGroupItem>
    {
        public void Configure(EntityTypeBuilder<DocumentAssetGroupItem> builder)
        {
            builder.ToTable("DocumentAssetGroupItem", DbConstants.CofoundrySchema);

            // Primary Key
            builder.HasKey(s =>new { s.DocumentAssetId, s.DocumentAssetGroupId });

            // Properties
            builder
                .Property(s => s.DocumentAssetId)
                .ValueGeneratedNever();

            builder
                .Property(s => s.DocumentAssetGroupId)
                .ValueGeneratedNever();

            // Relationships
            builder.HasOne(s => s.DocumentAssetGroup)
                .WithMany(s => s.DocumentAssetGroupItems)
                .HasForeignKey(d => d.DocumentAssetGroupId);

            builder.HasOne(s => s.DocumentAsset)
                .WithMany(s => s.DocumentAssetGroupItems)
                .HasForeignKey(d => d.DocumentAssetId);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
