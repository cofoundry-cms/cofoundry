using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cofoundry.Domain.Data
{
    public class AssetFileCleanupQueueItemMap : IEntityTypeConfiguration<AssetFileCleanupQueueItem>
    {
        public void Configure(EntityTypeBuilder<AssetFileCleanupQueueItem> builder)
        {
            builder.ToTable("AssetFileCleanupQueueItem", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.FileNameOnDisk)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(50);

            builder.Property(s => s.FileExtension)
                .IsRequired()
                .HasMaxLength(5);
        }
    }
}
