using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class PageTagMap : IEntityTypeConfiguration<PageTag>
    {
        public void Create(EntityTypeBuilder<PageTag> builder)
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
                .HasForeignKey(d => d.PageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Tag)
                .WithMany()
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.Restrict);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
