using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class AssetFileCleanupQueueItemMap : IEntityTypeConfiguration<AssetFileCleanupQueueItem>
    {
        public void Configure(EntityTypeBuilder<AssetFileCleanupQueueItem> builder)
        {
            builder.ToTable(nameof(AssetFileCleanupQueueItem), DbConstants.CofoundrySchema);

            builder.Property(s => s.FileNameOnDisk)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(50);

            builder.Property(s => s.FileExtension)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(s => s.AttemptPermittedDate).IsUtc();
            builder.Property(s => s.CompletedDate).IsUtc();
            builder.Property(s => s.CreateDate).IsUtc();
            builder.Property(s => s.LastAttemptDate).IsUtc();
        }
    }
}