using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cofoundry.Domain.Data
{
    public class DocumentAssetTagMap : IEntityTypeConfiguration<DocumentAssetTag>
    {
        public void Configure(EntityTypeBuilder<DocumentAssetTag> builder)
        {
            builder.ToTable("DocumentAssetTag", DbConstants.CofoundrySchema);

            // Primary Key
            builder.HasKey(s =>new { s.DocumentAssetId, s.TagId });

            // Properties
            builder.Property(s => s.DocumentAssetId)
                .ValueGeneratedNever();

            builder.Property(s => s.TagId)
                .ValueGeneratedNever();

            // Relationships
            builder.HasOne(s => s.DocumentAsset)
                .WithMany(s => s.DocumentAssetTags)
                .HasForeignKey(d => d.DocumentAssetId);

            builder.HasOne(s => s.Tag)
                .WithMany()
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.Restrict);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
