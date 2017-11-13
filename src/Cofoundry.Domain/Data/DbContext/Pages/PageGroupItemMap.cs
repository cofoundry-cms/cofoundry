using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Data
{
    public class PageGroupItemMap : IEntityTypeConfiguration<PageGroupItem>
    {
        public void Configure(EntityTypeBuilder<PageGroupItem> builder)
        {
            builder.ToTable("PageGroupItem", DbConstants.CofoundrySchema);

            // Primary Key
            builder.HasKey(s => new { s.PageId, s.PageGroupId });

            // Properties

            builder.Property(s => s.PageId)
                .ValueGeneratedNever();

            builder.Property(s => s.PageGroupId)
                .ValueGeneratedNever();
            
            // Relationships

            builder.HasOne(s => s.PageGroup)
                .WithMany(s => s.PageGroupItems)
                .HasForeignKey(d => d.PageGroupId);

            builder.HasOne(s => s.Page)
                .WithMany(s => s.PageGroupItems)
                .HasForeignKey(d => d.PageId);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
