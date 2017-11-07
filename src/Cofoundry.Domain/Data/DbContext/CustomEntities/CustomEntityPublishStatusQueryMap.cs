using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityPublishStatusQueryMap : IEntityTypeConfiguration<CustomEntityPublishStatusQuery>
    {
        public void Create(EntityTypeBuilder<CustomEntityPublishStatusQuery> builder)
        {
            builder.ToTable("CustomEntityPublishStatusQuery", DbConstants.CofoundrySchema);

            builder.HasKey(s => new { s.CustomEntityId, s.PublishStatusQueryId });

            // Relationships

            builder.HasOne(s => s.CustomEntity)
                .WithMany(s => s.CustomEntityPublishStatusQueries)
                .HasForeignKey(d => d.CustomEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.CustomEntityVersion)
                .WithMany(s => s.CustomEntityPublishStatusQueries)
                .HasForeignKey(d => d.CustomEntityVersionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
