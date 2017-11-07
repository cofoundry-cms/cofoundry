using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class PagePublishStatusQueryMap : IEntityTypeConfiguration<PagePublishStatusQuery>
    {
        public void Create(EntityTypeBuilder<PagePublishStatusQuery> builder)
        {
            builder.ToTable("PagePublishStatusQuery", DbConstants.CofoundrySchema);

            builder.HasKey(s => new { s.PageId, s.PublishStatusQueryId });

            // Relationships

            builder.HasOne(s => s.Page)
                .WithMany(s => s.PagePublishStatusQueries)
                .HasForeignKey(d => d.PageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PageVersion)
                .WithMany(s => s.PagePublishStatusQueries)
                .HasForeignKey(d => d.PageVersionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
