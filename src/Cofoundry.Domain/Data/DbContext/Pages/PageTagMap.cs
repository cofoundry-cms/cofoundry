using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class PageTagMap : IEntityTypeConfiguration<PageTag>
    {
        public void Configure(EntityTypeBuilder<PageTag> builder)
        {
            builder.ToTable("PageTag", DbConstants.CofoundrySchema);

            // Primary Key

            builder.HasKey(s => new { s.PageId, s.TagId });

            // Properties

            builder.Property(s => s.PageId)
                .ValueGeneratedNever();

            builder.Property(s => s.TagId)
                .ValueGeneratedNever();

            // Relationships

            builder.HasOne(s => s.Page)
                .WithMany(s => s.PageTags)
                .HasForeignKey(d => d.PageId);

            builder.HasOne(s => s.Tag)
                .WithMany()
                .HasForeignKey(d => d.TagId);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
