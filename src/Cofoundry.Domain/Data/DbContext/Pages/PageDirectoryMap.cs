using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Data
{
    public class PageDirectoryMap : IEntityTypeConfiguration<PageDirectory>
    {
        public void Configure(EntityTypeBuilder<PageDirectory> builder)
        {
            builder.ToTable("PageDirectory", DbConstants.CofoundrySchema);

            // Primary Key
            builder.HasKey(s => s.PageDirectoryId);

            // Properties
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(s => s.UrlPath)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships
            builder.HasOne(s => s.ParentPageDirectory)
                .WithMany(s => s.ChildPageDirectories)
                .HasForeignKey(d => d.ParentPageDirectoryId);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
